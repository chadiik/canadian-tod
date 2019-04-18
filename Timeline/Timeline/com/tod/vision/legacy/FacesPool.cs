using com.tod.core;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.vision {

	public delegate void PortraitEvent(Portrait portrait);

	class FacesPool {

		// Add a monitor for displacement over time (center point travel), if none for 'long', then discard => false recognition.

		public static double FLICKER_TIME_TOLERANCE = 1000; // A time advantage for missed capture
		public static float FACE_RECT_GROW = 1.2f;          // Scan area to determine if a next frame rectangle is the same, side-effect: discard fast moving people
		public static float FACE_MAX_SCALE_CHANGE = 1.2f;   // Avoid blobbing into temporary large false-positives
		public static float FACE_MIN_TRAVEL_TIME_ONE = 20f; // Min travel distance (in pixels), at SCORE_TIME_ONE, required to register (Eliminate stationary)

		// CAPTURE_FACE_TRANSFORMS is also used by TODFilters in Equalize functions
		public static float[] CAPTURE_FACE_TRANSFORMS = new float[] { 0, -0.15f, 1.6f, 2.2f }; // translateX, translateY, scaleX, scaleY (factors of capture rectangle)

		public static float SCORE_SIZE_ONE = 90f * 180f;        // Optimal face dimensions cap (pixels) => Score = 1f
		public static double SCORE_TIME_ONE = 1800;             // Optimal capture duration cap (seconds) => Score = 1f
		public static double SCORE_TIME_MAX = 10000;            // Highest recorded capture time

		public static float
			SCORE_MUL_SIZE = .2f,
			SCORE_MUL_TIME = .4f,
			SCORE_MUL_EYES = .4f;

		private int _poolSize;
		private List<Rectangle> _faces;
		private List<double> _startCaptureTime;
		private List<double> _lastCaptureTime;
		private List<float> _scores;
		private List<int> _sorted;
		private List<int> _numEyes;
		private List<int> _uid;
		private List<double> _lastScoreTime;

		private List<int> _lastOriginX; // this is to eliminate artificial and stationary face-like features in the environment.
		private List<int> _lastOriginY; // this is to eliminate artificial and stationary face-like features in the environment.
		private List<float> _travel;

		private List<Rectangle> _eyes;
		public List<Rectangle> Eyes { set { _eyes = value; } }

		private Rectangle _temp = new Rectangle(0, 0, 0, 0);
		private static DateTime START = DateTime.UtcNow;

		public Mat sharedMat;
		public Mat sharedMatDebug;

		public FacesPool(int length) {
			
			_poolSize = length;

			_faces = new List<Rectangle>(_poolSize);
			_startCaptureTime = new List<double>(_poolSize);
			_lastCaptureTime = new List<double>(_poolSize);
			_scores = new List<float>(_poolSize);
			_sorted = new List<int>(_poolSize);
			_numEyes = new List<int>(_poolSize);
			_uid = new List<int>(_poolSize);
			_lastScoreTime = new List<double>(_poolSize);

			_lastOriginX = new List<int>();
			_lastOriginY = new List<int>();
			_travel = new List<float>();

			for (int i = 0; i < _poolSize; i++) {
				_faces.Add(new Rectangle(0, 0, 0, 0));
				_startCaptureTime.Add(0);
				_lastCaptureTime.Add(0);
				_scores.Add(0);
				_sorted.Add(i);
				_numEyes.Add(0);
				_uid.Add(0);
				_lastScoreTime.Add(0);

				_lastOriginX.Add(0);
				_lastOriginY.Add(0);
				_travel.Add(0);
			}
		}

		private double ElapsedMillis {
			get { return Config.time.ElapsedMillis; }
		}

		public void Reset() {
			for (int i = 0; i < _poolSize; i++) {
				_faces[i] = new Rectangle(0, 0, 0, 0);
				_startCaptureTime[i] = (0);
				_lastCaptureTime[i] = (0);
				_scores[i] = (0);
				_sorted[i] = (i);
				_numEyes[i] = (0);
				_uid[i] = 0;
				_lastScoreTime[i] = 0;

				_lastOriginX[i] = 0;
				_lastOriginY[i] = 0;
				_travel[i] = (0);
			}

			_f0 = 0;
		}

		private void SetFace(ref Rectangle face, int x, int y, int w, int h, int c, int r) {
			x = x >= c ? c - 1 : (x < 0 ? 0 : x);
			y = y >= r ? r - 1 : (y < 0 ? 0 : y);
			face.X = x;
			face.Y = y;
			face.Width = x + w >= c ? c - x - 1 : w - 1;
			face.Height = y + h >= r ? r - y - 1 : h - 1;
		}

		private int NumEyes(ref Rectangle face) {
			int eyes = 0;
			int numEyes = _eyes.Count;
			for (int i = 0; i < numEyes; i++) {
				if (face.Contains(_eyes[i])) {
					eyes++;
				}
			}
			return eyes;
		}

		public bool Process(Rectangle inFace) {
			double t = ElapsedMillis;

			int wp = (int)Math.Max(inFace.Width * FACE_RECT_GROW, 1);
			int hp = (int)Math.Max(inFace.Height * FACE_RECT_GROW, 1);

			SetFace(ref _temp,
						inFace.X - (wp - inFace.Width) / 2,
						inFace.Y - (hp - inFace.Height) / 2,
						wp, hp,
						sharedMat.Cols, sharedMat.Rows
						);

			float inArea = inFace.Width * inFace.Height;

			CvInvoke.Rectangle(sharedMatDebug, _temp, new Bgr(Color.Blue).MCvScalar, 1);

			for (int iFace = 0; iFace < _poolSize; iFace++) {
				Rectangle face = _faces[iFace];
				double dt = t - _lastCaptureTime[iFace];
				float faceArea = face.Width * face.Height;
				float scaleRatio = faceArea > inArea ? faceArea / inArea : inArea / faceArea;

				if (scaleRatio < FACE_MAX_SCALE_CHANGE && _temp.Contains(face) && dt < FLICKER_TIME_TOLERANCE) { // same face
					SetFace(ref face,
						(face.X + inFace.X) / 2,
						(face.Y + inFace.Y) / 2,
						(face.Width + inFace.Width) / 2,
						(face.Height + inFace.Height) / 2,
						sharedMat.Cols, sharedMat.Rows
						);
					_faces[iFace] = face;
					CvInvoke.Rectangle(sharedMatDebug, face, new Bgr(Color.Red).MCvScalar, 2);

					double captureTime = Math.Min(SCORE_TIME_MAX, _lastCaptureTime[iFace] - _startCaptureTime[iFace]) / 1000;
					CvInvoke.PutText(sharedMatDebug, captureTime.ToString("0.0"), new Point(face.Right + 2, face.Top - 4), FontFace.HersheyPlain, 1.5, new Bgr(Color.Black).MCvScalar, 4);
					CvInvoke.PutText(sharedMatDebug, captureTime.ToString("0.0"), new Point(face.Right + 2, face.Top - 4), FontFace.HersheyPlain, 1.5, new Bgr(Color.Red).MCvScalar, 1);
					_lastCaptureTime[iFace] = t;

					_numEyes[iFace] = NumEyes(ref face);

					if (_lastOriginX[iFace] > 0 && _lastOriginY[iFace] > 0) {
						int vx = inFace.X - _lastOriginX[iFace];
						int vy = inFace.Y - _lastOriginY[iFace];
						float d = (float)Math.Sqrt(vx * vx + vy * vy);
						_travel[iFace] += d;
					}
					_lastOriginX[iFace] = inFace.X;
					_lastOriginY[iFace] = inFace.Y;

					return true;
				}
			}

			// A new face
			for (int iFace = 0; iFace < _poolSize; iFace++) {
				Rectangle face = _faces[iFace];
				if (face.Width == 0 && face.Height == 0) { // free
					SetFace(ref face,
						inFace.X,
						inFace.Y,
						inFace.Width,
						inFace.Height,
						sharedMat.Cols, sharedMat.Rows
						);
					_faces[iFace] = face;

					CvInvoke.Rectangle(sharedMatDebug, face, new Bgr(Color.LightGreen).MCvScalar, 4);
					_startCaptureTime[iFace] = t;
					_lastCaptureTime[iFace] = t;

					return true;
				}
			}

			// No face
			return false;
		}

		public static bool debug = false;

		private float _f0 = 0;
		public List<float> CalculateScores() {
			double t = ElapsedMillis;

			for (int i = 0; i < _poolSize; i++) {
				_sorted[i] = i; // reset indices

				// check last capture time, and reset accordingly
				double dt = t - _lastCaptureTime[i];
				if (dt > FLICKER_TIME_TOLERANCE || _lastCaptureTime[i] - _startCaptureTime[i] > SCORE_TIME_MAX) {
					Rectangle face = _faces[i];
					face.Width = 0;
					face.Height = 0;
					_faces[i] = face;
					_lastCaptureTime[i] = double.MaxValue;
					_numEyes[i] = 0;
					_uid[i]++;
					_lastScoreTime[i] = 0;

					_lastOriginX[i] = _lastOriginY[i] = 0;
					_travel[i] = 0;
				}
			}

			// Calculate scores
			for (int i = 0; i < _poolSize; i++) {
				Rectangle face = _faces[i];

				double dt = t - _lastScoreTime[i];
				float area = (float)(face.Width * face.Height);

				if (/*_numEyes[i] > 0 && */area > 100) {
					double captureTime = Math.Min(SCORE_TIME_MAX, _lastCaptureTime[i] - _startCaptureTime[i]);
					int eyes = _numEyes[i];

					float s_size = Math.Min(SCORE_SIZE_ONE, area) / SCORE_SIZE_ONE;
					float s_time = (float)(Math.Min(SCORE_TIME_ONE, captureTime) / SCORE_TIME_ONE);
					float s_eyes = eyes == 2 ? 1f : (eyes == 1 ? .25f : 0f);
					float score = _scores[i] = s_size * SCORE_MUL_SIZE + s_time * SCORE_MUL_TIME + s_eyes * SCORE_MUL_EYES;

					if (score > 0.5f && _travel[i] > FACE_MIN_TRAVEL_TIME_ONE) {
						int wp = (int)Math.Max(face.Width * CAPTURE_FACE_TRANSFORMS[2], 1);
						int hp = (int)Math.Max(face.Height * CAPTURE_FACE_TRANSFORMS[3], 1);

						SetFace(ref _temp,
							face.X - (wp - face.Width) / 2 + (int)(CAPTURE_FACE_TRANSFORMS[0] * (float)face.Width),
							face.Y - (hp - face.Height) / 2 + (int)(CAPTURE_FACE_TRANSFORMS[1] * (float)face.Height),
							wp,
							hp,
							sharedMat.Cols, sharedMat.Rows
						);

						CvInvoke.Rectangle(sharedMatDebug, _temp, new Bgr(Color.Red).MCvScalar, 1);
						if (score > _f0) {
							int uid = 10000 + _uid[i] * 1000 + i;
							float timelessScore = _scores[i] = s_size * SCORE_MUL_SIZE + s_eyes * SCORE_MUL_EYES;
							if (Add(uid, sharedMat, face, _temp, score, timelessScore)) {
								_lastScoreTime[i] = t;
							}
						}
					}
				}
			}

			return _scores;
		}

		#region Archives

		public static event PortraitEvent PortraitCreated;
		public static event PortraitEvent CandidateFound;

		private static List<Portrait> _portraits = new List<Portrait>();
		private static List<float> _timelessScores = new List<float>();

		public static List<Portrait> Portraits {
			get { return _portraits; }
		}

		public static bool Best(out Portrait outPortrait, int range = int.MaxValue) {

			Portrait portrait = null;

			int numPortraits = _portraits.Count;
			if (numPortraits > 0) {
				range = Math.Max(0, _portraits.Count - Math.Abs(range));
				int highScoreIndex = -1;
				float highScore = 0;
				for (int i = numPortraits - 1; i >= range; i--) {
					if (!_portraits[i].Saved && _portraits[i].score > highScore) {
						highScore = _portraits[i].score;
						highScoreIndex = i;
					}
				}

				if (highScoreIndex > -1) {
					_portraits[highScoreIndex].Save();
					outPortrait = _portraits[highScoreIndex];
					return true;
				}
			}

			outPortrait = portrait;
			return false;
		}

		private static int _debugIndex = 0;
		public static bool Add(int uid, Mat source, Rectangle face, Rectangle frame, float score, float timelessScore) {
			Image<Bgr, byte> sourceImage = source.ToImage<Bgr, byte>();
			sourceImage = sourceImage.Copy(frame);
			face.X -= frame.X;
			face.Y -= frame.Y;
			Portrait portrait = new Portrait(_debugIndex++, uid, sourceImage, ref face, score);
			PortraitCreated?.Invoke(portrait);

			int numPortraits = _portraits.Count;
			for (int iPortrait = 0; iPortrait < numPortraits; iPortrait++) {
				Portrait testPortrait = _portraits[iPortrait];
				if (testPortrait.uid == uid) {
					if (timelessScore > _timelessScores[iPortrait]) {
						testPortrait.Copy(portrait);
						_timelessScores[iPortrait] = timelessScore;
						CandidateFound?.Invoke(testPortrait);
						return true;
					}
					return false;
				}
			}

			CandidateFound?.Invoke(portrait);
			_portraits.Add(portrait);
			_timelessScores.Add(timelessScore);
			return true;
		}

		#endregion
	}
}
