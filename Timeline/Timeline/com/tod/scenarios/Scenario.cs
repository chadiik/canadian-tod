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

		public Scenario(Wall wall, State state = State.Idle) {

			this.wall = wall;
			this.state = state;

			vision = new Vision( Config.debug ? Source.VideoFile : Source.Camera);
			vision.CandidateFound += OnCandidateFound;

			ik = new IK();
			ik.ConversionCompleted += OnIKConversionCompleted;

			streamer = new CanadianStreamer(!Config.stream || Config.debug);
			streamer.ConnectionFailed += () => Logger.Instance.WriteLog("Scenario: CONNECTION TO CANADIAN STREAMER FAILED!");
			streamer.PacketSent += (total) => JSON.Save(new SketchJobProcessed(total), "sketchJobProcessed");
		}

		public void Resume(SketchJob job, int processed) {

			List<Coo> sketch = job.GetUnprocessedSketch(processed);
			if(sketch != null) {
				Config.cell = job.cell + 1;
				StreamSketch(sketch, job.cell);
			}
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
		private void OnSketchCompleted(List<Line> path) {

			Sketch.SketchCompleted -= OnSketchCompleted;
			if (Config.time.ElapsedMillis - m_LastCompletedOn < 100)
				return;

			m_LastCompletedOn = Config.time.ElapsedMillis;

			Line square = new Line();
			square.Add(new Coo(.1f, .1f, false));
			square.Add(new Coo(.9f, .1f, false));
			square.Add(new Coo(.9f, .9f, false));
			square.Add(new Coo(.1f, .9f, false));
			square.Add(new Coo(.1f, .1f, false));

			if (false) {

				path.Clear();
				for (int i = 0; i < 300; i++)
					path.Add(square);
			}

			int cell = (Config.cell++) % wall.UnitCells.Count;
			List<Line> sketch = wall.ToCell(path, cell);
			switch (Config.version) {
				case Sketch.Version.Legacy:
					sketch = Sketch.Optimize(sketch, 4, 100);
					break;

				case Sketch.Version.Hatch:
					//sketch = Sketch.Optimize(sketch, 0, 200);
					//tod.sketch.hatch.Hatch.SketchPreview(sketch.ToList(), new Image<Bgr, byte>(wall.width, wall.height, new Bgr(255, 255, 255)), new MCvScalar(0), 1);
					break;
			}

			StreamSketch(Line.Merge(sketch), cell);
		}

		private void StreamSketch(List<Coo> sketch, int cell) {

			JSON.Save(SketchJob.Create(sketch, cell), "sketchJob");
			JSON.Save(new SketchJobProcessed(0), "sketchJobProcessed");

			Line square = new Line();
			square.Add(new Coo(.1f, .1f, false));
			square.Add(new Coo(.9f, .1f, false));
			square.Add(new Coo(.9f, .9f, false));
			square.Add(new Coo(.1f, .9f, false));
			square.Add(new Coo(.1f, .1f, false));

			Console.WriteLine("square: {0}", square);

			if (true) {
				Line tpSquare = wall.ToCell(square, cell);
				sketch.InsertRange(0, tpSquare.path);
			}

			Sketch.DrawToWall(new List<Line> { new Line ( sketch ) });

            int xOffset = Config.xOffset;
            int yOffset = Config.yOffset;
            float xScale = (float)Config.xScale;
            float yScale = (float)Config.yScale;
            float overallScale = 1f;
            float skewYMax = (float)Config.skewYMax;
            float skewXOffset = (float)Config.skewXOffset;

            Func<float, float, bool, Coo> transform = (px, py, down) => {
                float y = wall.height - py;
                float skew = y / skewYMax * skewXOffset;
                float x = (px + skew) * xScale * overallScale + xOffset;
                y = y * yScale * overallScale + yOffset;
                return new Coo(x, y, down);
            };

            for (int i = 0, numPoints = sketch.Count; i < numPoints; i++) {
                Coo p = sketch[i];
                sketch[i] = transform(p.x, p.y, p.down);
            }

            Coo idleCoo = transform(Config.idleX, Config.idleY, false);
            for (int i = 0; i < 200; i++) {
                sketch.Add(idleCoo);
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
					System.Threading.Thread.Sleep(100);
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
	}
}
