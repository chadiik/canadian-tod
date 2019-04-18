using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using com.tod.sketch.path;

namespace com.tod.sketch {

	public class TODFilters {

		public struct Filters {
			public Boolean gradient;
		}

		public static MCvScalar BLACK = new Bgr(Color.Black).MCvScalar;
		public static MCvScalar WHITE = new Bgr(Color.White).MCvScalar;
		public static Mat MORPH_KERNEL = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new Size(2, 2), new Point(-1, -1));

		public static Filters filters;

		private static TODPath _path = new TODPath();

		public static new string ToString() {
			return string.Format("TODFilters{0}", "");
		}

		public static Image<Bgr, byte> ScaleToFit(Image<Bgr, byte> source, int Rows, out double scale) {
			scale = (float)Rows / (float)source.Rows;
			Image<Bgr, byte> scaled = source.Resize(scale, Inter.Linear);
			return scaled;
		}

		public static Image<Bgr, byte> GrayToBgr(Image<Gray, byte> source) {

			Image<Bgr, byte> bgrImage = new Image<Bgr, byte>(source.Cols, source.Rows);
			bgrImage.ConvertFrom(source);
			return bgrImage;
		}

		public static Image<Bgr, byte> EqualizeGray(Image<Bgr, byte> source) {
			Image<Hls, byte> filteredHls = source.Convert<Hls, byte>();
			Image<Gray, Byte> imageL = filteredHls[1];
			imageL._EqualizeHist();
			filteredHls[1] = imageL;

			return filteredHls.Convert<Bgr, Byte>();
		}

		public static void EqualizeClahe(Image<Bgr, byte> source) {
			ColorClahe(source);
		}

		public static byte Clamp(double b) {
			return b > 255.0 ? (byte)255 : (b < 0.0 ? (byte)0 : (byte)b);
		}

		public static byte Clamp(int b) {
			return b > 255f ? (byte)255 : (b < 0f ? (byte)0 : (byte)b);
		}

		public static void Clamp(ref TRect rect, int c, int r) {
			rect.x = rect.x >= c ? c - 1 : (rect.x < 0 ? 0 : rect.x);
			rect.y = rect.y >= r ? r - 1 : (rect.y < 0 ? 0 : rect.y);
			rect.w = rect.x + rect.w >= c ? c - rect.x - 1 : rect.w - 1;
			rect.h = rect.y + rect.h >= r ? r - rect.y - 1 : rect.h - 1;
		}

		public static Point MinMax(byte[,,] data, int channel, int cols, int rows, float drag, TRect roi) {
			float min = 255f, max = 0f;
			int xStart = (int)roi.x;
			int yStart = (int)roi.y;
			cols = (int)roi.w;
			rows = (int)roi.h;

			for (int y = yStart; y < rows; y++) {
				for (int x = xStart; x < cols; x++) {
				
					float b = (float)data[y, x, channel];
					if (b < min) min = min + (b - min) * drag;
					if (b > max) max = max + (b - max) * drag;
				}
			}

			bool adjusted = false;
			float tempMin = min;
			if (min > max) {
				adjusted = true;
				min = 0f;
			}
			if (max < tempMin) {
				adjusted = true;
				max = 255f;
			}
			Point minMax = new Point((int)min, (int)max);
			if(adjusted)
				Logger.Instance.SilentLog(string.Format("minMax adjusted = {0}, for rect {1}", minMax, roi));
			return minMax;
		}

		public static void SourceFilter(Image<Bgr, byte> image, TRect roi) {

			Image<Bgr, byte> filtered = new Image<Bgr, byte>(image.Cols, image.Rows);
			CvInvoke.FastNlMeansDenoisingColored(image, filtered, 3f, 10f, 7, 13);
			//CvInvoke.BilateralFilter(image, filtered, 9, 100, 100);
			// preview off Sketch.ShowProcessImage(filtered, "Mean shift");

			filtered.CopyTo(image);
		}

		private static double clipLimit = 2.5;
		private static Size tileGridSize = new Size(10, 10);
		private static Image<Bgr, byte> ColorClahe(Image<Bgr, byte> source) {

			Image<Lab, byte> labSource = source.Convert<Lab, byte>();
			Image<Gray, byte>[] lab = labSource.Split();
			CvInvoke.CLAHE(lab[0], clipLimit, tileGridSize, lab[0]);

			byte[, ,] data = lab[0].Data;
			byte[, ,] labData = labSource.Data;

			int w = source.Cols;
			int h = source.Rows;
			for (int y = 0; y < h; y++) {
				for (int x = 0; x < w; x++) {
					labData[y, x, 0] = data[y, x, 0];
				}
			}

			//labSource.Data = labData;

			Image<Bgr, byte> bgrSource = labSource.Convert<Bgr, byte>();
			return bgrSource;
		}

