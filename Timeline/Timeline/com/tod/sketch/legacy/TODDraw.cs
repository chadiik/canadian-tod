using com.tod.core;
using com.tod.sketch.orbital;
using com.tod.sketch.path;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Timers;

namespace com.tod.sketch {

	class TODDraw {

		public static MCvScalar BLACK = new Bgr(Color.Black).MCvScalar;
		public static MCvScalar WHITE = new Bgr(Color.White).MCvScalar;

		public event SketchComplete SketchCompleted;

		private Portrait _face;

		private Image<Bgr, byte> _sourceImage;
		private Image<Bgr, byte> _filteredImage;
		private Image<Bgr, byte> _focusImage;
		private ImageData _imageData;
		private bool _updateProcess;
		private bool _completed;
		private List<TP> _path;
		private List<OTP> _edges;

		private OrbitalWanderer _wanderer;

		private Image<Bgr, byte> _tImg;
		private MCvScalar _tCol;
		private int _penUp;
		private int _numPathPoints;

		public void Draw(Portrait face) {

			_previewImage = null;

			_face = face;

			_updateProcess = false;
			_penUp = 0;

			Setup();
			//Start(); merged

			/*
			Timer timer = new Timer(1000.0 / 30);
			ElapsedEventHandler timerHandler = null;
			timerHandler = new ElapsedEventHandler((object sender, ElapsedEventArgs eventArgs) => {
				if(Update() == false) {
					timer.Elapsed -= timerHandler;
					timer.Stop();
					_updateProcess = false;
					Logger.Instance.SilentLog("Sketch > Timer stop");
				}
			});
			timer.Elapsed += timerHandler;
			timer.Start();
			*/

			Thread processThread = null;
			processThread = new Thread(() => {
				if (Update() == false) {
					TimerEnd("Orbital wandering");
					processThread.Abort();
					_updateProcess = false;
				}
			});
			processThread.Start();
		}

		private static Stopwatch s_Stopwatch = null;
		private static void TimerStart() { s_Stopwatch = Stopwatch.StartNew(); }
		private static void TimerEnd(string title) {
			s_Stopwatch.Stop();
			Console.WriteLine(" | | | {0} took {1} s", title, (s_Stopwatch.ElapsedMilliseconds / 1000.0).ToString(".00"));
		}

		private void Setup() {

			Logger.Instance.WriteLog("Sketch.Setup");
			_completed = false;

			int hMax = 500;

			TimerStart();
			// Scale and apply easy filter
			double scale = 1;
			_sourceImage = _face.source.Clone();
			_sourceImage = TODFilters.ScaleToFit(_sourceImage, hMax, out scale);
			Sketch.ShowProcessImage(_sourceImage.Clone(), "source");
			int w = _sourceImage.Cols, h = _sourceImage.Rows;
			_face.ScaleRect(scale);

			TRect faceRect = FromRect(_face.rect);
			faceRect.Shrink(faceRect.w * .15f);
			TODFilters.SourceFilter(_sourceImage, faceRect);

			// Focus map
			_focusImage = new Image<Bgr, byte>(Config.files.focusMap);
			double focusScale = 1;
			_focusImage = TODFilters.ScaleToFit(_focusImage, hMax, out focusScale);
			TimerEnd("Filters");

			TimerStart();
			// Dither map creation
			Image<Bgr, byte> clahe = _sourceImage.Clone();
			Image<Gray, byte> edgesSource;
			TODFilters.Clahe(clahe, faceRect, _focusImage, out edgesSource);

			Image<Bgr, byte> houghEdges = _sourceImage.Clone();
			_edges = TODFilters.HoughEdges(edgesSource);
			PathOrder._edges = _edges;
			PathOrder._edges = new List<OTP>();
			PathOrder._edges.Add(new OTP(TP.PenUp));

			_filteredImage = clahe;

			// START()

			Logger.Instance.WriteLog("Sketch.Start");

			_imageData = new ImageData();
			_imageData.Image = _filteredImage;

			//int w = _filteredImage.Cols, h = _filteredImage.Rows;
			Image<Gray, byte> eqGray = _sourceImage.Convert<Gray, byte>();
			eqGray._EqualizeHist();

			List<TP> points = TPDither.DitherFloyd(_filteredImage, eqGray);
			Image<Bgr, byte> dithered = new Image<Bgr, byte>(w, h, new Bgr(255, 255, 255));
			DrawDither(points, dithered);

			Image<Bgr, double> eqColor = eqGray.Convert<Bgr, byte>().Convert<Bgr, double>();
			eqColor._Mul(1.0 / 255.0);
			_sourceImage = _sourceImage.Convert<Bgr, double>().Mul(eqColor).Convert<Bgr, byte>();
			// preview off Sketch.ShowProcessImage(_sourceImage.Clone(), "Eq - Orbital Wanderer map");
			TimerEnd("Dither and edges");

			Sketch.ShowProcessImage(dithered, String.Format("Dither: {0} points", points.Count.ToString()));

			TimerStart();
			Image<Bgr, byte> debugPath = new Image<Bgr, byte>(w, h);
			List<OTP> path = PathOrder.Order(points, _imageData, debugPath);
			_numPathPoints = path.Count - 200;

			#region less wander
			_path = new List<TP>(_numPathPoints);
			for (int i = 0; i < _numPathPoints; i++) {
				_path.Add(path[i].point);
			}
			TimerEnd("Path order");

			//OnCompleted();
			//return;
			#endregion

			TimerStart();
			Orbit orbit = new Orbit();
			orbit.PhaseStep = 0.1f;

			PathSequencer sequencer = new PathSequencer(PathSequencer.DEFAULT_PASSING_DISTANCE);
			sequencer.Step = 0.002f;
			sequencer.Frac = 10f; Logger.Instance.SilentLog(">>>>>>>>>>>>>> Drawing generation could be sped up here!");
			sequencer.Path = path;

			_wanderer = new OrbitalWanderer(orbit, sequencer);

			_path = new List<TP>();
			_updateProcess = true;
			_completed = false;
		}

