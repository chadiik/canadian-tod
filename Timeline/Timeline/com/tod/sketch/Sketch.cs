using chadiik.algorithms;
using com.tod.core;
using com.tod.sketch.hatch;
using com.tod.sketch.zigzag;
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

		public enum Version { Legacy, Zigzag, Hatch };

		public static readonly MCvScalar BLACK = new Bgr(Color.Black).MCvScalar;
		public static readonly MCvScalar WHITE = new Bgr(Color.White).MCvScalar;

		public static event DisplayEntryRequest DisplayEntryRequested;
		public static event SketchComplete SketchCompleted;
		public static event SketchComplete DrawToWallRequested;

        private TODDraw m_TODDraw;
		private Zigzag m_Zigzag;
		private Hatch m_Hatch;
		private Version m_Version;

		public Sketch(Version version) {

			m_Version = version;

			switch (m_Version) {
				case Version.Legacy:
					m_TODDraw = new TODDraw();
					m_TODDraw.SketchCompleted += path => SketchCompleted?.Invoke(path);
					break;

				case Version.Zigzag:
					m_Zigzag = new Zigzag();
					m_Zigzag.SketchCompleted += path => SketchCompleted?.Invoke(path);
					break;

				case Version.Hatch:
					m_Hatch = new Hatch();
					m_Hatch.SketchCompleted += path => SketchCompleted?.Invoke(path);
					break;
			}
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

			switch (m_Version) {
				case Version.Legacy:
					m_TODDraw.Draw(portrait);
					break;

				case Version.Zigzag:
					m_Zigzag.Draw(portrait, Zigzag.Parameters.Default(3));
					break;

				case Version.Hatch:
					m_Hatch.Draw(portrait.source, Hatch.Parameters.Default(3));
					break;
			}
		}

		public static void ShowProcessImage(Image<Bgr, byte> image, string txt = null) {

			if (txt != null) {
				CvInvoke.Rectangle(image, new Rectangle(1, 1, image.Width - 2, 20), WHITE, -1);
				CvInvoke.PutText(image, txt, new Point(18, 18), FontFace.HersheyPlain, 1, BLACK, 1);
			}

			DisplayEntryRequested?.Invoke(image);
		}

		public static void ShowProcessImage(Image<Gray, byte> image, string txt = null) {

			if (txt != null) {
				CvInvoke.Rectangle(image, new Rectangle(1, 1, image.Width - 2, 20), WHITE, -1);
				CvInvoke.PutText(image, txt, new Point(18, 18), FontFace.HersheyPlain, 1, BLACK, 1);
			}

			DisplayEntryRequested?.Invoke(image);
		}

        public static void DrawToWall(List<TP> path) {

            DrawToWallRequested?.Invoke(path);
        }

		public static List<TP> Optimize(List<TP> original, float simplificationTolerance, double breakDistance) {

			// Copy all down
			List<Point> path = new List<Point>();
			for (int i = 0, numPoints = original.Count; i < numPoints; i++) {
				TP tp = original[i];
				if (tp.IsDown) {
					path.Add(tp.ToPoint());
				}
			}

			return Optimize(path, simplificationTolerance, breakDistance);
		}

		public static List<TP> Optimize(List<Point> path, float simplificationTolerance, double breakDistance) {

			if (simplificationTolerance > float.Epsilon)
				path = SimplifyJS.Simplify(path, simplificationTolerance);

            double pathLength = 0;
            int penUps = 2;
            int coordinates = 0;

            List<TP> optimized = new List<TP> { TP.PenUp };
            Point previous = default(Point);
            for (int i = 0, numPoints = path.Count; i < numPoints; i++) {
                Point current = path[i];
                double vx = current.X - previous.X;
                double vy = current.Y - previous.Y;
                double d = Math.Sqrt(vx * vx + vy * vy);
                pathLength += d;
                if (d < breakDistance) {
                    optimized.Add(new TP(current.X, current.Y));
                    coordinates++;
                }
                else {
                    optimized.Add(TP.PenUp);
                    penUps++;
                }

                previous = current;
            }

            optimized.Add(TP.PenUp);

            Logger.Instance.WriteLog("Path length = {0} mm", Math.Floor(pathLength));
            Logger.Instance.WriteLog("Coordinates = {0}x", coordinates);
            Logger.Instance.WriteLog("Pen ups = {0}x", penUps);

            return optimized;
        }
    }
}
