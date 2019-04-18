using System;
using System.Collections.Generic;
using com.tod.sketch.path;

namespace com.tod.sketch.orbital {
	class OrbitalWanderer : Wanderer {

		public float radiusChangeSpeed = .2f;

		private Orbit _orbit;
		private float _radius;
		private float _targetRadius;

		public OrbitalWanderer(Orbit orbit, PathSequencer pathSequencer):base(pathSequencer) {
			_orbit = orbit;
		}

		public float Radius {
			set {
				_targetRadius = value;
			}
		}

		public Orbit Orbit {
			get {
				return _orbit;
			}
			
		}

		private void UpdateRadius() {
			_radius = _radius + (_targetRadius - _radius) * radiusChangeSpeed;//Wanderer.EaseInOutQuad(_radius, _targetRadius, )
		}

		public override bool Update() {
			//_orbit.Dir = Orbit.Angle(sequencer.Motion);
			_orbit.Update();
			bool continueFlag = sequencer.Update();

			TP orbitCircle = _orbit.OnCircle;
			TP wanderer = sequencer.Trail;

			UpdateRadius();

			position.x = wanderer.x + orbitCircle.x * _radius;
			position.y = wanderer.y + orbitCircle.y * _radius;

			return continueFlag;
		}
	}
}
