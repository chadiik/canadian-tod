using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.tod.stream {
	public class ComInterface {
		private static ComInterface _instance;
		public static ComInterface Instance {
			get {
				return _instance;
			}
			set {
				_instance = value;
			}
		}
		public ComInterface() {
			
		}

		public delegate void Callback();
		virtual public void Connect(Callback sucess, Callback failure) {

		}

		virtual public void Disconnect() {

		}

		virtual public void Send(int value) {

		}

		virtual public void Send(ref string value) {
			
		}
	}
}
