using com.tod.core;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.tod.sketch.hatch {

	public class Hatch {

		public class Parameters {

			public Threshold[] thresholds;
			public int simplify = 1, subdivide = 6;
			public double minArea = 240;

			public static Parameters Default(int numThresholds) {
				Threshold[] thresholds = new Threshold[numThresholds + 1];

				for (int i = 0; i < numThresholds; i++) {
					int low = (int)((double)(i + 1) / (numThresholds + 1) * 255);
					int high = (int)((double)(i + 2) / (numThresholds + 1) * 255);
					
					thresholds[i + 1] = new Threshold { low = low, high = high };
					//Logger.Instance.WriteLog("threshold {0}: {1}", i, thresholds[i]);
				}

				thresholds[0] = new Threshold { low = 0, high = thresholds[1].low};

				return new Parameters() { thresholds = thresholds };
			}
		}

		public event SketchComplete SketchCompleted;

		public void Draw(Image<Bgr, byte> portrait, Parameters parameters) {

			const int hMax = 500;
			double scale = 1;
			Image<Bgr, byte> sourceImage = TODFilters.ScaleToFit(portrait, hMax, out scale);
			Sketch.ShowProcessImage(sourceImage, "source");

			Image<Gray, byte> filteredSource = Preprocess(sourceImage.Clone());
			//filteredSource = TestData(new Rectangle(0, 0, filteredSource.Width, filteredSource.Height));
			filteredSource = filteredSource.Add(new Gray(1));
			Sketch.ShowProcessImage(filteredSource, "filteredSource");

			Rectangle mapRect = new Rectangle(2, 2, filteredSource.Width - 4, filteredSource.Height - 4);
			Image<Gray, byte> regionsMap = new Image<Gray, byte>(filteredSource.Size);
			List<Contour> allContours = new List<Contour>(),
				firstContours = new List<Contour>();
			for (int i = 0; i < parameters.thresholds.Length; i++) {
				Threshold threshold = parameters.thresholds[i];
				Image<Gray, byte> binary = filteredSource.Clone();
				List<Contour> contours = threshold.GetContours(binary, true);
				Sketch.ShowProcessImage(binary, i.ToString());
				regionsMap._Max(binary);

				foreach (Contour contour in contours) {
					if (contour.area > parameters.minArea && contour.IsWithin(mapRect) && !contour.ContainedIn(allContours)) {
						allContours.Add(contour);
						if (i == 1)
							firstContours.Add(contour);
					}
				}
			}

			Image<Gray, byte> regionsMask = new Image<Gray, byte>(filteredSource.Size);
			foreach (Contour contour in allContours)
				contour.Fill(regionsMask, new Gray(255));
			GetCenterMask(mapRect, parameters.minArea / 8.0).Fill(regionsMask, new Gray(255));

			regionsMask._Dilate(4);
			regionsMask._Erode(2);
			regionsMask._Dilate(4);


			Sketch.ShowProcessImage(regionsMap.Clone(), "RegionsMap unmasked");
			regionsMap._Min(regionsMask);
			regionsMask._ThresholdBinaryInv(new Gray(127), new Gray(255));
			regionsMap._Max(regionsMask);
			Sketch.ShowProcessImage(regionsMask, "RegionsMask");
			Sketch.ShowProcessImage(regionsMap, "RegionsMap");

			//(new Thread(() => {

				Hatcher hatcher = new Hatcher(parameters.thresholds, regionsMap);

				hatcher.ProcessCompleted += () => {
					Logger.Instance.WriteLog("Hatch completed");
					Image<Bgr, byte> preview = new Image<Bgr, byte>(regionsMap.Width, regionsMap.Height, new Bgr(255, 255, 255));
					//TP.Visualize(hatcher.path, preview, new MCvScalar(0), 1);
					SketchPreview(hatcher.path, preview, new MCvScalar(0), 1);

					SketchCompleted?.Invoke(hatcher.path);
				};

				hatcher.Process(sourceImage.Clone(), firstContours);
			//})).Start();
		}

		private Image<Gray, byte> Preprocess(Image<Bgr, byte> sourceImage) {

			CvInvoke.FastNlMeansDenoisingColored(sourceImage, sourceImage, 3f, 10f, 7, 13);

			// Clahe
			const double clipLimit = 4.0;
			Size tileGridSize = new Size(4, 4);

			Image<Gray, byte> grayFrame = sourceImage.Convert<Gray, byte>();
			grayFrame._EqualizeHist();
			Sketch.ShowProcessImage(grayFrame, "Equalized Histogram");

			grayFrame = grayFrame.SmoothBlur(2, 2);

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

		private static Contour GetCenterMask(Rectangle rect, double edgeLength) {

			Contour contour = Contour.FromRect(rect);
			contour.Subdivide(edgeLength);

			Random rand = new Random();
			Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
			for(int i = 0, numPoints = contour.points.Count; i < numPoints; i++) {
				Point p = contour.points[i];
				double toCenterX = center.X - p.X,
					toCenterY = center.Y - p.Y;
				double offset = .2 + rand.NextDouble() * .4;
				contour.points[i] = new Point((int)(p.X + toCenterX * offset), (int)(p.Y + toCenterY * offset));
			}

			return contour;
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

		private static void SketchPreview(List<TP> points, Image<Bgr, byte> image, MCvScalar lineColor, int lineThickness) {

			(new Thread(() => {
				bool penDown = false;
				TP previous = TP.Null;
				for (int i = 0, numPoints = points.Count; i < numPoints; i++) {
					if (points[i].IsDown) {
						if (!penDown) {
							penDown = true;
						}
						else {
							CvInvoke.Line(image, previous.ToPoint(), points[i].ToPoint(), lineColor, lineThickness);
						}
						previous = points[i];
					}
					else {
						penDown = false;
						Thread.Sleep(3);
					}

					if(i % 10 == 0)
						Thread.Sleep(1);
					Sketch.ShowProcessImage(image, "Preview");
				}
			})).Start();
		}
	}
}
