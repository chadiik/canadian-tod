using com.tod;
using com.tod.canvas;
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

namespace Playground {
	public partial class Form1 : Form {

		public LinesExtraction.HoughParameters hp;
		public LinesExtraction.EdgesParameters ep;
		public Wall.Parameters wp;

		public Form1() {
			InitializeComponent();

			HoughSetup();
			EdgesSetup();
			WallSetup();

			tabControl1.SelectTab(wallTab);
		}

		#region wall
		private void WallSetup() {

			if (!JSON.Load("wall", out wp))
				wp = new Wall.Parameters();

			saveWallButton.Click += (object sender, EventArgs e) => {
				JSON.Save(wp, wallFilename.Text);
			};

			wallWidth.Value = wp.width;
			wallHeight.Value = wp.height;
			numCells.Value = wp.cells;

			wallWidth.ValueChanged += OnWallParameterChanged;
			wallHeight.ValueChanged += OnWallParameterChanged;
			numCells.ValueChanged += OnWallParameterChanged;

			OnWallParameterChanged(null, null);
		}

		private void OnWallParameterChanged(object sender, EventArgs e) {

			wp.width = (int)wallWidth.Value;
			wp.height = (int)wallHeight.Value;
			wp.cells = (int)numCells.Value;

			Wall wall = new Wall(wp);
			wallPreviewImage.Image = wall.Visualize();
		}
		#endregion

		#region edges
		private void EdgesSetup() {

			if (!JSON.Load("hough-edges", out hp))
				hp = new LinesExtraction.HoughParameters();

			if (!JSON.Load("edges", out ep))
				ep = new LinesExtraction.EdgesParameters();

			saveEdgesButton.Click += (object sender, EventArgs e) => {
				JSON.Save(ep, edgesParameters.Text);
				JSON.Save(hp, "hough-" + edgesParameters.Text);
			};

			sobelOrder.Value = ep.sobelOrder;
			sobelAperture.Value = ep.sobelAperture;
			edgesThreshold.Value = ep.threshold;
			erosion.Value = ep.erosion;

			sobelOrder.ValueChanged += OnEdgesParameterChanged;
			sobelAperture.ValueChanged += OnEdgesParameterChanged;
			edgesThreshold.ValueChanged += OnEdgesParameterChanged;
			erosion.ValueChanged += OnEdgesParameterChanged;

			edgesSourceImage.Image = new Image<Bgr, byte>("assets/faces1.png");

			OnEdgesParameterChanged(null, null);
		}

		private void OnEdgesParameterChanged(object sender, EventArgs e) {

			var parameters = ep;
			parameters.sobelOrder = (int)sobelOrder.Value;
			parameters.sobelAperture = (int)sobelAperture.Value;
			parameters.threshold = (int)edgesThreshold.Value;
			parameters.erosion = (int)erosion.Value;

			Image<Bgr, byte> source = new Image<Bgr, byte>(edgesSourceImage.Image.Bitmap);
			Image<Bgr, byte> filtered;
			List<List<Point>> lines = LinesExtraction.Sobel(source, parameters, hp, out filtered);
			edgesFilteredImage.Image = filtered;
			numEdgesLines.Text = string.Format("{0} x hough lines", lines.Count);

			Image<Bgr, byte> linesPreview = new Image<Bgr, byte>(source.Size);
			LinesExtraction.Visualize(lines, linesPreview, 1);

			edgesResultImage.Image = linesPreview;
		}
		#endregion

