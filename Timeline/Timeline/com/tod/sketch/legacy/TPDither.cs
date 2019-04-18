using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tod.sketch;

namespace com.tod.sketch {
	class TPDither {

		public static float BRIGHT = 0f;
		public static float DARK = 1f;
		public static int MAX_DITHER_POINTS = 4000;
		//return (0.299 * color.red + 0.587 * color.green + 0.114 * color.blue);
		public static List<TP> FastDitherBinary(Image<Bgr, byte> source, int maxPoints, float rMul, float gMul, float bMul, float thresh, int iterations, float flipDensity = 1f){
			List<TP> points = new List<TP>();

			float vectorLength = (float)Math.Sqrt(rMul * rMul + gMul * gMul + bMul * bMul);
			rMul /= vectorLength;
			gMul /= vectorLength;
			bMul /= vectorLength;

			rMul *= thresh;
			gMul *= thresh;
			bMul *= thresh;

			Random random = new Random();
			float w = (float)source.Cols;
			float h = (float)source.Rows;
			float dw = 1f / w, dh = 1f / h;
			byte[,,] pixels = source.Data;

			for (int y = 0; y < h; y++) {
				for(int x = 0; x < w; x++){
					float 
						r = (float)pixels[(int)y, (int)x, 2] / 255f,
						g = (float)pixels[(int)y, (int)x, 1] / 255f,
						b = (float)pixels[(int)y, (int)x, 0] / 255f;

					float threshold = r * rMul + g * gMul + b * bMul;
					float dIterations = threshold / ((float)iterations) * Math.Max(0f, (float)(iterations - 1));
					if (x % 10 == 0 && y == 0) Logger.Instance.SilentLog("{0}/{1} | ", threshold.ToString("0.00"), dIterations.ToString("0.00"));
					threshold += dIterations;
					TP point = TP.Null;
					for (int ite = 0; ite < iterations && point.IsNull; ite++) {
						float rand = (float)random.NextDouble();
						
						if( rand > threshold ){
							float xoff = (float)random.NextDouble() * 4;
							float yoff = (float)random.NextDouble() * 4;
							int px = (int)(xoff + x);
							int py = (int)(yoff + y);
							point = new TP(px / w, py / h);
							points.Add(point);
						}
					}
				}
			}
			return points;
		}

		private static int [,] _ditheringArray;

		public static List<TP> DitherFloyd(Image<Bgr, byte> source, Image<Gray, byte> sobelSource) {
			List<TP> points = new List<TP>();
			byte[,,] pixels = source.Data;

			int rows = source.Rows;
			int cols = source.Cols;
			_ditheringArray = new int[rows, cols];
			int x = 0, y = 0;
			int red = 0, green = 0, blue = 0, gray = 0;

			for (x = 0; x < cols; x++) {
				for (y = 0; y < rows; y++) {
					red = pixels[y, x, 2];
					green = pixels[y, x, 1];
					blue = pixels[y, x, 0];
					gray = GrayCount(red, green, blue);
					_ditheringArray[y, x] = gray;
				}
			}

			for (y = 1; y < rows - 1; y++) {
				for (x = 1; x < cols - 1; x++) {
					DitheringBreak(y, x);
				}
			}

			double wf = 1 / (double)cols;
			double hf = 1 / (double)rows;

			Image<Bgr, byte> penaltyDebugImage = new Image<Bgr, byte>(cols, rows);
			Point debugPoint = new Point();
			MCvScalar debugColor = new MCvScalar(0, 0, 255);
			MCvScalar debugPixelColor = new MCvScalar(0, 0, 0);

			Image<Bgr, byte> penaltyMap = new Image<Bgr, byte>(cols, rows, new Bgr(0, 0, 0));
			Image<Bgr, byte> threshMap = new Image<Bgr, byte>(cols, rows, new Bgr(0, 0, 0));

			// edges
			sobelSource = sobelSource.SmoothBlur(4, 4);
			//Sketch.ShowProcessImage(sobelSource.Convert<Bgr, byte>(), "blur");
			Image<Gray, float> sobel = sobelSource.Sobel(0, 1, 3).Add(sobelSource.Sobel(1, 0, 3)).AbsDiff(new Gray(0.0));

			Image<Gray, byte> sobelContrast = sobel.Convert<Gray, byte>();
			sobelContrast._GammaCorrect(1.2);
			sobelContrast._Dilate(2);
			//Sketch.ShowProcessImage(sobelContrast.Convert<Bgr, byte>(), "sobel");
			byte[,,] sobelData = sobelContrast.Data;
			// edges

			int ditherPoints = 0;
			for (x = 0; x < cols; x++) {
				for (y = 0; y < rows; y++) {
					gray = _ditheringArray[y, x];
					if (gray == 0 || sobelData[y, x, 0] > 32) {

						double px = x * wf;
						double py = y * hf;

						double cx = 0.48 + _random.NextDouble() * .04;
						double cy = 0.45 + _random.NextDouble() * .04;
						double vx = px - cx;
						double vy = py - cy;
						double dc = Math.Sqrt(vx * vx + vy * vy);
						double inverseDc = 1 - dc;
						double penalty = Math.Pow(dc * 1.1, 1.5);

						double dThresh = _random.NextDouble() * Math.Pow(inverseDc, 1.5);
						// edges mask
						const double sobelInf = .75;
						dThresh *= (1.0 - sobelInf) + sobelInf * (sobelData[y, x, 0] / 256f);

						penaltyMap.Data[y, x, 0] = (byte)(penalty * 255);
						threshMap.Data[y, x, 1] = (byte)(dThresh * 255);

						if (dThresh > penalty) {
							ditherPoints++;
							points.Add(new TP((float)px, (float)py));
						}
						else {
							debugPoint.X = x;
							debugPoint.Y = y;
							CvInvoke.Circle(penaltyDebugImage, debugPoint, 1, debugColor, -1);
						}
					}
				}
			}

			//Sketch.ShowProcessImage(penaltyDebugImage, "Dither Penalty");
			//Sketch.ShowProcessImage(penaltyMap, "penaltyMap");
			Sketch.ShowProcessImage(threshMap, "threshMap");

			return points;
		}

