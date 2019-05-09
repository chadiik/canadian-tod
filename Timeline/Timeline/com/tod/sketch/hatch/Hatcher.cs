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

		public event ProcessComplete ProcessCompleted;

		public Hatcher(Threshold[] thresholds, Image<Gray, byte> regionsMap, Image<Bgr, byte> source) {

			this.thresholds = thresholds;
			this.regionsMap = regionsMap;

			LineSegment2D[][] lines = LinesExtraction.From(source, new LinesExtraction.HoughParameters());

			Image<Bgr, byte> hatchesPreview = new Image<Bgr, byte>(regionsMap.Size);
			Image<Bgr, byte> pathPreview = new Image<Bgr, byte>(regionsMap.Size);
			int numThresholds = thresholds.Length;
			int thresholdsProcessed = 0;
			Action callback = () => {
				if (++thresholdsProcessed == numThresholds) {
					Sketch.ShowProcessImage(hatchesPreview, "Hatches");
					Sketch.ShowProcessImage(pathPreview, "Path");
					ProcessCompleted?.Invoke();
				}
			};

			foreach(Threshold threshold in thresholds) {
				//(new Thread(() => {
					HatchRegion hatchRegion = new HatchRegion();

					hatchRegion.Process(threshold, regionsMap);
					Segment.Visualize(hatchRegion.hatches, hatchesPreview, new MCvScalar(255, 255, 255), 1, false);
					//Segment.Visualize(hatchRegion.hatches, pathPreview, new MCvScalar(255, 100, 100), 1, false);

					hatchRegion.Link(threshold, regionsMap);
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
