using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.tod.stream {

	public class CanadianStreamer : Streamer {

		private const string DEFAULT_COM_PORT = "COM5";

		public event Connection ConnectionEstablished;
		public event Connection ConnectionFailed;
		public event Connection ConnectionClosed;

		public event StreamState CalibrationCompleted;
		public event StreamState CalibrationStarted;

		public event StreamState StreamCompleted;
		public event StreamState StreamPaused;
		public event StreamState StreamStarted;

		private ArduinoCom m_ArduinoSerial;
        private Queue<string> m_StreamQueue;
        private Thread m_Streamer;
        private object m_StreamLock = new object();
        private bool m_IsStreaming = false;
        private bool m_AwaitingFeedback = true;
        private int m_CountPackets = 0;
        private bool m_Debug = true;

        public CanadianStreamer(bool debug) {
            m_Debug = debug;
            Logger.Instance.StreamLog("debug: {0}", m_Debug);
            m_ArduinoSerial = new ArduinoCom();
            m_StreamQueue = new Queue<string>(8000);
        }

		public void Open() {

            m_StreamQueue.Clear();
			m_AwaitingFeedback = false;

			if (m_ArduinoSerial.IsConnected) {
                lock (m_StreamLock) {
                    m_AwaitingFeedback = false;
                }
                ConnectionEstablished?.Invoke();
				return;
			}

			try {
				if (m_Debug) {
					ConnectionEstablished?.Invoke();
				}
				else {

                    m_ArduinoSerial.Connect(
						DEFAULT_COM_PORT,
						data => Receive(data), // Data sent from Arduino
						() => Logger.Instance.WriteLog("Awaiting feedback")/*ConnectionEstablished?.Invoke()*/, // Success
						() => ConnectionFailed?.Invoke() // Failure
					);
                }
			}
			catch(Exception ex) {
				Logger.Instance.StreamLog("Streamer.Open() error: {0}", ex.Message);
				ConnectionFailed?.Invoke();
			}
		}

        public void Pause() {
            StreamPaused?.Invoke();

            m_IsStreaming = false;
        }

        public void Resume() {
            StreamStarted?.Invoke();

            m_IsStreaming = true;
            m_CountPackets = 0;

            if (m_Streamer == null) {
                m_Streamer = new Thread(ConsumeStreamQueue);
                m_Streamer.Start();
            }

            Logger.Instance.WriteLog("Stream started");
        }

		public void Close() {

			try {
				m_ArduinoSerial.Disconnect();
                m_Streamer.Abort();
			}
			catch(Exception ex) {
				Logger.Instance.StreamLog("Streamer.Close() error: {0}", ex.Message);
			}
			finally {
				ConnectionClosed?.Invoke();
			}
		}

		public void Calibrate() {

            const string command = "c";
            Logger.Instance.WriteLog("Calibrating");
            try {
				if (m_Debug) {
                    CalibrationStarted?.Invoke();
					CalibrationCompleted?.Invoke();
				}
				else {
                    m_StreamQueue.Enqueue(command);
                    CalibrationStarted?.Invoke();
                }
			}
			catch (Exception ex) {
				Logger.Instance.StreamLog("Streamer.Calibrate() error: {0}", ex.Message);
                throw;
            }
		}

		public void Stream(int xsteps, int ssteps, int esteps, int wrist) {
            const string pad5 = "D5";
			string command = xsteps == 0 && ssteps == 0 && esteps == 0 && wrist == 0 ?
				"1" :
				string.Format("q{0}_{1}_{2}_{3}", xsteps.ToString(pad5), ssteps.ToString(pad5), esteps.ToString(pad5), wrist);

			Logger.Instance.SilentLog("Stream {0}", command); 
			try {
				if (m_Debug) {
					m_StreamQueue.Enqueue("1");
				}
				else {
                    m_StreamQueue.Enqueue(command);
				}
			}
			catch (Exception ex) {
				Logger.Instance.StreamLog("error: {0} in Streamer.Send({1}, {2}, {3}, {4}) ", ex.Message, xsteps, ssteps, esteps, wrist);
			}
		}

        private void ConsumeStreamQueue() {

            while (true) {
                
                if (m_IsStreaming && !m_AwaitingFeedback && m_StreamQueue.Count > 0) {
                    string command = m_StreamQueue.Dequeue();
                    lock (m_StreamLock) {
                        m_AwaitingFeedback = true;
                    }

					if (command == "1") {
						Send("q00000_00000_00000_0");
						Thread.Sleep(1000);
						m_StreamQueue.Clear();
						StreamCompleted?.Invoke();
					}
					else {
						Send(command);
					}

                    Thread.Sleep(10);
                }
                else {
                    Thread.Sleep(50);
                }
            }
        }

        private void Send(string value) {
            
            Logger.Instance.StreamLog("o: '{0}'", value);

            try {
                if (m_Debug) {

                }
                else {
                    m_ArduinoSerial.Send(ref value);
                }
            }
            catch (Exception ex) {
                Logger.Instance.StreamLog("Streamer.Send({1}) error: {0}", ex.Message, value);
                throw;
            }
        }

        private void Receive(string value) {

			try {
                Logger.Instance.StreamLog("i: '{0}'", value);
                switch (value) {
                    case "c":
                        lock (m_StreamLock) {
                            m_AwaitingFeedback = false;
                        }
                        CalibrationCompleted?.Invoke();
                        break;

                    case "b":
                        lock (m_StreamLock) {
                            m_AwaitingFeedback = true;
                        }
                        Logger.Instance.StreamLog("Sent packet {0}", m_CountPackets++);
                        break;

                    case "r":
                        lock (m_StreamLock) {
                            m_AwaitingFeedback = false;
                        }
                        break;

                    default:
                        lock (m_StreamLock) {
                            m_AwaitingFeedback = false;
                        }
                        ConnectionEstablished?.Invoke();
                        break;
                }
            }
			catch (Exception ex) {
				Logger.Instance.StreamLog("Streamer.Receive({1}) error: {0}", ex.Message, value);
                throw;
            }
		}
	}
}
