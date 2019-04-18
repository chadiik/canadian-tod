using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using System.Drawing;
using com.tod.core;

namespace com.tod.vision {

	public enum Source { Camera, VideoFile }

	public delegate void ImageEvent(Mat image);

	public class Vision {

		public float sourceUpdateRate = 1f / 15;

		public event ImageEvent SourceUpdated; 
		public event ImageEvent FaceDetected;
		public event PortraitEvent CandidateFound;
		public event PortraitEvent PortraitCreated;

		private ISource m_Source;
		private FaceDetection m_FaceDetection;
		private long m_LastDetection;

		public Vision(Source source) {

			switch (source) {

				case Source.VideoFile:
				default:
					try {

						Motion motion = new Motion();
						motion.MovementDetected += (Mat image) => {
							if (ThrottleCompleted())
								DetectFaces(image);
						};

						m_Source = source == Source.VideoFile ? new VideoFileSource(Config.files.video0) as ISource : new CameraSource() as ISource;
						m_Source.SourceUpdated += (Mat image) => {
							motion.Process(image);
							SourceUpdated?.Invoke(image);
						};

						m_FaceDetection = new FaceDetection(
							new Rectangle(0, 0, m_Source.Width, m_Source.Height), 
							marginH: .05f, 
							marginV: .0875f);

						m_Source.Fps = Config.fps;
						m_Source.Start();
					}
					catch (NullReferenceException excpt) {
						Logger.Instance.ExceptionLog(excpt.Message);
					}

					break;
			}

			m_FaceDetection.FaceDetected += OnFaceDetected;
			FacesPool.PortraitCreated += OnPortraitCreated;
			FacesPool.CandidateFound += OnCandidateFound;
		}

		public void Stop() {
			m_Source.Stop();
		}

		private bool ThrottleCompleted() {
			TimeSpan span = TimeSpan.FromTicks(DateTime.Now.Ticks - m_LastDetection);
			return span.TotalSeconds > sourceUpdateRate;
		}

		private void DetectFaces(Mat image) {
			long now = DateTime.Now.Ticks;
			m_LastDetection = now;

			m_FaceDetection.Process(image, image.Clone());
		}

		private void OnFaceDetected(Mat image) {
			FaceDetected?.Invoke(image);
		}

		private void OnPortraitCreated(Portrait portrait) {
			PortraitCreated?.Invoke(portrait);
		}

		private void OnCandidateFound(Portrait portrait) {
			//Logger.Instance.SilentLog("Candidate found");
			CandidateFound?.Invoke(portrait);
		}
	}
}