		public static Image<Bgr, byte> Clahe(Image<Bgr, byte> image, TRect rect, Image<Bgr, byte> claheMask, out Image<Gray, byte> edgesSource) {
			
			double clipLimit = 2.0;
			Size tileGridSize = new Size(40, 40);

			int w = image.Cols;
			int h = image.Rows;

			Image<Bgr, byte> eqImage = ColorClahe(image);

			Image<Gray, byte> grayFrame = eqImage.Convert<Gray, byte>();
			edgesSource = grayFrame;

			//grayFrame._EqualizeHist();
			Sketch.ShowProcessImage(grayFrame.Convert<Bgr, byte>(), "Equalized Histogram");
			Image<Gray, byte> sobelSource = grayFrame.SmoothBlur(8, 8);

			CvInvoke.CLAHE(grayFrame, clipLimit, tileGridSize, grayFrame);
			// preview off Sketch.ShowProcessImage(grayFrame.Convert<Bgr, byte>(), "Clahe");

			Image<Bgr, byte> coloredGF = grayFrame.Convert<Bgr, byte>();
			Sharpen(coloredGF, 6);
			// preview off Sketch.ShowProcessImage(coloredGF.Clone(), "Sharpened");

			Image<Bgr, byte> filtered = grayFrame.Convert<Bgr, byte>();
			byte[,,] filteredData = filtered.Data;
			if (filters.gradient) {
				Image<Bgr, byte> gradient = filtered;
				CvInvoke.MorphologyEx(255 - gradient, gradient, Emgu.CV.CvEnum.MorphOp.Gradient, MORPH_KERNEL, new Point(-1, -1), 4, Emgu.CV.CvEnum.BorderType.Constant, WHITE);
				gradient = 255 - gradient;
				filteredData = gradient.Data;
				Sketch.ShowProcessImage(gradient.Clone(), "Gradient");
			}

			//CvInvoke.Dilate(255 - grayFrame, grayFrame, MORPH_KERNEL, new Point(-1, -1), 3, BorderType.Constant, WHITE);
			//grayFrame = 255 - grayFrame;

			/*Image<Bgr, byte> eqGrayToColor = Equalize(coloredGF, rect);
			Show(eqGrayToColor.Clone(), "Eq Gray");*/

			byte[, ,] maskData = claheMask.Data;
			int maskW = claheMask.Cols, maskH = claheMask.Rows;
			//Show(claheMask, "mask");

			Image<Bgr, double> gfDouble = coloredGF.Convert<Bgr, double>();
			double[, ,] gfData = gfDouble.Data;

			double w_d = (double)w, h_d = (double)h;

			for (int y = 0; y < h; y++) {
				for (int x = 0; x < w; x++) {

					double claheMultiplier = (double)gfData[y, x, 0] / 255.0;
					claheMultiplier = Math.Pow(claheMultiplier, 1.0);

					double gradientMultiplier = (double)filteredData[y, x, 0] / 255.0;

					double maskMultiplier = ((x < maskW && y < maskH) ? ((double)maskData[y, x, 0] / 255.0) : 0) * .5; // TIMES POINT 5 !!!!!!!!!!!!!!
					double inverseMask = 1 - maskMultiplier;

					double min = Math.Min(claheMultiplier, gradientMultiplier);
					double max = Math.Max(claheMultiplier, gradientMultiplier);
					double vx = (w_d / 2.0 - (double)x) / (w_d / 3.0);
					double vy = (h_d / 2.0 - (double)y) / (h_d / 3.0);
					double t = Math.Sqrt(vx * vx + vy * vy);
					double inverseT = 1 - t;

					double result = claheMultiplier * maskMultiplier + gradientMultiplier * inverseMask;

					gfData[y, x, 0] = gfData[y, x, 1] = gfData[y, x, 2] = result * 255;
				}
			}

			Image<Bgr, byte> gfDoubleByte = gfDouble.Convert<Bgr, byte>();
			double blacks = CountBlack(gfDoubleByte.Convert<Gray, byte>());
			byte[, ,] multiplierData = gfDoubleByte.Data;

			// edges
			if (false) {
				Sketch.ShowProcessImage(sobelSource.Convert<Bgr, byte>(), "blur");
				Image<Gray, float> sobel = sobelSource.Sobel(0, 1, 3).Add(sobelSource.Sobel(1, 0, 3)).AbsDiff(new Gray(0.0));
				float[,,] sobelData = sobel.Data;
				Sketch.ShowProcessImage(sobel.Convert<Bgr, byte>(), "sobel");
			}
			// edges

			byte[, ,] data = image.Data;
			for (int y = 0; y < h; y++) {
				for (int x = 0; x < w; x++) {

					double cx = w_d / 2.0;
					double cy = h_d / 2.0;
					double nx = (double)x;
					double ny = (double)y;
					double vx = (cx - nx) / (w_d / 2.0);
					double vy = (cy - ny) / (h_d / 2.0);
					double t = Math.Sqrt(vx * vx + vy * vy);
					double tMul = Math.Max(0.0, t - .25) * 128.0;

					double multiplier = (double)multiplierData[y, x, 0] / 255;
					double offset = multiplier * 192;

					double val = Math.Max(0, Math.Min(255, 255.0 * multiplier + tMul));

					for (int iChannel = 0; iChannel < 3; iChannel++) {
						data[y, x, iChannel] = (byte)val;
					}
				}
			}

			image._GammaCorrect(.8);

			Sketch.ShowProcessImage(image.Clone(), String.Format("S x M (B = {0})", blacks.ToString("0.000")));

			Sharpen(image, 3);

			return image;// merge.Convert<Bgr, byte>();
		}
		
