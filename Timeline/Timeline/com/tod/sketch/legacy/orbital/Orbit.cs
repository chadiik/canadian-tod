using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tod.sketch;

namespace com.tod.sketch.orbital {
	class Orbit {

		public const float GOOD_RATIO = 0.002f / 0.5f;
		public static float Angle(TP vector) {
			return (float)Math.Atan2(vector.y, vector.x);
		}

		public const float PI = (float)Math.PI;
		public const float TWO_PI = PI * 2f;

		private float _phase;
		private TP _onCircle;
		private float _dir;

		

		public Orbit(float phase01 = 0f) {
			_phase = phase01 * TWO_PI;
			_onCircle = new TP();
			_dir = 0f;
		}

		public float PhaseStep { get; set; }
		public float Dir {
			get {
				return _dir;
			}
			set {
				_dir = value;
			}
		}
		public TP OnCircle {
			get {
				return _onCircle;
			}
		}

		public void Update() {
			_onCircle.x = (float)Math.Cos(_phase + _dir);
			_onCircle.y = (float)Math.Sin(_phase + _dir);
			_phase += PhaseStep;
		}

	}
}