		public static List<TP> DitherFloydx(Image<Bgr, byte> source, Image<Gray, byte> sobelSource) {
			List<TP> points = new List<TP>();
			byte[,,] pixels = source.Data;

			int rows = source.Rows;
			int cols = source.Cols;
			_ditheringArray = new int[rows, cols];
			int x = 0, y = 0;
			int red = 0, green = 0, blue = 0, gray = 0;

			for (x = 0; x < cols; x++) {
				for (y = 0; y < rows; y++) {
					red = pixels[y, x, 2];
					green = pixels[y, x, 1];
					blue = pixels[y, x, 0];
					gray = GrayCount(red, green, blue);
					_ditheringArray[y, x] = gray;
				}
			}

			for (y = 1; y < rows - 1; y++) {
				for (x = 1; x < cols - 1; x++) {
					DitheringBreak(y, x);
				}
			}

			double wf = 1 / (double)cols;
			double hf = 1 / (double)rows;

			Image<Bgr, byte> penaltyDebugImage = new Image<Bgr, byte>(cols, rows);
			Point debugPoint = new Point();
			MCvScalar debugColor = new MCvScalar(0, 0, 255);
			MCvScalar debugPixelColor = new MCvScalar(0, 0, 0);

			// edges
			sobelSource = sobelSource.SmoothBlur(4, 4);
			Sketch.ShowProcessImage(sobelSource.Convert<Bgr, byte>(), "blur");
			Image<Gray, float> sobel = sobelSource.Sobel(0, 1, 3).Add(sobelSource.Sobel(1, 0, 3)).AbsDiff(new Gray(0.0));

			Image<Gray, byte> sobelContrast = sobel.Convert<Gray, byte>();
			sobelContrast._GammaCorrect(1.2);
			sobelContrast._Dilate(2);
			Sketch.ShowProcessImage(sobelContrast.Convert<Bgr, byte>(), "sobel");
			byte[,,] sobelData = sobelContrast.Data;
			// edges

			int ditherPoints = 0;
			for (x = 0; x < cols; x++) {
				for (y = 0; y < rows; y++) {
					gray = _ditheringArray[y, x];
					if (gray == 0) {
						double xoff = 0;// (float)_random.NextDouble() * 1;
						double yoff = 0;// (float)_random.NextDouble() * 1;

						double px = ((double)x + xoff) * wf;
						double py = ((double)y + yoff) * hf;

						double cx = 0.4 + _random.NextDouble() * .2;
						double cy = 0.4 + _random.NextDouble() * .2;
						double vx = (px - cx) * 2.0;
						double vy = (py - cy) * 2.0;
						double dc = Math.Sqrt(vx * vx + vy * vy);
						double inverseDc = 1 - dc;
						double penalty = dc * ( .5 + _random.NextDouble() * .5);

						double dThresh = Math.Min(1.0, (.2 + _random.NextDouble() * .4) * Math.Max(0.01, inverseDc));

						// edges mask
						//dThresh += (sobelData[y, x, 0] - 127f) / 127f;

						if (dc < .8 && (dThresh > penalty || sobelData[y, x, 0] > 64)) {
							ditherPoints++;
							points.Add(new TP((float)px, (float)py));
						}
						else {
							debugPoint.X = x;
							debugPoint.Y = y;
							CvInvoke.Circle(penaltyDebugImage, debugPoint, 1, debugColor, -1);
						}
					}
				}
			}

			Sketch.ShowProcessImage(penaltyDebugImage, "Dither Penalty");

			return points;
		}

