using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch {
	class ImageData {

		private Image<Bgr, byte> _image;
		private byte[, ,] _data;
		private byte[, ,] _dataHsv;
		public int w, h;

		public ImageData() {
			
		}

		public Image<Bgr, byte> Image {
			get { return _image; }
			set {
				_image = value.SmoothBlur(3, 3);
				_image.Resize(1.0 / 3.0, Emgu.CV.CvEnum.Inter.Linear);
				w = _image.Cols;
				h = _image.Rows;
				_image.ROI = new System.Drawing.Rectangle(0, 0, w, h);
				_data = _image.Data;
			}
		}

		public void CreateHsv() {
			if(_dataHsv == null) _dataHsv = _image.Convert<Hsv, byte>().Data;
		}

		public int GetColor(int x, int y, int channelBGR) {
			if (x >= w || x < 0 || y >= h || y < 0) return 0;
			return _data[y, x, channelBGR];
		}

		public int GetColor(int x, int y) {
			if (x >= w || x < 0 || y >= h || y < 0) return 0;

			int r = _data[y, x, 2];
			int g = _data[y, x, 1];
			int b = _data[y, x, 0];
			return (r + g + b) / 3;
		}

		public int GetHsv(int x, int y, int channelHsv){
			if (x >= w || x < 0 || y >= h || y < 0) throw new Exception("GetHsv outOfBounds");
			return _data[y, x, channelHsv];
		}

		public float GetHsv(float x, float y, int channelHsv) {
			int xi = (int)(x * (float)w);
			int yi = (int)(y * (float)h);
			if (xi >= w || xi < 0 || yi >= h || yi < 0) throw new Exception("GetHsv outOfBounds");
			return (float)_data[yi, xi, channelHsv] / 255f;
		}
	}
}
