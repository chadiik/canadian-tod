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

	public class Hatcher {

		public delegate void ProcessComplete();

		public Threshold[] thresholds;
		public Image<Gray, byte> regionsMap;
		public List<TP> path;

		public event ProcessComplete ProcessCompleted;

		public Hatcher(Threshold[] thresholds, Image<Gray, byte> regionsMap) {

			this.thresholds = thresholds;
			this.regionsMap = regionsMap;
		}

		public void Process(Image<Bgr, byte> source, List<Contour> firstContours = null) {

			Image<Bgr, byte> filtered;
			LinesExtraction.HoughParameters hp;
			if (!JSON.Load("hough-edges", out hp)) hp = new LinesExtraction.HoughParameters();
			LinesExtraction.EdgesParameters ep;
			if (!JSON.Load("edges", out ep)) ep = new LinesExtraction.EdgesParameters();
			List<List<Point>> lines = LinesExtraction.Sobel(source, ep, hp, out filtered);

			byte[,,] regionsData = regionsMap.Data;
			foreach (List<Point> points in lines) {
				for (int i = 0; i < points.Count; i++) {
					Point point = points[i];
					if (regionsData[point.Y, point.X, 0] == 255)
						points.RemoveAt(i--);
				}
			}

			path = new List<TP> { TP.PenUp };
			List<HatchLine> mergedPath = new List<HatchLine>();

			Action addFirstContour = () => {
				if (firstContours != null) {
					foreach (Contour contour in firstContours) {
						path.AddRange(contour.ToPath());
						mergedPath.Add(new HatchLine(contour.points));
					}
				}
			};

			Action addHoughLines = () => {
				foreach (List<Point> points in lines) {
					int numPoints = points.Count;
					if (numPoints > 1) {
						path.Add(new TP(points[0].X, points[0].Y));
						for (int i = 1; i < numPoints; i++)
							path.Add(new TP(points[i].X, points[i].Y));
					}
					path.Add(TP.PenUp);
					mergedPath.Add(new HatchLine(points));
				}
			};

			Image<Bgr, byte> linesPreview = new Image<Bgr, byte>(source.Size);
			LinesExtraction.Visualize(lines, linesPreview, 2);
			Sketch.ShowProcessImage(linesPreview, string.Format("HoughLines x {0}", lines.Count));

			Image<Bgr, byte> hatchesPreview = new Image<Bgr, byte>(regionsMap.Size);
			Image<Bgr, byte> pathPreview = new Image<Bgr, byte>(regionsMap.Size);
			int numThresholds = thresholds.Length - 2;
			int thresholdsProcessed = 0;
			Comparison<HatchLine> mergedPathSort = HatchLine.DistanceSort(new Point(regionsMap.Width / 2, regionsMap.Height / 2));
			Action callback = () => {
				thresholdsProcessed++;
				Logger.Instance.WriteLog("Processed {0}/{1} thresholds", thresholdsProcessed, numThresholds);
				if (thresholdsProcessed == numThresholds) {
					//Sketch.ShowProcessImage(hatchesPreview, "Hatches");
					//Sketch.ShowProcessImage(pathPreview, "Path");

					HatchLine.Sanitize(mergedPath);
					mergedPath.Sort(mergedPathSort);
					path = HatchLine.Link(mergedPath, 60);

					ProcessCompleted?.Invoke();
				}
			};

			//addHoughLines();
			for (int i = 0; i < numThresholds; i++) {
				Threshold threshold = thresholds[i];
				//(new Thread(() => {
					HatchRegion hatchRegion = new HatchRegion();

					hatchRegion.Process(threshold, regionsMap);
					Segment.Visualize(hatchRegion.hatches, hatchesPreview, new MCvScalar(255, 255, 255), 1, false);
					//Segment.Visualize(hatchRegion.hatches, pathPreview, new MCvScalar(255, 100, 100), 1, false);

					hatchRegion.Link(threshold, regionsMap);
					path.AddRange(hatchRegion.path);
					mergedPath.AddRange(hatchRegion.lines);

					bool isFirstThreshold = i == 0;
					if (isFirstThreshold) {
						addFirstContour();
						addHoughLines();
					}
					TP.Visualize(hatchRegion.path, pathPreview, new MCvScalar(255, 255, 255), 1);

					callback.Invoke();
				//})).Start();
			}
		}

		private Threshold GetThreshold(int x, int y) {
			byte brightness = regionsMap.Data[y, x, 0];
			foreach (Threshold threshold in thresholds)
				if (brightness == threshold.Brightness)
					return threshold;

			return null;
		}

		public static Comparison<Contour> DistanceSort(Point p) {
			return (Contour a, Contour b) => {
				double da = a.DistanceSquaredTo(p);
				double db = b.DistanceSquaredTo(p);

				if (da < db) return -1;
				else if (da > db) return 1;
				return 0;
			};
		}
	}
}
