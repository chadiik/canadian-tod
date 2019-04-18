using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace com.tod.stream {

	public class PathStream {

		public enum STATE { DISCONNECTED, IDLE, STREAMING, OP };
		public STATE state = STATE.DISCONNECTED;

		public static string COMPORT = "COM5";

		public enum STREAM { OFF, STREAMING, BUFFER_FULL };
		public STREAM streamState = STREAM.OFF;
		public const string
			STREAM_CONFIRMATION = "q",
			STREAM_BUFFER_FULL = "b",
			STREAM_RESUME = "r";

		public const int RESET_COM_INTERVAL = 2;
		public const int SENT_DATA_LOG_INTERVAL = 50;

		public bool IsStreaming {
			get {
				return (streamState == STREAM.STREAMING);
			}
		}

		public bool calibrated;
		public int index;
		private List<int> _path;
		private bool _start;
		private int _resetComCounter;
		private int _sentDataLogInterval;

		public void SetPath(List<int> path) {
			_path = path;
		}

		public ArduinoCom serialCom;


		public PathStream() {
			
		}

		public void SetupConnection(bool debug = false) {
			if (debug) {
				//serialCom = new MockCom();
				serialCom.Connect(COMPORT, OnData, OnConnect, OnFail);
			}
			else {
				serialCom = new ArduinoCom();
				serialCom.Connect(COMPORT, OnData, OnConnect, OnFail);
			}
		}

		protected virtual void OnConnect() {
			Log("Connection succeeded");
		}

		protected virtual void OnFail() {
			Log("Connection failed");
		}

		protected virtual void Calibrate() {

			if (calibrated) return;

			Logger.Instance.WriteLog("Calibrating");

			string calCmd = TODCmd.Calibrate();
			Send(ref calCmd);
		}

		protected virtual void OnCalibrationComplete() {
			Logger.Instance.WriteLog("****************** Calibrated");
			calibrated = true;

			if (_start) {
				state = STATE.STREAMING;
				streamState = STREAM.STREAMING;
				index = 0;
			}

			if (state == STATE.STREAMING && streamState == STREAM.STREAMING) {
				Logger.Instance.WriteLog("Calibrated c, sending next.");
				SendNext();
			}

		}

		protected void Pause() {
			Log("Paused streaming.");
			state = STATE.IDLE;
		}

		protected virtual void OnStreamResume() {
			Log("Resumed streaming.");
		}

		protected virtual void OnStreamComplete() {
		}

		public void Start() {

			if (serialCom.IsConnected) {
				_start = true;
				Logger.Instance.WriteLog("ONLINE");
				_resetComCounter = RESET_COM_INTERVAL;
				_sentDataLogInterval = SENT_DATA_LOG_INTERVAL;
			}
			else {
				Log(" Could not connect.");
			}
		}

		private static string Format(string str, int stringLength = 5) {
			if (str.Length > stringLength) str = "";
			while (str.Length < stringLength) {
				str = "0" + str;
			}
			return str;
		}

		private string _receivedDataChar = "z";
		private int _countReceived = 0;
		private void Log(string data) {
			Logger.Instance.WriteLog(Format(_countReceived.ToString(), 8) + ") " + data);
		}
		private void OnData(string data) {
			if (data.Length < 1) return;
			_countReceived++;

			_receivedDataChar = data[0].ToString();

			switch (state) {

				case STATE.DISCONNECTED:
					switch (_receivedDataChar) {
						case "0":
							state = STATE.IDLE;
							Logger.Instance.WriteLog("IDLE");
							Log(_receivedDataChar);
							if (!calibrated) {
								string penupCmd = TODCmd.Move(50, 50, 50, true);
								Send(ref penupCmd);

								Thread.Sleep(2000);
								Calibrate();
							}
							break;

						case "m":
							break;
					}
					break;

				case STATE.IDLE:
					switch (_receivedDataChar) {
						case "c":
							Log(_receivedDataChar);
							OnCalibrationComplete();
							break;
					}
					break;

				case STATE.STREAMING:
					switch (_receivedDataChar) {

						case "c":
							Log(_receivedDataChar);
							OnCalibrationComplete();
							break;

						case STREAM_BUFFER_FULL:
							Log(_receivedDataChar);
							streamState = STREAM.BUFFER_FULL;
							break;

						case STREAM_CONFIRMATION:
							//streamState = STREAM.STREAMING; do nothing it's drawing
							break;

						case STREAM_RESUME:

							_resetComCounter--;
							if (_resetComCounter == int.MinValue) {
								_resetComCounter = RESET_COM_INTERVAL;
								Logger.Instance.WriteLog("Resetting connection to serial...");
								serialCom.ResetConnection();
							}

							Log(_receivedDataChar);
							streamState = STREAM.STREAMING;
							OnStreamResume();
							break;
					}

					if(calibrated) SendNext();

					if (index >= _path.Count) {
						// COMPLETE
						Logger.Instance.WriteLog("Sent {0} coordinates.", sentCoords.ToString());
						state = STATE.IDLE;
						streamState = STREAM.OFF;
						OnStreamComplete();
					}

					break;
			}
		}

		private int penInterval = 150;
		private int _penCounter = 150;
		int sentCoords = 0;
		private bool SendNext() {

			if (!calibrated) {
				Thread.Sleep(1000);
				Calibrate();
			}

			if (index >= _path.Count - 5) {
				Logger.Instance.WriteLog("Complete");
				return true;
			}
			if (state != STATE.STREAMING || streamState != STREAM.STREAMING) return false;

			int xSteps = _path[index++];
			int shoulderSteps = _path[index++];
			int elbowSteps = _path[index++];
			bool penUp = _path[index++] > 0;

			Thread.Sleep(20);

			string command = TODCmd.Stream(xSteps, shoulderSteps, elbowSteps, penUp);
			Send(ref command);
			
			sentCoords++;

			return true;
		}

		public void Send(ref string cmd) {
			serialCom.Send(ref cmd);

			if (_sentDataLogInterval-- <= 0) {

				_sentDataLogInterval = SENT_DATA_LOG_INTERVAL;
				Log("Sending command: " + cmd);
			}
		}

		#region helper function

		public static void Format(ref string str, int stringLength) {
			if (str.Length > stringLength) str = "";
			while (str.Length < stringLength) {
				str = "0" + str;
			}
		}

		#endregion

	}
}