		private bool Update() {

			for (int i = 0; i < 50; i++) {
				if (_updateProcess) {
					_updateProcess = UpdateProcess();
					_completed = !_updateProcess;
					if (_completed) {
						OnCompleted();
                        _updateProcess = false;
                    }
				}
				else if (_previewImage != null) {
					Thread previewThread = null;
					previewThread = new Thread(() => {
						while (UpdatePreview())
							Thread.Sleep(20);
					});
					previewThread.Start();

					return false;
				}
			}

			return true;
		}

		private void ExtendPath() {

			TP center = new TP(.5f, .5f);
			Random rand = new Random();
			int pathLength = _path.Count;
			List<TP> extension = new List<TP>();
			for(int i = 0; i < pathLength; i++) {
				TP p = _path[i];
				double dSq = p.DistanceSquared(center);
				if(dSq < .05 + rand.NextDouble() * .1) {
					extension.Add(new TP(p.x, p.y));
				}
			}

			extension.AddRange(_path);
			_path = extension;
		}

		// legacy private FaceEntry _previewEntry;
		private void OnCompleted() {
			Logger.Instance.WriteLog("Sketch.OnCompleted");

			//ExtendPath();

			const int clip = 100;
			if (_path.Count > clip)
				_path.RemoveRange(0, clip);

			SketchCompleted?.Invoke(Line.Convert(_path));
			DrawVisuals();

			/* legacy _previewEntry = */Sketch.ShowProcessImage(_previewImage, null);
			// legacy _handler.Invoke(_face.imageName, ToFloatList(_path));

		}

		private List<float> ToFloatList(List<TP> list) {

			int numList = list.Count;
			List<float> floatList = new List<float>(numList * 2);

			int penup = 0;
			int index = 0;
			while (index < numList) {
				penup++;
				while (index < numList && list[index].x == TP.PenUp.x && list[index].y == TP.PenUp.y) {
					index++;
					penup = 0;
				}

				if (index < numList) {
					if (penup < 1) {
						floatList.Add(-1f);
						floatList.Add(-1f);
					}
					TP point = list[index];
					floatList.Add(point.x);
					floatList.Add(point.y);
				}

				index++;
			}

			return floatList;
		}


