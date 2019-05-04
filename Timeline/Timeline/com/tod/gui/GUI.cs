using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.gui {

	public abstract class GUI {

		private static GUI m_Instance;

		public static GUI Instance {
			get { return m_Instance; }
			set { m_Instance = value; }
		}

		public GUI() {
			Logger.Instance.Silent += Silent;
			Logger.Instance.Write += Write;
			Logger.Instance.Notification += Notification;
			Logger.Instance.Exception += Exception;
			Logger.Instance.Stream += Stream;
        }

		public abstract void Silent(string message);
		public abstract void Write(string message);
		public abstract void Notification(string message);
		public abstract void Exception(string message);
		public abstract void Stream(string message);
    }

	public interface IGUIImage {

	}
}
