using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.vision {

	class ImageSource : VideoCapture, ISource {

		public event ImageEvent SourceUpdated;

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
			SourceUpdated?.Invoke(image);
		}
	}
}
