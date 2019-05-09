using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch.zigzag {

	public class Probe {

		/// <summary>Normalized upward direction component</summary>
		public double nux, nuy;

		/// <summary>Normalized downward direction component</summary>
		public double ndx, ndy;

		/// <summary>Absolute coordinate</summary>
		public double x, y;

		public int X { get { return (int)x; } }
		public int Y { get { return (int)y; } }

		public Point Point { get { return new Point((int)x, (int)y); } }

		public Probe(double angle, double angularSpread) {
			nux = Math.Cos(angle);
			nuy = -Math.Sin(angle);

			ndx = Math.Cos(angle + Math.PI + angularSpread);
			ndy = -Math.Sin(angle + Math.PI + angularSpread);

			Logger.Instance.WriteLog("New probe: {0}/{1}", (angle * 180.0 / Math.PI).ToString("0."), (angularSpread * 180.0 / Math.PI).ToString("0."));
			Logger.Instance.WriteLog("upward: {0}, {1}", nux.ToString(".00"), nuy.ToString(".00"));
			Logger.Instance.WriteLog("downward: {0}, {1}", ndx.ToString(".00"), ndy.ToString(".00"));
		}

		public void Advance(double direction) {
			double d = Math.Abs(direction);
			if(direction < 0) {
				x += ndx * d;
				y += ndy * d;
			}
			else {
				x += nux * d;
				y += nuy * d;
			}
		}
	}
}
