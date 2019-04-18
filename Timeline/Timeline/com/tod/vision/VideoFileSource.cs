using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.tod.vision {

	class VideoFileSource : ImageSource {

		private long m_LastFrameTime;

		public VideoFileSource(string filepath) : base(filepath) {}

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

					int delay = 1000 / Fps - (int)(Config.time.ElapsedMillis - m_LastFrameTime);
					if (delay > 1)
						Thread.Sleep(delay);
					m_LastFrameTime = Config.time.ElapsedMillis;
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