		private bool UpdateProcess() {

			int w = _sourceImage.Cols;
			int h = _sourceImage.Rows;
			float wf = (float)w;
			float hf = (float)h;

			bool continueFlag = true;
			int iterations = 2000;
			for (int ite = 0; ite < iterations && continueFlag; ite++) {

				float phaseStep = 0.125f;
				_wanderer.Orbit.PhaseStep = phaseStep;

				continueFlag = _wanderer.Update();

				if (_wanderer.position.x > 0.05f && _wanderer.position.y > 0.05f) _path.Add(_wanderer.position); // OR trail
				if (_wanderer.sequencer.PenUp) _path.Add(TP.PenUp);
			}

			return continueFlag;
		}


		private Image<Bgr, byte> _previewImage;
		private Point _lastPoint, _drawPoint;
		private void DrawVisuals() {

			int scale = 3;
			int w = _sourceImage.Cols * scale;
			int h = _sourceImage.Rows * scale;

			_previewImage = new Image<Bgr, byte>(w, h, new Bgr(255, 255, 255));
			TPDither.Canvas = _previewImage;

			int numPoints = _path.Count - 3;
			_lastPoint = new Point(-1, -1);
			_drawPoint = new Point();
		}

		private TP Pop(int _index) {
			_penUp++;
			while (_index < _path.Count && _path[_index].x == TP.PenUp.x && _path[_index].y == TP.PenUp.y) {
				_index++;
				_penUp = 0;
			}

			if (_index < _path.Count) return _path[_index++];

			return _path[_path.Count - 1];
		}

		private static Point ToPoint(TP p) {
			return new Point((int)p.x, (int)p.y);
		}

		public void DrawDither(List<TP> points, Image<Bgr, byte> drawTarget) {
			float w = (float)drawTarget.Cols;
			float h = (float)drawTarget.Rows;

			int numPoints = points.Count;
			Logger.Instance.WriteLog("{0} Dither points", numPoints.ToString());
			_tImg = drawTarget;
			_tCol = BLACK;
			for (int iPoint = 0; iPoint < numPoints; iPoint++) {
				TP point = points[iPoint];
				int x = (int)(point.x * w);
				int y = (int)(point.y * h);

				_tCol = _sourceImage[y, x].MCvScalar;
				SetPixel(x, y, 1);
			}
		}

		private void SetPixel(int x, int y, int radius) {

			int xmin = Math.Max(0, x - radius + 1);
			int ymin = Math.Max(0, y - radius + 1);
			int xmax = Math.Min(_tImg.Cols, x + radius);
			int ymax = Math.Min(_tImg.Rows, y + radius);
			for (x = xmin; x < xmax; x++) {
				for (y = ymin; y < ymax; y++) {
					_tImg.Data[y, x, 0] = (byte)_tCol.V0;
					_tImg.Data[y, x, 1] = (byte)_tCol.V1;
					_tImg.Data[y, x, 2] = (byte)_tCol.V2;
				}
			}
		}
		
		private Rectangle FromRect(TRect rect) {
			return new Rectangle((int)rect.x, (int)rect.y, (int)rect.w, (int)rect.h);
		}
		private TRect FromRect(Rectangle rect) {
			return new TRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		private int _index = 0;
		internal bool UpdatePreview() {

			int scale = 3;
			int w = _sourceImage.Cols * scale;
			int h = _sourceImage.Rows * scale;
			float wf = w;
			float hf = h;

            bool sprayLine = Config.sprayLine;
			for (int i = 0; i < 200; i++) {
				int numPoints = _path.Count;
				if (_index < numPoints) {
					TP point = Pop(_index);
					_drawPoint.X = (int)(point.x * wf);
					_drawPoint.Y = (int)(point.y * hf);
					if (_penUp > 2 && _drawPoint.X > 1 && _lastPoint.X > 1) {
                        if(sprayLine)
						    TPDither.SprayLine(_lastPoint.X, _lastPoint.Y, _drawPoint.X, _drawPoint.Y);
                        else
                            Stroke(_lastPoint, _drawPoint);
                    }
					_lastPoint = _drawPoint;
				}
				_index++;
			}

			Sketch.ShowProcessImage(_previewImage, null);
			//Sketch.ShowProcessImage(TPDither.Canvas);

			return _index < _path.Count;
		}

        private void Stroke(Point p0, Point p1) {
            CvInvoke.Line(TPDither.Canvas, p0, p1, BLACK, 1);
        }
	}
}
