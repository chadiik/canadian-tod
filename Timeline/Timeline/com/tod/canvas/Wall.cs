using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.canvas {

	public class Wall : Canvas {

		public class Parameters {
			public int width = 2000, height = 1000, cells = 3;
		}

		public static RectangleF PORTRAIT = new RectangleF(0, 0, 363, 500);

		// xRatio = 0.726
		private List<Point> _cellsCoordinates;
		private List<RectangleF> _cells;

		public List<RectangleF> UnitCells {
			get {
				return _cells;
			}
		}

		public Wall(Parameters parameters) {
			Set(0, 0, parameters.width, parameters.height);
			Setup(parameters.cells);
		}

		private void Setup(int cells) {

			float scale = 0;
			int columns = 1000, rows = 1000;


			Action<float> test = offset => {
				scale += offset;
				columns = (int)(width / (PORTRAIT.Width * scale));
				rows = (int)(height / (PORTRAIT.Height * scale));
			};

			while (columns * rows >= cells) {
				test(.05f);
			}
			test(-.05f);

			Console.WriteLine("columns={0}, rows={1}, scale={2}", columns, rows, scale.ToString(".00"));

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

			int offsetX = (int)(columns / 2.0) - 0;
			int offsetY = (int)(rows / 2.0) - 0;
			float wf = columns;
			float hf = (float)rows + (1 - rows % 2);
			float cw = 1f / wf;
			float ch = 1f / hf;

			int numCells = _cellsCoordinates.Count;
			_cells = new List<RectangleF>(numCells);
			RectangleF canvas = new RectangleF(-.01f, -.01f, 1.02f, 1.02f);
			for (int i = 0; i < numCells; i++) {

				float xf = _cellsCoordinates[i].X + offsetX;
				float yf = _cellsCoordinates[i].Y + offsetY;

				float zigzagX = cw * .5f * (_cellsCoordinates[i].Y % 2);
				RectangleF rect = new RectangleF(xf / wf + zigzagX, yf / hf, cw, ch);
				if (canvas.Contains(rect))
					_cells.Add(rect);
			}
		}

		public Image<Bgr, byte> Visualize() {

			Image<Bgr, byte> image = new Image<Bgr, byte>(1200, 800);

			double sx = (double)(image.Size.Width - 200) / width;
			double sy = (double)(image.Size.Height - 200) / height;
			double scale = scale = Math.Min(sx, sy);

			int w = (int)(width * scale), 
				h = (int)(height * scale);
			int x = (image.Size.Width - w) / 2,
				y = (image.Size.Height - h) / 2;
			CvInvoke.Rectangle(image, new Rectangle(x, y, w, h), new MCvScalar(255, 255, 255), -1);

			Random rand = new Random(_cells.Count);
			MCvScalar black = new MCvScalar(0);
			for(int i = 0; i < _cells.Count; i++) {
				MCvScalar color = new MCvScalar(64 + 192 * rand.NextDouble(), 64 + 192 * rand.NextDouble(), 64 + 192 * rand.NextDouble());

				RectangleF r = _cells[i];
				r.X = (float)Math.Floor(r.X * width);
				r.Y = (float)Math.Floor(r.Y * height);
				r.Width = (float)Math.Floor(r.Width * width);
				r.Height = (float)Math.Floor(r.Height * height);

				Console.WriteLine("rect {0}: {1}", i, r);

				Rectangle rect = ToIntRect(r, scale);
				rect.X += x;
				rect.Y += y;

				CvInvoke.Rectangle(image, rect, color, -1);
				CvInvoke.PutText(image, (i + 1).ToString(), new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2), Emgu.CV.CvEnum.FontFace.HersheyPlain, 2, black, 2);
			}

			return image;
		}

		public void Test(int index) {
			Point cellCoord = _cellsCoordinates[index];
			Logger.Instance.WriteLog("{0}\t=> {1}, {2}", index.ToString(), cellCoord.X.ToString(), cellCoord.Y.ToString());
			Logger.Instance.WriteLog("\t=> {0}", _cells[index].ToString());
		}

		public static Rectangle ToIntRect(RectangleF rect) {
			return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		public static Rectangle ToIntRect(RectangleF rect, double scale) {
			return new Rectangle((int)(rect.X * scale), (int)(rect.Y * scale), (int)(rect.Width * scale), (int)(rect.Height * scale));
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
