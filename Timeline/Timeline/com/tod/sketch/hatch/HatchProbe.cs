using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch.hatch {

	public class HatchProbe {

		/// <summary>Absolute coordinate</summary>
		public double x, y;

		/// <summary>Normalized downward direction component</summary>
		public double nx, ny;

		private double m_XOffset;
		private double m_Spread;
		private Rectangle m_region;

		public int X { get { return (int)x; } }
		public int Y { get { return (int)y; } }

		public HatchProbe(double angle, double spread, Rectangle region) {
			nx = Math.Cos(angle + Math.PI);
			ny = -Math.Sin(angle + Math.PI);

			m_XOffset = -spread / 2.0;
			m_Spread = spread;
			m_region = region;
		}

		public bool Advance(double distance) {
			x += nx * distance;
			y += ny * distance;

			return x > m_region.X && y > m_region.Y && y < m_region.Bottom && x < m_region.Right;
		}

		public bool Next() {
			x = m_XOffset += m_Spread;
			y = -.0001;

			double intersectionCheckLength = m_region.Height * 2;

			Point a = new Point(m_region.X, m_region.Y),
				b = new Point(m_region.X + m_region.Width, m_region.Y),
				c = new Point(m_region.X + m_region.Width, m_region.Y + m_region.Height),
				start = new Point((int)x, (int)y),
				end = new Point((int)(x + nx * intersectionCheckLength), (int)(y + ny * intersectionCheckLength)),
				intersection;

			bool isInRange = 
				chadiik.geom.PathUtils.SegmentIntersect(start, end, a, b, out intersection)
				|| chadiik.geom.PathUtils.SegmentIntersect(start, end, b, c, out intersection);

			x = intersection.X;
			y = intersection.Y;

			return isInRange;
		}
	}
}
