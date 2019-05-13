using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch.hatch {

	public class HatchLine {

		public List<Point> path;

		private Point m_DistanceTarget;
		private double m_DistanceCache = -1;

		public int Length {
			get { return path.Count; }
		}

		public Point First {
			get {
				return path.Count > 0 ? path[0] : default(Point);
			}
		}

		public Point Last {
			get {
				return path.Count > 0 ? path[path.Count - 1] : default(Point);
			}
		}

		public HatchLine(List<Point> path = null) {
			this.path = path != null ? path : new List<Point>();
		}

		public void Add(Point point) {
			path.Add(point);
		}

		public void Clear() {
			path.Clear();
		}

		public double Orient(Point p) {
			double vx = path[path.Count - 1].X - p.X;
			double vy = path[path.Count - 1].Y - p.Y;
			double distanceEnd = vx * vx + vy * vy;

			vx = path[0].X - p.X;
			vy = path[0].Y - p.Y;
			double distanceStart = vx * vx + vy * vy;

			if (distanceStart < distanceEnd)
				path.Reverse();

			return Math.Min(distanceStart, distanceEnd);
		}

		public double CalculateDistanceSqTo(Point p) {
			if (m_DistanceTarget != p) {

				double vx = path[path.Count - 1].X - p.X;
				double vy = path[path.Count - 1].Y - p.Y;
				double distanceEnd = vx * vx + vy * vy;

				vx = path[0].X - p.X;
				vy = path[0].Y - p.Y;
				double distanceStart = vx * vx + vy * vy;

				m_DistanceCache = Math.Min(distanceStart, distanceEnd);
				m_DistanceTarget = p;
			}

			return m_DistanceCache;
		}

		public static Comparison<HatchLine> DistanceSort(Point p) {
			return (HatchLine a, HatchLine b) => {
				double da = a.CalculateDistanceSqTo(p);
				double db = b.CalculateDistanceSqTo(p);

				if (da < db) return -1;
				else if (da > db) return 1;
				return 0;
			};
		}

		public static void Sanitize(List<HatchLine> lines) {
			for(int i = 0; i < lines.Count; i++) {
				if (lines[i].Length < 2)
					lines.RemoveAt(i--);
			}
		}

		public static List<TP> Link(List<HatchLine> lines, double breakDistance) {
			List<TP> path = new List<TP>();

			while (lines.Count > 0) {
				HatchLine line = lines[0];
				foreach (Point p in line.path) path.Add(new TP(p.X, p.Y));
				lines.Sort(DistanceSort(line.Last));
				if (line.Orient(line.Last) > breakDistance) path.Add(TP.PenUp);
				lines.RemoveAt(0);
			}

			return path;
		}
	}
}
