using com.tod;
using com.tod.canvas;
using com.tod.core;
using com.tod.ik;
using com.tod.scenarios;
using com.tod.sketch;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Timeline {
	public partial class Form1 : Form {

		private GUI m_GUI;
		private Scenario m_Scenario;

		public Form1() {
			InitializeComponent();
			FormClosed += OnFormClosed;

			Config.fps = 4;
			Config.files.ikJobsDir = "../../../../IK Solver/simpleIK_sharp/bin/Debug/";
			Config.files.ikProcess = "../../../../IK Solver/simpleIK_sharp/bin/Debug/simpleIK_sharp.exe";
			Config.files.assetsDir = "assets";
			Config.files.focusMap = "assets/focus.jpg";
			Config.files.eyesHC = "haarcascade_eye.xml";
			Config.files.facesHC = "haarcascade_frontalface_default.xml";
			Config.files.video0 = "videos/" + new string[] { "faces.mp4" }[0];

			Start();
		}

		private void Debug() {

			List<TP> path = new List<TP>() {
				new TP(-1, -1),
				new TP(.5f, .5f),
				new TP(1f, .5f),
				new TP(-1, -1),
				new TP(.5f, 1f),
				new TP(-1, -1)
			};

			Canvas canvas = Config.canvas;
			List<TP> sizedPath = canvas.ToCell(path);

			IK ik = new IK();
			ik.Convert(sizedPath, (int xsteps, int ssteps, int esteps, int wrist) => {
				Logger.Instance.SilentLog("Debug: x[{0}] s[{1}] e[{2}] wrist[{3}]", xsteps, ssteps, esteps, wrist);
			});
		}

		private void Start() {

			m_GUI = new GUI() {
				log = new ManagedTextBox(log),
				sourceImage = new Image(sourceImage),
				candidateImage = new Image(candidateImage),
				faceRecognition = new Image(faceRecognition),
				debugPreview = new Image(debugPreview),
				processGallery = new ImageGallery(processGallery, new Image(galleryItemTemplate))
			};

			m_Scenario = new Scenario(Config.canvas);
			m_Scenario.vision.SourceUpdated += (Mat image) => m_GUI.sourceImage.Source = image;
			m_Scenario.vision.FaceDetected += (Mat image) => m_GUI.faceRecognition.Source = image;
			//m_Scenario.vision.CandidateFound += (Portrait portrait) => m_GUI.debugPreview.Source = portrait.source.Mat.ToImage<Bgr, byte>();

			Sketch.DisplayEntryRequested += (IImage image) => {
				m_GUI.SetProcessImage(image);
				//m_GUI.debugPreview.Source = image;
			};

			//m_Scenario.sketch.Test("c.jpg");
		}

		private void OnFormClosed(object sender, FormClosedEventArgs e) {
			try {
				m_Scenario.Stop();
			}
			catch (Exception) { }
		}
	}
}
