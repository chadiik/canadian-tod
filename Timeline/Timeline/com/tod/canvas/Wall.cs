using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.canvas {

	class Wall : Canvas {

		// xRatio = 0.7272
		private List<Point> _cellsCoordinates;
		private List<RectangleF> _cells;

		public List<RectangleF> wallCells;

		public List<RectangleF> UnitCells {
			get {
				return _cells;
			}
		}

		public Wall(int x, int y, int width, int height, int columns, int rows) : base(x, y, width, height) {

			Setup(columns, rows);
		}

		private void Setup(int columns, int rows) {
			#region spiral
			// by Can Berk Güder (http://stackoverflow.com/a/398302)

			int x, y, dx, dy;
			x = y = dx = 0;
			dy = -1;
			int t = Math.Max(columns, rows);
			int maxI = t * t;

			_cellsCoordinates = new List<Point>(columns * rows);

			for (int i = 0; i < maxI; i++) {
				if ((-columns / 2 <= x) && (x <= columns / 2) && (-rows / 2 <= y) && (y <= rows / 2)) {
					_cellsCoordinates.Add(new Point(x, y));
				}
				if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y))) {
					t = dx;
					dx = -dy;
					dy = t;
				}
				x += dx;
				y += dy;
			}
			#endregion

			int offsetX = columns / 2 - 1;
			int offsetY = rows / 2 - 0;
			float wf = (float)columns;
			float hf = (float)rows + (1 - rows % 2);
			float cw = 1f / wf;
			float ch = 1f / hf;

			int numCells = _cellsCoordinates.Count;
			_cells = new List<RectangleF>(numCells);

			for (int i = 0; i < numCells; i++) {

				float xf = (float)(_cellsCoordinates[i].X + offsetX);
				float yf = (float)(_cellsCoordinates[i].Y + offsetY);

				float zigzagX = cw * .5f * (float)(_cellsCoordinates[i].Y % 2);
				_cells.Add(new RectangleF(
					xf / wf + zigzagX, yf / hf, cw, ch
					));
			}
		}

		public void Test(int index) {
			Point cellCoord = _cellsCoordinates[index];
			Logger.Instance.WriteLog("{0}\t=> {1}, {2}", index.ToString(), cellCoord.X.ToString(), cellCoord.Y.ToString());
			Logger.Instance.WriteLog("\t=> {0}", _cells[index].ToString());
		}

		private static Point CellCoordinates(int tileNum) {
			// by Jonathan M (http://stackoverflow.com/a/20591835)
			int intRoot = (int)(Math.Sqrt(tileNum));

			int x = (int)((Math.Round(intRoot / 2.0) * Math.Pow(-1, (intRoot + 1))) + (Math.Pow(-1, (intRoot + 1)) * (((intRoot * (intRoot + 1)) - tileNum) - Math.Abs((intRoot * (intRoot + 1)) - tileNum)) / 2));
			int y = (int)((Math.Round(intRoot / 2.0) * Math.Pow(-1, intRoot)) + (Math.Pow(-1, (intRoot + 1)) * (((intRoot * (intRoot + 1)) - tileNum) + Math.Abs((intRoot * (intRoot + 1)) - tileNum)) / 2));

			return new Point(x, y);
		}

	}
}
