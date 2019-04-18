using com.tod.core;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch {

	public delegate void DisplayEntryRequest(IImage image);
	public delegate void SketchComplete(List<TP> path);

	public class Sketch {

		public static readonly MCvScalar BLACK = new Bgr(Color.Black).MCvScalar;
		public static readonly MCvScalar WHITE = new Bgr(Color.White).MCvScalar;

		public static event DisplayEntryRequest DisplayEntryRequested;
		public static event SketchComplete SketchCompleted;

		private TODDraw m_TODDraw;

		public Sketch() {

			TODDraw.SketchCompleted += path => SketchCompleted?.Invoke(path);
			m_TODDraw = new TODDraw();
		}

		public void Test(string filename = "0.png") {

			string filepath = string.Format("{0}/{1}", Config.files.assetsDir, filename);

			Image<Bgr, byte> faceImage = new Image<Bgr, byte>(filepath);
			Rectangle faceRect = new Rectangle(0, 0, faceImage.Width, faceImage.Height);
			Logger.Instance.SilentLog("Testing sketch with {0} x {1}", filepath, faceRect);
			Portrait face = new Portrait(0, 0, faceImage, ref faceRect, 1f);
			Draw(face);
		}

		public void Draw(Portrait portrait) {

			m_TODDraw.Draw(portrait);
		}

		public static void ShowProcessImage(Image<Bgr, byte> image, string txt = null) {

			if (txt != null) {
				CvInvoke.Rectangle(image, new Rectangle(1, 1, image.Width - 2, 20), WHITE, -1);
				CvInvoke.PutText(image, txt, new Point(18, 18), FontFace.HersheyPlain, 1, BLACK, 1);
			}

			DisplayEntryRequested?.Invoke(image);
		}
	}
}