		#region edges

		private static List<OTP> FinalizeEdges(OTP iterator, OTP edgesEnd) {

			List<OTP> path = new List<OTP>(500);
			path.Add(new OTP(TP.PenUp));

			double hAngleMax = Math.PI / 5;
			
			OTP realPrevious = null;
			OTP previous = iterator;
			
			while (iterator != null && iterator.next != null) {
				iterator = iterator.next;

				if (AcceptSegment(previous.point, iterator.point, hAngleMax)) {


					if (realPrevious != null) {
						double vx = realPrevious.point.x - previous.point.x;
						double vy = realPrevious.point.y - previous.point.y;

						if (vx < 0.03 && vy < 0.004) {
							//path.Add(realPrevious.point);
							//path.Add(previous.point);
						}
						else {
							path.Add(new OTP(TP.PenUp));
						}
					}

					path.Add(previous);
					path.Add(iterator);

					realPrevious = iterator;
				}
				else {
					//previous.point.IsNull = true;
					//iterator.point.IsNull = true;
					//path.Add(previous.point);
					//path.Add(iterator.point);
				}


				iterator = iterator.next;
				previous = iterator;
			}

			return path;
		}

		private static int SortY(LineSegment2D s1, LineSegment2D s2) {
			float yVal1 = (float)(s1.P1.Y) * 4f;
			yVal1 += (float)(s1.P1.X);
			float yVal2 = (float)(s2.P1.Y) * 4f;
			yVal2 += (float)(s2.P1.X);
			if (yVal1 > yVal2) return 1;
			else if (yVal1 < yVal2) return -1;
			return 0;
		}

		private static bool AcceptSegment(TP p1, TP p2, double hAngleMax) {
			double vx = p1.x - p2.x, vy = p1.y - p2.y;
			double a = Math.Atan2(vy, vx);
			if ((a < Math.PI / 2 + hAngleMax && a > Math.PI / 2 - hAngleMax)
				|| (a < -Math.PI / 2 + hAngleMax && a > -Math.PI / 2 - hAngleMax)
				) {
				return false;
			}

			return true;
		}

