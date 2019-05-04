using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod {
	class Logger {

		public delegate void Log(string message);

		private static Logger m_Instance;
		public static Logger Instance {
			get {
				if (m_Instance == null) m_Instance = new Logger();
				return m_Instance;
			}
		}

		public event Log Silent;
		public void SilentLog(string message, params object[] args) {
			Silent?.Invoke(string.Format(message, args));
		}

		public event Log Write;
		public void WriteLog(string message, params object[] args) {
			Write?.Invoke(string.Format(message, args));
		}

		public event Log Exception;
		public void ExceptionLog(string message, params object[] args) {
			Exception?.Invoke(string.Format(message, args));
		}

		public event Log Notification;
		public void NotificationLog(string message, params object[] args) {
			Notification?.Invoke(string.Format(message, args));
		}

        public event Log Stream;
        public void StreamLog(string message, params object[] args) {
            Stream?.Invoke(string.Format(message, args));
        }
    }
}
