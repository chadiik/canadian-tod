using com.tod.core;
using com.tod.sketch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.canvas {

	public class Canvas {

		public int x, y, width, height;

		public Canvas() {

		}

		public Canvas(int x, int y, int width, int height) {
			Set(x, y, width, height);
		}

		public void Set(int x, int y, int width, int height) {
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public virtual List<Line> ToCell(List<Line> path, int cell = 0) {

			return path;
		}
	}
}