		public static List<OTP> HoughEdges(Image<Gray, byte> image) {

			float w = (float)image.Cols, h = (float)image.Rows;
			List<OTP> edges = new List<OTP>();
			Image<Gray, byte> source = image.Clone();
			source.SmoothBlur(2, 2);
			//PyrMeanShiftFiltering(SPATIAL_RADIUS = 8, COLOR_RADIUS = 60, PYRAMID_LEVEL = 1, new MCvTermCriteria(MAX_ITERATIONS, 1));
			Image<Bgr, byte> coloredSource = source.Convert<Bgr, byte>();
			CvInvoke.PyrMeanShiftFiltering(coloredSource, coloredSource, 8, 60, 1, new MCvTermCriteria(1, 1));
			//HoughLinesBinary(HL_DISTANCE_RES = 1, Math.PI / 90, HL_VOTE_THRESH = 10, HL_MIN_LENGTH = 0, HL_MAX_GAP = 2)
			//source.HoughLinesBinary(rhoResolution, thetaResolution, threshold, minLineWidth, gapBetweenLines);
			source = coloredSource.Convert<Gray, byte>();
			LineSegment2D[][] lines = source.HoughLines(30.0, 60.0, 1.0, Math.PI / 60, 7, 5, 4);

			List<LineSegment2D> sortedLines = lines[0].ToList<LineSegment2D>();
			sortedLines.Sort(SortY);

			Random random = new Random();
			OTP start = null;
			OTP iterator = null;
			for (int i = 0; i < sortedLines.Count; i++) {
				LineSegment2D line = sortedLines[i];

				float p1x = (float)line.P1.X / w;
				float p1y = (float)line.P1.Y / h;

				double vx = (double)(.5f - p1x);
				double vy = (double)(.5f - p1y);
				double d = vx * vx + vy * vy;
				double threshold = 0.32 + random.NextDouble() * 0.15;
				threshold *= threshold;
				if (d < threshold) {

					float p2x = (float)line.P2.X / w;
					float p2y = (float)line.P2.Y / h;

					OTP n1 = new OTP(new TP(p1x, p1y));
					OTP n2 = new OTP(new TP(p2x, p2y));
					if (iterator != null) iterator.next = n1;
					iterator = n1;
					iterator.next = n2;
					if (start == null) start = iterator;
					iterator = n2;
				}
			}

			OTP edgesEnd = iterator;
			return FinalizeEdges(start, edgesEnd);
		}

		#endregion

		private static double CountBlack(Image<Gray, byte> image, int offset = 50) {

			double black = 0;
			int w = image.Cols - offset * 2;
			int h = image.Rows - offset * 2;

			byte[, ,] data = image.Data;
			for (int y = offset; y < h; y++) {
				for (int x = offset; x < w; x++) {
					black += 255.0 - (double)data[y, x, 0] / 255.0;
				}
			}

			return black;
		}

		public static void Sharpen(Image<Bgr, byte> image, double bSize = 6) {
			//cv::GaussianBlur(frame, image, cv::Size(1, 1), 3);
			//cv::addWeighted(frame, 1.5, image, -0.5, 0, image);

			int w = image.Cols, h = image.Rows;

			Image<Bgr, byte> imageClone = image.Clone();
			Image<Bgr, byte> blurred = new Image<Bgr, byte>(w, h);
			CvInvoke.GaussianBlur(imageClone, blurred, new Size(0, 0), bSize);
			CvInvoke.AddWeighted(imageClone, 1.5, blurred, -0.5, 0, image);
			//Show(blurred, "blur");
			//Show(image.Clone(), "sharpened");
		}


		public static Image<Bgr, byte> Equalize(Image<Bgr, byte> source, TRect roi) {

			return source.Clone();
			Clamp(ref roi, source.Cols, source.Rows);

			Image<Hsv, byte> filteredHsv = source.Convert<Hsv, byte>();
			byte[, ,] data = filteredHsv.Data;
			int numPixels = data.Length;
			int cols = filteredHsv.Cols;
			int rows = filteredHsv.Rows;
			int channel = 2;// (int)Form1.self.minMaxChannel.Value;
			float minMaxDrag = 0.1f;// (float)Form1.self.minMaxDrag.Value;
			Point minMax = MinMax(data, channel, cols, rows, minMaxDrag, roi);
			
			int min = minMax.X;
			int max = minMax.Y;
			int newMin = 0;// (int)Form1.self.minVal.Value;
			int newMax = 255;// (int)Form1.self.maxVal.Value;
			float range = (float)(newMax - newMin);
			float dRatio = range / (float)(max - min);

			Logger.Instance.SilentLog("Changing roi({0}) range \n\tfrom [{1}, {2}] to [{3}, {4}] using dRatio({5})", 
				roi.ToString(), min.ToString(), max.ToString(), newMin.ToString(), newMax.ToString(), dRatio.ToString());

			for (int y = 0; y < rows; y++) {
				for (int x = 0; x < cols; x++) {
					byte v = data[y, x, channel];
					int normalized = (int)(newMin + (v - min) * dRatio);
					//int normalized = (int)((v - min) * 255 / range);
					data[y, x, channel] = Clamp(normalized);
				}
			}
			
			//filteredHsv.Data = data;
			//CvInvoke.Rectangle(filteredHsv, roi.ToRectangle(), new Bgr(Color.GreenYellow).MCvScalar, 1);

			return filteredHsv.Convert<Bgr, Byte>();
		}

	}
}