		public static int GrayCount(int red, int green, int blue) {
			//return (red + green + blue) / 3;
			return (int)(0.299f * red + 0.587f * green + 0.114f * blue);
		}

		public static void DitheringBreak(int row, int column) {
			int gray = 0;
			if (_ditheringArray[row, column] < 128) {
				gray = _ditheringArray[row, column] / 16;
				_ditheringArray[row, column] = 0;
			}
			else {
				gray = (_ditheringArray[row, column] - 255) / 16;
				_ditheringArray[row, column] = 255;
			}
			_ditheringArray[row + 1, column - 1] += (gray * 3);
			_ditheringArray[row + 1, column] += (gray * 5);
			_ditheringArray[row + 1, column + 1] += gray;
			_ditheringArray[row, column + 1] += (gray * 7);
		}

		private static Random _random = new Random(1);
		private static int _canvasWidth;
		private static int _canvasHeight;
		private static Image<Bgr, byte> _canvas;
		private static byte[, ,] _data;
		public static Image<Bgr, byte> Canvas {
			get { return _canvas; }
			set {
				_canvas = value;
				_data = _canvas.Data;
				_canvasWidth = _canvas.Cols;
				_canvasHeight = _canvas.Rows;
			}
		}

		public static void UpdateCanvas() {
			_canvas.Data = _data;
		}

		private static Random rline = new Random((int)new DateTime().Ticks);
		public static void SetPixel(int x, int y) {

			for (int i = 0; i < 3; i++) {
				int xOffset = (int)((rline.NextDouble() - .5) * 6);
				int yOffset = (int)((rline.NextDouble() - .5) * 6);
				int nx = Clamp0X(x + xOffset, _canvasWidth);
				int ny = Clamp0X(y + yOffset, _canvasHeight);
				int previous = _data[ny, nx, 0];
				_data[ny, nx, 0] = _data[ny, nx, 1] = _data[ny, nx, 2] = (byte)Math.Max(0, previous - 16);
			}
		}

		private static int Clamp0X(int a, int x) {
			if (a < 0) return 0;
			if (a >= x) return x - 1;
			return a;
		}

		public static void SprayLine(int x0, int y0, int x1, int y1) {
			int dy = (y1 - y0);
			int dx = (x1 - x0);
			int stepx, stepy;

			if (dy < 0) { dy = -dy; stepy = -1; }
			else { stepy = 1; }
			if (dx < 0) { dx = -dx; stepx = -1; }
			else { stepx = 1; }
			dy <<= 1;
			dx <<= 1;

			float fraction = 0;

			SetPixel(x0, y0);
			if (dx > dy) {
				fraction = dy - (dx >> 1);
				while (Math.Abs(x0 - x1) > 1) {
					if (fraction >= 0) {
						y0 += stepy;
						fraction -= dx;
					}
					x0 += stepx;
					fraction += dy;
					SetPixel(x0, y0);
				}
			}
			else {
				fraction = dx - (dy >> 1);
				while (Math.Abs(y0 - y1) > 1) {
					if (fraction >= 0) {
						x0 += stepx;
						fraction -= dy;
					}
					y0 += stepy;
					fraction += dx;
					SetPixel(x0, y0);
				}
			}
		}
	}
}