		#region hough
		private void HoughSetup() {

			if(!JSON.Load("hough", out hp))
				hp = new LinesExtraction.HoughParameters();

			saveButton.Click += (object sender, EventArgs e) => {
				JSON.Save(hp, houghParameters.Text);
			};

			cannyThreshold.Value = (decimal)hp.cannyThreshold;
			cannyThresholdLinking.Value = (decimal)hp.cannyThresholdLinking;
			rhoResolution.Value = (decimal)hp.rhoResolution;
			thetaResolution.Value = (decimal)(hp.thetaResolution * 180.0 / Math.PI);
			threshold.Value = hp.threshold;
			minLineWidth.Value = (decimal)hp.minLineWidth;
			gapBetweenLines.Value = (decimal)hp.gapBetweenLines;

			blurSize.Value = hp.blurSize;
			spatialRadius.Value = (decimal)hp.spatialRadius;
			colorRadius.Value = (decimal)hp.colorRadius;
			maxLevel.Value = hp.maxLevel;
			termIterations.Value = hp.termIterations;
			termEpsilon.Value = (decimal)hp.termEpsilon;
			addWeightAlpha.Value = (decimal)hp.addWeightAlpha;
			addWeightBeta.Value = (decimal)hp.addWeightBeta;
			addWeightScalar.Value = (decimal)hp.addWeightScalar;

			cannyThreshold.ValueChanged += OnHoughParameterChanged;
			cannyThresholdLinking.ValueChanged += OnHoughParameterChanged;
			rhoResolution.ValueChanged += OnHoughParameterChanged;
			thetaResolution.ValueChanged += OnHoughParameterChanged;
			threshold.ValueChanged += OnHoughParameterChanged;
			minLineWidth.ValueChanged += OnHoughParameterChanged;
			gapBetweenLines.ValueChanged += OnHoughParameterChanged;

			blurSize.ValueChanged += OnHoughParameterChanged;
			spatialRadius.ValueChanged += OnHoughParameterChanged;
			colorRadius.ValueChanged += OnHoughParameterChanged;
			maxLevel.ValueChanged += OnHoughParameterChanged;
			termIterations.ValueChanged += OnHoughParameterChanged;
			termEpsilon.ValueChanged += OnHoughParameterChanged;
			addWeightAlpha.ValueChanged += OnHoughParameterChanged;
			addWeightBeta.ValueChanged += OnHoughParameterChanged;
			addWeightScalar.ValueChanged += OnHoughParameterChanged;

			sourceImage.Image = new Image<Bgr, byte>("assets/faces1.png");

			OnHoughParameterChanged(null, null);
		}

		private void OnHoughParameterChanged(object sender, EventArgs e) {

			var parameters = hp;
			parameters.cannyThreshold = (double)cannyThreshold.Value;
			parameters.cannyThresholdLinking = (double)cannyThresholdLinking.Value;
			parameters.rhoResolution = (double)rhoResolution.Value;
			parameters.thetaResolution = (double)thetaResolution.Value * Math.PI / 180.0;
			parameters.threshold = (int)threshold.Value;
			parameters.minLineWidth = (double)minLineWidth.Value;
			parameters.gapBetweenLines = (double)gapBetweenLines.Value;

			parameters.blurSize = (int)blurSize.Value;
			parameters.spatialRadius = (double)spatialRadius.Value;
			parameters.colorRadius = (double)colorRadius.Value;
			parameters.maxLevel = (int)maxLevel.Value;
			parameters.termIterations = (int)termIterations.Value;
			parameters.termEpsilon = (double)termEpsilon.Value;
			parameters.addWeightAlpha = (double)addWeightAlpha.Value;
			parameters.addWeightBeta = (double)addWeightBeta.Value;
			parameters.addWeightScalar = (double)addWeightScalar.Value;

			Image<Bgr, byte> source = new Image<Bgr, byte>(sourceImage.Image.Bitmap);
			Image<Bgr, byte> filtered;
			List<List<Point>> lines = LinesExtraction.Hough(source, parameters, out filtered);
			filteredImage.Image = filtered;
			numLines.Text = string.Format("{0} x hough lines", lines.Count);

			Image<Bgr, byte> linesPreview = new Image<Bgr, byte>(source.Size);
			LinesExtraction.Visualize(lines, linesPreview, 1);

			resultImage.Image = linesPreview;
		}
		#endregion
	}
}
