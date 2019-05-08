using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.tod.vision {

	class ImageSource : VideoCapture, ISource {

		public event ImageEvent SourceUpdated;

		private Thread m_ThrottleThread;
		private Mat m_ThrottledMat;
		protected bool m_IsActive;

		public ImageSource() { }
		public ImageSource(string filepath) : base(filepath) { }

		public bool IsActive {
			get { return m_IsActive; }
			set { m_IsActive = value; }
		}

		public virtual int Fps {
			set { SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, value); }
			get { return (int)GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps); }
		}

		public virtual void Start() {
			m_IsActive = true;
			base.Start();
		}

		public virtual new void Stop() {
			m_IsActive = false;
			base.Stop();
			Dispose();
		}

		protected void Update(Mat image) {

			m_ThrottledMat = image;
			if (m_ThrottleThread == null) {
				m_ThrottleThread = new Thread(() => {
					while (m_IsActive) {
						Thread.Sleep(10);
						if (m_ThrottledMat != null) {
							SourceUpdated?.Invoke(m_ThrottledMat);
							m_ThrottledMat = null;
						}
					}
					m_ThrottleThread.Abort();
					m_ThrottleThread = null;
				});
				m_ThrottleThread.Start();
			}
			return;
			//
			SourceUpdated?.Invoke(image);
		}
	}
}
