using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch.hatch {

	public class HatchRegion {

		public List<Segment> hatches;
		public List<TP> path;

		public void Process(Threshold threshold, Image<Gray, byte> map) {

			HatchProbe probe = new HatchProbe(threshold.Angle, threshold.Spread, new Rectangle(0, 0, map.Width, map.Height));

			hatches = new List<Segment>();
			byte[,,] data = map.Data;
			int brightness = threshold.Brightness;
			Rectangle mapRect = new Rectangle(4, 4, map.Width - 8, map.Height - 8);

			Segment segment = null;
			while (probe.Next()) {
				while (probe.Advance(2.0)) {
					if (data[probe.Y, probe.X, 0] == brightness) {
						if (segment == null) {
							segment = new Segment { a = new Point(probe.X, probe.Y) };
						}
					}
					else if (segment != null) {
						segment.b = new Point(probe.X, probe.Y);
						if (mapRect.Contains(segment.a) && mapRect.Contains(segment.b))
							hatches.Add(segment);
						segment = null;
					}
				}

				if (segment != null) {
					segment.b = new Point(probe.X, probe.Y);
					segment = null;
				}
			}
		}

		public void Link(Threshold threshold, Image<Gray, byte> map) {

			byte[,,] data = map.Data;
			int brightness = threshold.Brightness;

			PointF directionStart = new PointF((float)Math.Cos(threshold.Angle + Math.PI), -(float)Math.Sin(threshold.Angle + Math.PI)),
				directionEnd = new PointF((float)Math.Cos(threshold.Angle), -(float)Math.Sin(threshold.Angle));

			path = new List<TP> { TP.PenUp };
			bool first = true;
			Segment hatch = hatches.Count > 0 ? hatches[0] : null;
			while (hatch != null) {
				hatches.Remove(hatch);

				Point start, end;
				if (first) { start = hatch.a; end = hatch.b; }
				else { start = hatch.b; end = hatch.a; }

				if (path.Count > 0) {
					Point previous = new Point((int)path[path.Count - 1].x, (int)path[path.Count - 1].y);
					if(!IsWithinRegion(start, previous, 4, brightness, data, map.Cols, map.Rows))
						path.Add(TP.PenUp);
				}

				path.Add(new TP(start.X, start.Y));

				if (hatches.Count == 0) {
					path.Add(TP.PenUp);
					path.Add(TP.PenUp);
					return;
				}

				Point nearest;
				hatch = FindNearest(end, !first, brightness, data, out nearest);

				first = !first;
			}

			path.Add(TP.PenUp);
		}

		private double Dot(PointF a, PointF b) {
			double da = Math.Sqrt(a.X * a.X + a.Y * a.Y);
			double db = Math.Sqrt(b.X * b.X + b.Y * b.Y);
			return (a.X / da) * (b.Y / db) - (a.Y / da) * (b.X / db);
		}

		private Segment FindNearest(Point p, bool first, int brightness, byte[,,] data, out Point nearest) {

			Segment segment = null;
			double d = double.MaxValue;
			foreach(Segment hatch in hatches) {
				Point cp = first ? hatch.a : hatch.b;
				double cpd = (cp.X - p.X) * (cp.X - p.X) + (cp.Y - p.Y) * (cp.Y - p.Y);
				if(cpd < d) {
					d = cpd;
					segment = hatch;
				}
			}

			if(segment == null) {
				nearest = default(Point);
				return null;
			}

			nearest = first ? segment.a : segment.b;
			return segment;
		}

		private static bool IsWithinRegion(Point a, Point b, int subdivisions, int brightness, byte[,,] data, int width, int height) {

			int maxW = width - 1,
				maxH = height - 1;
			PointF aToB = new PointF(b.X - a.X, b.Y - a.Y);
			PointF aToBSubdivision = new PointF(aToB.X / subdivisions, aToB.Y / subdivisions);
			for (int s = 1; s < subdivisions; s++) {
				int x = (int)(a.X + aToBSubdivision.X * s);
				int y = (int)(a.Y + aToBSubdivision.Y * s);
				if (data[Math.Min(maxH, Math.Max(0, y)), Math.Min(maxW, Math.Max(0, x)), 0] != brightness)
					return false;
			}

			return true;
		}
	}
}
