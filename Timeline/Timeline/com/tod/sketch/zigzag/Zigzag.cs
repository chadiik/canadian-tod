using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tod.core;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Threading;

namespace com.tod.sketch.zigzag {

	public class Zigzag {

		public class Parameters {

			public Threshold[] thresholds;
			public int simplify = 1, subdivide = 6;
			public double minArea = 240;

			public static Parameters Default(int numThresholds) {
				Threshold[] thresholds = new Threshold[numThresholds];

				for(int i = 0; i < numThresholds; i++) {
					int low = (int)((double)(i + 1) / (numThresholds + 1) * 255);
					int high = (int)((double)(i + 2) / (numThresholds + 1) * 255);
					Threshold threshold = new Threshold { low = low, high = high };
					//Logger.Instance.WriteLog("threshold {0}: {1}", i, threshold);
					thresholds[i] = threshold;
				}

				return new Parameters() { thresholds = thresholds };
			}
		}

		public event SketchComplete SketchCompleted;

		public void Draw(Portrait portrait, Parameters parameters) {

			Image<Gray, byte> filteredSource = Preprocess(portrait);
			filteredSource = TestData(new Rectangle(0, 0, filteredSource.Width, filteredSource.Height));
			Sketch.ShowProcessImage(filteredSource, "filteredSource");

			Rectangle mapRect = new Rectangle(2, 2, filteredSource.Width - 4, filteredSource.Height - 4);
			Image<Gray, byte> regionsMap = new Image<Gray, byte>(filteredSource.Size);
			List<Contour> allContours = new List<Contour>();
			for (int i = 0; i < parameters.thresholds.Length; i++) {
				Threshold threshold = parameters.thresholds[i];
				Image<Gray, byte> binary = filteredSource.Clone();
				List <Contour> contours = threshold.GetContours(binary, true);
				Sketch.ShowProcessImage(binary, i.ToString());
				regionsMap._Max(binary);

				foreach (Contour contour in contours)
					if (contour.area > parameters.minArea && contour.IsWithin(mapRect) && !contour.ContainedIn(allContours))
						allContours.Add(contour);
			}

			Sketch.ShowProcessImage(regionsMap, "RegionsMap");

			List<LinkedContour> linkedContours = new List<LinkedContour>();
			foreach(Contour contour in allContours) {
				contour.Close();
				contour.Simplify(parameters.simplify);
				//contour.Subdivide(parameters.subdivide);

				LinkedContour linkedContour = LinkedContour.FromContour(contour);
				linkedContours.Add(linkedContour);
			}

			(new Thread(() => {

				Zigzagger zigzagger = new Zigzagger(linkedContours, parameters.thresholds, regionsMap);
				SketchCompleted?.Invoke(new List<Line>() { new Line(new List<Coo> { new Coo(1000, 1000, false), new Coo(1000, 1000, false) } ) });
			})).Start();
		}

		private Image<Gray, byte> Preprocess(Portrait portrait) {

			const int hMax = 500;

			// Scale and apply easy filter
			double scale = 1;
			Image<Bgr, byte> sourceImage = portrait.source.Clone();
			sourceImage = TODFilters.ScaleToFit(sourceImage, hMax, out scale);
			Sketch.ShowProcessImage(sourceImage, "source");

			portrait.ScaleRect(scale);

			CvInvoke.FastNlMeansDenoisingColored(sourceImage, sourceImage, 3f, 10f, 7, 13);

			// Clahe
			const double clipLimit = 4.0;
			Size tileGridSize = new Size(5, 5);

			Image<Gray, byte> grayFrame = sourceImage.Convert<Gray, byte>();
			grayFrame._EqualizeHist();
			Sketch.ShowProcessImage(grayFrame, "Equalized Histogram");

			grayFrame = grayFrame.SmoothBlur(4, 4);

			CvInvoke.CLAHE(grayFrame, clipLimit, tileGridSize, grayFrame);
			Image<Gray, byte> filtered = grayFrame;
			Sketch.ShowProcessImage(filtered, "Clahe");

			//TODFilters.Sharpen(coloredGF, 12);

			return filtered;
		}

		public static void Visualize(List<Point> points, IInputOutputArray image, MCvScalar lineColor, int lineThickness, bool label) {
			for (int i = 1, numPoints = points.Count; i < numPoints; i++) {
				CvInvoke.Line(image, points[i - 1], points[i], lineColor, lineThickness);
				
				if (label)
					CvInvoke.PutText(image, (i - 1).ToString(), points[i - 1], Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, lineColor, 1);
			}
		}

		private static Image<Gray, byte> TestData(Rectangle rect) {
			Image<Gray, byte> data = new Image<Gray, byte>(rect.Width, rect.Height);

			int wn = rect.Width / 6,
				hn = rect.Height / 6;
			Rectangle larger = new Rectangle(rect.X + wn, rect.Y + hn, rect.Width - wn * 2, rect.Height - hn * 2);
			Rectangle smaller = new Rectangle(rect.X + wn * 2, rect.Y + hn * 2, rect.Width - wn * 4, rect.Height - hn * 4);

			CvInvoke.Rectangle(data, larger, new MCvScalar(100, 100, 100), -1);
			CvInvoke.Rectangle(data, smaller, new MCvScalar(200, 200, 200), -1);

			return data;
		}
	}
}
