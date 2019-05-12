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
			ContextMenu = CreateContextMenu();

			Config.fps = 4;
			Config.files.ikJobsDir = "../../../../IK Solver/simpleIK_sharp/bin/Debug/";
			Config.files.ikProcess = "../../../../IK Solver/simpleIK_sharp/bin/Debug/simpleIK_sharp.exe";
			Config.files.assetsDir = "assets";
			Config.files.focusMap = "assets/focus.jpg";
			Config.files.eyesHC = "haarcascade_eye.xml";
			Config.files.facesHC = "haarcascade_frontalface_default.xml";
			//Config.files.video0 = "videos/" + new string[] { "faces.mp4" }[0];

			Start();
		}

		private ContextMenu CreateContextMenu() {
			ContextMenu cm = new ContextMenu();
			cm.MenuItems.Add("Pause", new EventHandler((object sender, EventArgs e) => m_Scenario.Pause()));
			cm.MenuItems.Add("Resume", new EventHandler((object sender, EventArgs e) => m_Scenario.Resume()));
			cm.MenuItems[1].Enabled = false;
			return cm;
		}

		private void Start() {

			m_GUI = new GUI() {
				log = new ManagedTextBox(log),
                streamLog = new ManagedTextBox(streamLog),
                sourceImage = new Image(sourceImage),
				candidateImage = new Image(candidateImage),
				faceRecognition = new Image(faceRecognition),
				debugPreview = new Image(debugPreview),
                wallPreview = new Image(wallPreviewImage),
				processGallery = new ImageGallery(processGallery, new Image(galleryItemTemplate))
			};

			Wall.Parameters wp;
			if (!JSON.Load(Config.wallConfig, out wp))
				wp = new Wall.Parameters();

            Wall wall = new Wall(wp);
            m_Scenario = new Scenario(wall);
			m_Scenario.vision.SourceUpdated += (Mat image) => m_GUI.sourceImage.Source = image;
			m_Scenario.vision.FaceDetected += (Mat image) => m_GUI.faceRecognition.Source = image;
			//m_Scenario.vision.CandidateFound += (Portrait portrait) => m_GUI.debugPreview.Source = portrait.source.Mat.ToImage<Bgr, byte>();

			Sketch.DisplayEntryRequested += (IImage image) => {
				m_GUI.SetProcessImage(image);
				//m_GUI.debugPreview.Source = image;
			};

            Image<Bgr, byte> wallPreview = wall.Visualize(false);
            m_GUI.wallPreview.Source = wallPreview;
            Sketch.DrawToWallRequested += (List<TP> path) => {
                List<TP> scaledPath = wall.Fit(wallPreview.Width, wallPreview.Height, path);
                TP.Visualize(scaledPath, wallPreview, new MCvScalar(0), 1);
                m_GUI.wallPreview.Source = wallPreview;
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
