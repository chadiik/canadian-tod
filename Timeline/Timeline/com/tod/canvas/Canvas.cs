using com.tod.sketch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.canvas {

	public class Canvas {

		public int x, y, width, height;

		public Canvas(int x, int y, int width, int height) { // mm
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public virtual List<TP> ToCell(List<TP> path) {

			List<TP> result = new List<TP>();
			int pathLength = path.Count;
			for(int i = 0; i < pathLength; i++) {
				TP p = path[i];
				result.Add(p.IsDown ? new TP(x + p.x * width, y + p.y * width, p.IsNull) : new TP(p.x, p.y, p.IsNull));
			}

			return result;
		}
	}
}
