using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch {

	public struct TP{

		public static TP Null = new TP(0, 0, true);
		public static TP Zero = new TP(0, 0);
		public static TP PenUp = new TP(-1f, -1f);

		public float x, y;
		public bool IsNull;

		public TP(float _x, float _y, bool isNull = false){
			x = _x; y = _y;
			IsNull = isNull;
		}

		public Boolean IsDown {
			get { return x > -.1f && y > -.1f; }
		}

		public float DistanceSquared(TP point){
			float vx = point.x - x;
			float vy = point.y - y;
			return vx * vx + vy * vy;
		}

		public TP Scaled(float sx, float sy) {
			return new TP(x * sx, y * sy);
		}

		public void Normalize(float toScale = 1f) {
			if (x == 0f && y == 0f) return;
			float m = (float)Math.Sqrt(x * x + y * y);
			x = x / m * toScale;
			y = y / m * toScale;
		}

		public bool Equals(TP point) {
			return x == point.x && y == point.y;
		}

		public override string ToString() {
			return String.Format("TP({0}, {1})", x.ToString(), y.ToString());
		}
	}

	public struct TRect {
		public static TRect Null = new TRect(0, 0, 0, 0);

		public float x, y, w, h;

		public TRect(float _x, float _y, float _w, float _h) {
			x = _x;
			y = _y;
			w = _w;
			h = _h;
		}

		public TRect Set(float _x, float _y, float _w, float _h) {
			x = _x;
			y = _y;
			w = _w;
			h = _h;
			return this;
		}

		public TRect Mul(float scale) {
			x *= scale;
			y *= scale;
			w *= scale;
			h *= scale;
			return this;
		}

		public TRect Shrink(float borderOffset) {
			float offX = Math.Min(borderOffset, w / 2);
			float offY = Math.Min(borderOffset, h / 2);
			x += offX;
			y += offY;
			w -= offX;
			h -= offY;
			return this;
		}

		public Rectangle ToRectangle() {
			return new Rectangle((int)x, (int)y, (int)w, (int)h);
		}

		public override string ToString() {
			return String.Format("TRect({0}, {1}, {2}, {3})", x.ToString(), y.ToString(), w.ToString(), h.ToString());
		}
	}
	class TODPath {

		private List<TP> _points;
		private int _index;
		private int _iteIndex;

		public TODPath() {
			_points = new List<TP>();
			_index = 0;
		}

		public void Reset() {
			_index = 0;
		}

		public void StartIte() {
			_iteIndex = 0;
		}

		public bool NextIte(out TP point) {
			if (_iteIndex < _index) {
				point = _points[_iteIndex];
				_iteIndex++;
				return true;
			}

			point = TP.Null;
			return false;
		}

		public void AppendLineSegments(TP point) {
			
		}

		public void Append(TP point) {
			if (_points.Count > _index) {
				_points[_index++] = point;
			}
			else {
				_points.Add(point);
				_index++;

				if (_index != _points.Count) throw new Exception("Points.Count/_Index mismatch");
			}
		}

		public void Append(TODPath path) {
			path.StartIte();
			TP point;
			while (path.NextIte(out point)) {
				Append(point);
			}
		}

		public void Append(params float[] coords) {
			int numCoords = coords.Length - (coords.Length % 2);
			for (int i = 0; i < numCoords; i += 2) {
				Append(new TP(coords[i], coords[i + 1]));
			}
		}

		public TODPath Clone() {
			TODPath path = new TODPath();
			path.Append(this);
			return path;
		}

		override public string ToString() {
			return String.Format("TODPath({0})\tCapacity: {1}\tContent:...", _index.ToString(), _points.Count.ToString());
		}

	}
}
