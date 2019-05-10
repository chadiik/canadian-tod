using com.tod.canvas;
using com.tod.core;
using com.tod.ik;
using com.tod.sketch;
using com.tod.stream;
using com.tod.vision;
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
		public Canvas canvas;
		public IK ik;
		public Streamer streamer;

		public Scenario(Canvas canvas) {

			this.canvas = canvas;
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

		private void OnSketchCompleted(List<TP> path) {

			Sketch.SketchCompleted -= OnSketchCompleted;
			List<TP> sketch = canvas.ToCell(path);
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
						//Logger.Instance.SilentLog("{3}: x[{0}] s[{1}] e[{2}]", xsteps, ssteps, esteps, wrist, job);
						streamer.Stream(Math.Abs(xsteps), Math.Abs(ssteps), Math.Abs(esteps), Math.Abs(wrist));
					});
					Logger.Instance.WriteLog("Scenario: Started IK conversion job: {0}", job);
				}
				else {
					System.Threading.Thread.Sleep(5000);
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
