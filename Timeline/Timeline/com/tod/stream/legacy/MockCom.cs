using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.tod.stream {
	public class MockCom : ArduinoCom {

		private string _portName;
		public delegate void DelDataReceived(string data);
		private DelDataReceived _dataCallback;

		public MockCom() {

		}

		public void Connect(string portName, DelDataReceived callback, Callback success = null, Callback failure = null) {
			_dataCallback = callback;
			success();
			OnDataReceived("0");
		}

		private static DateTime START = DateTime.UtcNow;
		private void OnDataReceived(string mockData) {
			double millis = DateTime.UtcNow.Subtract(START).TotalMilliseconds;
			_dataCallback(mockData);
		}

		override public void Disconnect() {

		}

		private string _lastVal = "z";
		override public void Send(ref string value) {

			_lastVal = value[0].ToString();
		}

		public bool Online {
			get {
				return true;
			}
		}

		internal void ProcessLastValue() {

			for (int i = 0; i < 500; i++) {
				switch (_lastVal) {
					case "c":
						_lastVal = "z";
						OnDataReceived("c");
						break;

					case "q":
						_lastVal = "z";
						OnDataReceived("q");
						break;
				}
			}
		}
	}
}
