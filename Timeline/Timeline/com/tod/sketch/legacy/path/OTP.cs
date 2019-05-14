using com.tod.sketch;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch.path {

	public class OTP{

		public TP point;
		public OTP next = null;
		public bool visited = false;
		public float hue = 0f;
		public float brightness = 0f;

		public OTP() { }
		public OTP(TP tp) {
			point = tp;
		}
	}
}
