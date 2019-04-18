using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.tod.stream {

	public class ArduinoCom : ComInterface {

		private string _portName;
		private SerialPort _port;
        public delegate void DelDataReceived(string data);
		private DelDataReceived _dataCallback;
		private Callback _success, _failure;

		public delegate void DelSetPortName(string portName);
		public DelSetPortName setPortNameCallback;

		public ArduinoCom() {
			
		}

		public void Connect(string portName, DelDataReceived callback, Callback success = null, Callback failure = null) {
			base.Connect(success, failure);
			_dataCallback = callback;
			_portName = portName;
			_success = success;
			_failure = failure;

			ConnectSequence();
		}

		private void ConnectSequence(){

			string[] coms = new string[]{
				_portName, "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM2", "COM9", "COM1"
			};

			for (int i = 0; i < coms.Length; i++) {
				try {
					
					_portName = coms[i];
					Logger.Instance.WriteLog("Trying {0}", _portName);

					_port = new SerialPort(_portName, 115200, Parity.None, 8, StopBits.One);
					_port.DtrEnable = true;
					_port.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
					_port.Open();

					if (setPortNameCallback != null) setPortNameCallback.Invoke(_portName);
					_success?.Invoke();

					return;
				}
				catch (IOException e) {
					Logger.Instance.ExceptionLog("{0}", e.ToString());

					Thread.Sleep(100);
				}
			}

			_failure?.Invoke();
		}

		private static DateTime START = DateTime.UtcNow;
		private void OnDataReceived(object sender, SerialDataReceivedEventArgs e) {
			if (_port.IsOpen) {

				while (_port.BytesToRead > 0) {
					string data = _port.ReadLine();
					double millis = DateTime.UtcNow.Subtract(START).TotalMilliseconds;
					_dataCallback(data);
				}
			}
		}

		override public void Disconnect() {
			base.Disconnect();

			if (_port != null && _port.IsOpen)
				_port.Close();
		}

		public void ResetConnection() {
			if (_port != null && _port.IsOpen) {
				Thread.Sleep(200);
				_port.Close();

				Thread.Sleep(1000);
				ConnectSequence();
				Thread.Sleep(500);
			}
		}

		override public void Send(ref string value) {
			if (_port != null && _port.IsOpen) {
				Logger.Instance.SilentLog("[ArduinoCom] Sending " + value);
				_port.WriteLine(value);
			}
		}

		public bool IsConnected {
			get {
				return _port != null && _port.IsOpen;
			}
		}
	}
}
