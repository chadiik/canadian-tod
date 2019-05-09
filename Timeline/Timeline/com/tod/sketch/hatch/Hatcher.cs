using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch.hatch {

	public class Hatcher {

		public List<Contour> contours;
		public Threshold[] thresholds;
		public Image<Gray, byte> regionsMap;

		public Hatcher(List<Contour> contours, Threshold[] thresholds, Image<Gray, byte> regionsMap) {

			this.contours = contours;
			this.thresholds = thresholds;
			this.regionsMap = regionsMap;

			//this.contours.Sort(DistanceSort(new Point(this.regionsMap.Width / 2, this.regionsMap.Height / 2)));
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
				throw new Exception("Not implemented");
				/*
				double da = a.Head.DistanceSquaredTo(p);
				double db = b.Head.DistanceSquaredTo(p);

				if (da < db) return -1;
				else if (da > db) return 1;
				return 0;*/
			};
		}
	}
}
