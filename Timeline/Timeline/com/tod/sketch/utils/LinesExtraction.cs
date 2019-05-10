using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch {

	public class LinesExtraction {

		public class HoughParameters {

			/// <summary>The threshhold to find initial segments of strong edges</summary>
			public double cannyThreshold = 30.0;
			/// <summary>The threshold used for edge Linking</summary>
			public double cannyThresholdLinking = 60.0;
			/// <summary>Distance resolution in pixel-related units</summary>
			public double rhoResolution = 1.0;
			/// <summary>Angle resolution measured in radians</summary>
			public double thetaResolution = Math.PI / 60.0;
			/// <summary>Minimum width of a line</summary>
			public double minLineWidth = 5.0;
			/// <summary>Minimum gap between lines</summary>
			public double gapBetweenLines = 4.0;
			/// <summary>A line is returned by the function if the corresponding accumulator value is greater than threshold</summary>
			public int threshold = 7;

			public int blurSize = 2;
			public double spatialRadius = 2, colorRadius = 60;
			public int maxLevel = 4;
			public int termIterations = 3;
			public double termEpsilon = 1;

			public double addWeightAlpha = 1.5, addWeightBeta = -.5, addWeightScalar = 0;
		}

		public class EdgesParameters {

			public int sobelOrder = 1, sobelAperture = 3, threshold = 127, erosion = 1;
		}

		public static List<List<Point>> Sobel(Image<Bgr, byte> source, EdgesParameters ep, HoughParameters hp, out Image<Bgr, byte> filtered) {

			Image<Gray, byte> gray = source.Convert<Gray, byte>();
			gray._EqualizeHist();
			Image<Gray, float> sobel = gray.Sobel(0, ep.sobelOrder, ep.sobelAperture).Add(gray.Sobel(ep.sobelOrder, 0, ep.sobelAperture)).AbsDiff(new Gray(0.0));
			sobel = sobel.SmoothBlur(hp.blurSize, hp.blurSize);

			gray = sobel.Convert<Gray, byte>();
			gray._EqualizeHist();
			gray._ThresholdBinary(new Gray(ep.threshold), new Gray(255));
			gray._Erode(ep.erosion);
			filtered = gray.Convert<Bgr, byte>();

			LineSegment2D[][] houghLines = gray.HoughLinesBinary(hp.rhoResolution, hp.thetaResolution, hp.threshold, hp.minLineWidth, hp.gapBetweenLines);
			//LineSegment2D[][] houghLines = new LineSegment2D[][] { CvInvoke.HoughLinesP(sobel, hp.rhoResolution, hp.thetaResolution, hp.threshold, hp.minLineWidth, hp.gapBetweenLines) };

			return FilterHoughResult(houghLines, 600);
		}

		public static List<List<Point>> Hough(Image<Bgr, byte> source, HoughParameters parameters, out Image<Bgr, byte> filtered) {

			source._EqualizeHist();
			Image<Bgr, byte> meanShifted = new Image<Bgr, byte>(source.Size);
			CvInvoke.PyrMeanShiftFiltering(source, meanShifted, parameters.spatialRadius, parameters.colorRadius, parameters.maxLevel, new MCvTermCriteria(parameters.termIterations, parameters.termEpsilon));
			filtered = meanShifted.SmoothBlur(parameters.blurSize, parameters.blurSize);

			LineSegment2D[][] houghLines = filtered.HoughLines(parameters.cannyThreshold, parameters.cannyThresholdLinking, parameters.rhoResolution, parameters.thetaResolution, parameters.threshold, parameters.minLineWidth, parameters.gapBetweenLines);

			return FilterHoughResult(houghLines);
		}

		private static List<List<Point>> FilterHoughResult(LineSegment2D[][] houghLines, double linkDistance = 120) {

			List<LineSegment2D> segments = new List<LineSegment2D>();
			foreach (LineSegment2D[] list in houghLines)
				segments.AddRange(list);

			List<Point> line = new List<Point>();
			List<List<Point>> lines = new List<List<Point>> { line };
			for (int i = 0; i < segments.Count; i++) {
				LineSegment2D segment = segments[i];
				line.Add(segment.P1);

				int nearestIndex = -1;
				double nearestDistance = double.MaxValue;
				for (int j = i + 1; j < segments.Count; j++) {
					LineSegment2D other = segments[j];
					double distance = (other.P1.X - segment.P2.X) * (other.P1.X - segment.P2.X) + (other.P1.Y - segment.P2.Y) * (other.P1.Y - segment.P2.Y);
					if (distance < nearestDistance) {
						nearestDistance = distance;
						nearestIndex = j;
					}
					else {
						continue;
						distance = (other.P2.X - segment.P2.X) * (other.P2.X - segment.P2.X) + (other.P2.Y - segment.P2.Y) * (other.P2.Y - segment.P2.Y);
						if (distance < nearestDistance) {
							nearestDistance = distance;
							nearestIndex = j;
						}
					}
				}

				if(nearestDistance < 120) {
					LineSegment2D nearest = segments[nearestIndex];
					//nearest.P1 = line[line.Count - 1];
					segments.RemoveAt(nearestIndex);
					segments[i--] = nearest;
				}
				else {
					line.Add(segment.P2);
					line = new List<Point>();
					lines.Add(line);
				}
			}

			return lines;
		}

		public static void Visualize(List<List<Point>> lines, Image<Bgr, byte> image, int thickness) {
			Random rand = new Random();
			foreach (List<Point> points in lines) {
				int numPoints = points.Count;
				if (numPoints > 1) {
					MCvScalar lineColor = new MCvScalar(64 + 192 * rand.NextDouble(), 64 + 192 * rand.NextDouble(), 64 + 192 * rand.NextDouble());
					for (int i = 1; i < numPoints; i++)
						CvInvoke.Line(image, points[i - 1], points[i], lineColor, thickness);
				}
			}
		}
	}
}
