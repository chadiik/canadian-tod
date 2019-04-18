using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.vision {

	class CameraSource : ImageSource {

		public override void Start() {
			ImageGrabbed += OnImageGrabbed;
			base.Start();
		}

		public override void Stop() {
			ImageGrabbed -= OnImageGrabbed;
			base.Stop();
		}

		private void OnImageGrabbed(object sender, EventArgs e) {
			try {
				if (m_IsActive) {
					Mat image = new Mat();
					Retrieve(image);
					Update(image);
				}
			}
			catch(Exception ex) {
				if(ex.Message == "Capture error") {
					Logger.Instance.ExceptionLog(ex.ToString());
				}
				else {
					throw ex;
				}
			}
		}
	}
}
