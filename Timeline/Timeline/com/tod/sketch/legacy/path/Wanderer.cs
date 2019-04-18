using System;
using System.Collections.Generic;

namespace com.tod.sketch.path {
	class Wanderer {

		public static float EaseInOutQuad(float a, float b, float t, float duration) {
			t /= duration / 2f;
			if (t < 1f) return b / 2f * t * t + a;
			t -= 1f;
			return -b / 2f * (t * (t - 2f) - 1f) + a;
		}

		public PathSequencer sequencer;
		public TP position;

		public Wanderer(PathSequencer pathSequencer) {
			sequencer = pathSequencer;
			position = new TP();
		}

		public virtual bool Update() {
			return false;
		}
	}
}
