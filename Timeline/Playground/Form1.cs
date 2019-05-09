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

		public Form1() {
			InitializeComponent();

			hp = new LinesExtraction.HoughParameters();

			cannyThreshold.Value = (decimal)hp.cannyThreshold;
			cannyThresholdLinking.Value = (decimal)hp.cannyThresholdLinking;
			rhoResolution.Value = (decimal)hp.rhoResolution;
			thetaResolution.Value = (decimal)hp.thetaResolution;
			threshold.Value = hp.threshold;
			minLineWidth.Value = (decimal)hp.minLineWidth;
			gapBetweenLines.Value = (decimal)hp.gapBetweenLines;

			cannyThreshold.ValueChanged += OnParameterValueChanged;
			cannyThresholdLinking.ValueChanged += OnParameterValueChanged;
			rhoResolution.ValueChanged += OnParameterValueChanged;
			thetaResolution.ValueChanged += OnParameterValueChanged;
			threshold.ValueChanged += OnParameterValueChanged;
			minLineWidth.ValueChanged += OnParameterValueChanged;
			gapBetweenLines.ValueChanged += OnParameterValueChanged;

			sourceImage.Image = new Image<Bgr, byte>("assets/faces0.png");

			OnParameterValueChanged(null, null);
		}

		private void OnParameterValueChanged(object sender, EventArgs e) {

			var parameters = hp;
			parameters.cannyThreshold = (double)cannyThreshold.Value;
			parameters.cannyThresholdLinking = (double)cannyThresholdLinking.Value;
			parameters.rhoResolution = (double)rhoResolution.Value;
			parameters.thetaResolution = (double)thetaResolution.Value * Math.PI / 180.0;
			parameters.threshold = (int)threshold.Value;
			parameters.minLineWidth = (double)minLineWidth.Value;
			parameters.gapBetweenLines = (double)gapBetweenLines.Value;

			Image<Bgr, byte> source = new Image<Bgr, byte>(sourceImage.Image.Bitmap);
			var lines = LinesExtraction.From(source, parameters);

			MCvScalar lineColor = new MCvScalar(255, 255, 255);
			Image<Bgr, byte> linesPreview = new Image<Bgr, byte>(source.Size);
			foreach (LineSegment2D[] list in lines)
				foreach (LineSegment2D line in list)
					CvInvoke.Line(linesPreview, line.P1, line.P2, lineColor, 1);

			resultImage.Image = linesPreview;
		}
	}
}
