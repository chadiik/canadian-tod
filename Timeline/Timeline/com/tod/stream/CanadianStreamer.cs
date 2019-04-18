using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.tod.stream {

	public class CanadianStreamer : Streamer {

		private const string DEFAULT_COM_PORT = "COM5";
		private const bool DEBUG = true;

		public event Connection ConnectionEstablished;
		public event Connection ConnectionFailed;
		public event Connection ConnectionClosed;

		public event StreamState CalibrationCompleted;
		public event StreamState CalibrationStarted;

		public event StreamState StreamCompleted;
		public event StreamState StreamPaused;
		public event StreamState StreamStarted;

		private ArduinoCom m_ArduinoSerial;

		public CanadianStreamer() {
			m_ArduinoSerial = new ArduinoCom();
		}

		public void Open() {

			if (m_ArduinoSerial.IsConnected) {
				ConnectionEstablished?.Invoke();
				return;
			}

			try {
				if (DEBUG) {
					ConnectionEstablished?.Invoke();
				}
				else {
					m_ArduinoSerial.Connect(
						DEFAULT_COM_PORT,
						data => Receive(data), // Data sent from Arduino
						() => ConnectionEstablished?.Invoke(), // Success
						() => ConnectionFailed?.Invoke() // Failure
					);
				}
			}
			catch(Exception ex) {
				Logger.Instance.ExceptionLog("Streamer.Open() error: {0}", ex.ToString());
				ConnectionFailed?.Invoke();
			}
		}

		public void Close() {

			try {
				m_ArduinoSerial.Disconnect();
			}
			catch(Exception ex) {
				Logger.Instance.ExceptionLog("Streamer.Close() error: {0}", ex.ToString());
			}
			finally {
				ConnectionClosed?.Invoke();
			}
		}

		public void Calibrate() {

			try {
				if (DEBUG) {
					Send("Calibrate or something");
					CalibrationStarted?.Invoke();
					CalibrationCompleted?.Invoke();
				}
				else {

				}
			}
			catch (Exception ex) {
				Logger.Instance.ExceptionLog("Streamer.Calibrate() error: {0}", ex.ToString());
			}
		}

		public void Send(object value) {
			string data = (string)value; // or something

			try {
				if (DEBUG) {

				}
				else {
					m_ArduinoSerial.Send(ref data);
				}
			}
			catch (Exception ex) {
				Logger.Instance.ExceptionLog("Streamer.Send({1}) error: {0}", ex.ToString(), value);
			}
		}

		private static Thread s_DebugThread;
		public void Send(int xsteps, int ssteps, int esteps, int wrist) {
			string command = string.Format("{0}_{1}_{2}_{3}", xsteps, ssteps, esteps, wrist);

			try {
				if (DEBUG) {
					if (s_DebugThread == null) {
						s_DebugThread = new Thread(() => {
							Thread.Sleep(3000);
							StreamCompleted?.Invoke();
							s_DebugThread = null;
						});
						s_DebugThread.Start();
					}
				}
				else {
					// Build/manage queue
					m_ArduinoSerial.Send(ref command);
				}
			}
			catch (Exception ex) {
				Logger.Instance.ExceptionLog("Streamer.Send({1}, {2}, {3}, {4}) error: {0}", ex.ToString(), xsteps, ssteps, esteps, wrist);
			}
		}

		public void Receive(object value) {

			try {
				string data = (string)value;
				throw new NotImplementedException();
			}
			catch (Exception ex) {
				Logger.Instance.ExceptionLog("Streamer.Receive({1}) error: {0}", ex.ToString(), value);
			}
		}
	}
}
