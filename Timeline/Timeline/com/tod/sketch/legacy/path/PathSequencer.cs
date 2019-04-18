using System;
using System.Collections.Generic;

namespace com.tod.sketch.path {
	class PathSequencer {

		public static float DEFAULT_PASSING_DISTANCE = (float)Math.Pow(0.01, 2);
		public static float DEFAULT_TRAIL_SPEED = .1f;

		private TP _trail = new TP();
		private OTP _currentPosition;
		private TP _motion;
		private List<OTP> _path;
		private int _index;
		private float _step;
		private float _frac;
		private OTP _targetPosition;
		private float _passingDistance;
		private bool _loop = false;
		private bool _penUp = false;
		private float _deltaLength;

		public PathSequencer(float passingDistance) {
			_currentPosition = new OTP();
			_motion = new TP();
			_targetPosition = new OTP();
			_passingDistance = passingDistance;
		}

		public List<OTP> Path {
			set {
				_index = 0;
				_path = value;
				_currentPosition = Pop();
			}
		}

		public float DeltaLength {
			get {
				return _deltaLength;
			}
		}

		public TP CurrentPosition {
			get { return _currentPosition.point; }
		}

		public TP Trail {
			get { return _trail; }
		}

		public TP Motion {
			get { return _motion; }
		}

		public float Step{
		  get { return _step; }
		  set { _step = value; }
		}

		public float Frac {
			get { return _frac; }
			set { _frac = value; }
		}

		public bool Loop {
			get { return _loop; }
			set { _loop = value; }
		}

		public bool PenUp {
			get {
				return _penUp;
			}
		}

		private void UpdatePosition() {
			TP delta = new TP(_targetPosition.point.x - _currentPosition.point.x, _targetPosition.point.y - _currentPosition.point.y);
			_deltaLength = (float)Math.Sqrt(delta.x * delta.x + delta.y * delta.y);
			_motion.x = _motion.y = 0f;
			if (_deltaLength > _passingDistance * .5f) {
				if(_frac > 0){
					_motion.x = delta.x / _frac;
					_motion.y = delta.y / _frac;
				}
				else{
					_motion.x = delta.x / _deltaLength * Step;
					_motion.y = delta.y / _deltaLength * Step;
				}
			}

			TP point = _currentPosition.point;
			point.x += _motion.x;
			point.y += _motion.y;
			_currentPosition.point = point;

			_trail.x += (_currentPosition.point.x - _trail.x) * DEFAULT_TRAIL_SPEED;
			_trail.y += (_currentPosition.point.y - _trail.y) * DEFAULT_TRAIL_SPEED;
		}

		private bool UpdateQueue() {
			if (_currentPosition.point.DistanceSquared(_targetPosition.point) < _passingDistance) {
				if (_index >= _path.Count) {
					if (_loop) _index = 0;
					else return false;
				}
				_targetPosition = Pop();
			}

			return true;
		}

		private OTP Pop() {
			_penUp = false;
			while (_index < _path.Count && _path[_index].point.x == TP.PenUp.x && _path[_index].point.y == TP.PenUp.y) {
				_index++;
				_penUp = true;
			}
				
			if(_index < _path.Count) return _path[_index++];
			
			return _targetPosition;
		}

		public bool Update() {
			UpdatePosition();
			return UpdateQueue();
		}

		
	}
}
