using com.tod.canvas;
using com.tod.core;
using com.tod.ik;
using com.tod.sketch;
using com.tod.stream;
using com.tod.vision;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace com.tod.scenarios {
	public class Scenario {

		public enum State {
			Idle,
			ProcessingPortrait,
			Streaming
		}

		public State state;
		public Vision vision;
		public Wall wall;
		public IK ik;
		public Streamer streamer;

		public Scenario(Wall wall) {

			this.wall = wall;
			state = State.Idle;

			vision = new Vision( Config.debug ? Source.VideoFile : Source.Camera);
			vision.CandidateFound += OnCandidateFound;

			ik = new IK();
			ik.ConversionCompleted += OnIKConversionCompleted;

			streamer = new CanadianStreamer(!Config.stream);
			streamer.ConnectionFailed += () => Logger.Instance.WriteLog("Scenario: CONNECTION TO CANADIAN STREAMER FAILED!");

			Timer timer = new Timer(1000.0);
			ElapsedEventHandler handler = null;
			handler = new ElapsedEventHandler((object sender, ElapsedEventArgs eventArgs) => Update());
			timer.Elapsed += handler;
			timer.Start();
		}

        public void Pause() {

            ik.Paused = true;
            streamer.Pause();
			vision.Pause();

			Logger.Instance.WriteLog("Scenario: Paused");
		}

        public void Resume() {

            ik.Paused = false;
            streamer.Resume();
			vision.Resume();

			Logger.Instance.WriteLog("Scenario: Resumed");
		}

		public void Stop() {

			vision.Stop();
			streamer.Close();
		}

		private void OnCandidateFound(Portrait portrait) {

			if(state == State.Idle) {

				Sketch.SketchCompleted += OnSketchCompleted;
				Logger.Instance.WriteLog("{0}{0}Scenario: NEW SKETCH{0}", Environment.NewLine);
				Sketch sketch = new Sketch(Config.version);
				sketch.Draw(portrait);
				state = State.ProcessingPortrait;
			}
		}

        private long m_LastCompletedOn = 0;
		private void OnSketchCompleted(List<TP> path) {

            Sketch.SketchCompleted -= OnSketchCompleted;
            if (Config.time.ElapsedMillis - m_LastCompletedOn < 100)
                return;

            m_LastCompletedOn = Config.time.ElapsedMillis;

            List<TP> square = new List<TP> {
                TP.PenUp,
                    new TP(.1f, .1f),
                    new TP(.9f, .1f),
                    new TP(.9f, .9f),
                    new TP(.1f, .9f),
                    new TP(.1f, .1f)
                };

            if (false) {

                path.Clear();
                for (int i = 0; i < 300; i++)
                    path.AddRange(square);
            }

            if (false) {
                path.InsertRange(0, square);
                path.InsertRange(0, square);
            }

            List<TP> sketch = wall.ToCell(path, (Config.cell++) % wall.UnitCells.Count);
			switch (Config.version) {
				case Sketch.Version.Legacy:
					sketch = Sketch.Optimize(sketch, 4, 100);
					break;

				case Sketch.Version.Hatch:
					//sketch = Sketch.Optimize(sketch, 0, 200);
					tod.sketch.hatch.Hatch.SketchPreview(sketch.ToList(), new Image<Bgr, byte>(wall.width, wall.height, new Bgr(255, 255, 255)), new MCvScalar(0), 1);
					break;
			}

            Sketch.DrawToWall(sketch);

            int xOffset = Config.xOffset;
            int yOffset = Config.yOffset;
            float xScale = (float)Config.xScale;
            float overallScale = 1f;
            for (int i = 0, numPoints = sketch.Count; i < numPoints; i++) {
                TP p = sketch[i];
                if (p.IsDown) {
                    float y = wall.height - p.y;
                    float skew = y / 1900f * 525f;
                    float x = (p.x + skew) * xScale * overallScale + xOffset;
                    y = y * overallScale + yOffset;
                    sketch[i] = new TP(x, y, p.IsNull);
                }
            }

			state = State.Streaming;

			StreamState onStreamCompleted = null;
			onStreamCompleted = () => {
				Logger.Instance.WriteLog("Scenario: StreamCompleted");

				streamer.StreamCompleted -= onStreamCompleted;
				ik.Stop();

				state = State.Idle;
			};

			StreamState onCalibrationCompleted = null;
			onCalibrationCompleted = () => {
				Logger.Instance.WriteLog("Scenario: CalibrationCompleted");

				streamer.CalibrationCompleted -= onCalibrationCompleted;
				streamer.StreamCompleted += onStreamCompleted;

				int job = -1;
                if (Config.stream) {
                    job = ik.Convert(sketch, (int xsteps, int ssteps, int esteps, int wrist) => {
                        Logger.Instance.SilentLog("{3}: x[{0}] s[{1}] e[{2}]", xsteps, ssteps, esteps, wrist, job);
                        if (xsteps < 0 || ssteps < 0 || esteps < 0) {
                            Logger.Instance.StreamLog("!!!");
                        }
						streamer.Stream(xsteps, ssteps, esteps, wrist);
					});
					Logger.Instance.WriteLog("Scenario: Started IK conversion job: {0}", job);
				}
				else {
					System.Threading.Thread.Sleep(4000);
					streamer.Stream(0, 0, 0, 0);
				}
			};

			Connection onConnectionEstablished = null;
			onConnectionEstablished = () => {
				Logger.Instance.WriteLog("Scenario: ConnectionEstablished");

                streamer.Resume();

                streamer.ConnectionEstablished -= onConnectionEstablished;
				streamer.CalibrationCompleted += onCalibrationCompleted;
				streamer.Calibrate();
            };

			streamer.ConnectionEstablished += onConnectionEstablished;
			streamer.Open();
		}

		private void OnIKConversionCompleted(int jobID) {
			Logger.Instance.WriteLog("Scenario: Completed IK conversion job: {0}", jobID);
		}

		private void Update() {

			switch (state) {
				case State.Idle:

					break;
			}
		}
	}
}
