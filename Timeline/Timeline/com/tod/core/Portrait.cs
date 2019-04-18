using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.core {
	public class Portrait {

		public int uid;
		public Image<Bgr, byte> source;
		public Rectangle face;
		public Rectangle rect;
		public float score;
		public bool changed = false;
		public int debugIndex = 0;
		public double timestamp = 0;
		private bool _saved = false;

		public Portrait(int debugIndex, int uid, Image<Bgr, byte> source, ref Rectangle face, float score) {

			this.debugIndex = debugIndex;

			this.uid = uid;
			this.source = source;
			this.face = rect = face;
			this.score = score;

			timestamp = Config.time.ElapsedMillis;
		}

		public void Copy(Portrait portrait) {

			uid = portrait.uid;
			source = portrait.source;
			face = portrait.face;
			score = portrait.score;
			changed = true;
		}

		public void Clean() {

			if (source != null) {
				source.Dispose();
			}
		}

		public void ScaleRect(double scale) {
			rect.X = (int)(face.X * scale);
			rect.Y = (int)(face.Y * scale);
			rect.Width = (int)(face.Width * scale);
			rect.Height = (int)(face.Height * scale);
		}

		public void Save() {
			_saved = true;
		}

		public bool Saved {
			get { return _saved; }
		}
	}
}
