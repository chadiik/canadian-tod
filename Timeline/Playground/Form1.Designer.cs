namespace Playground {
	partial class Form1 {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.hough = new System.Windows.Forms.TabPage();
			this.numLines = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.addWeightScalar = new System.Windows.Forms.NumericUpDown();
			this.label15 = new System.Windows.Forms.Label();
			this.addWeightBeta = new System.Windows.Forms.NumericUpDown();
			this.label14 = new System.Windows.Forms.Label();
			this.addWeightAlpha = new System.Windows.Forms.NumericUpDown();
			this.filteredImage = new Emgu.CV.UI.ImageBox();
			this.label13 = new System.Windows.Forms.Label();
			this.termEpsilon = new System.Windows.Forms.NumericUpDown();
			this.label12 = new System.Windows.Forms.Label();
			this.termIterations = new System.Windows.Forms.NumericUpDown();
			this.label11 = new System.Windows.Forms.Label();
			this.maxLevel = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.colorRadius = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.spatialRadius = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.blurSize = new System.Windows.Forms.NumericUpDown();
			this.houghParameters = new System.Windows.Forms.TextBox();
			this.saveButton = new System.Windows.Forms.Button();
			this.resultImage = new Emgu.CV.UI.ImageBox();
			this.sourceImage = new Emgu.CV.UI.ImageBox();
			this.label7 = new System.Windows.Forms.Label();
			this.threshold = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.gapBetweenLines = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.minLineWidth = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.thetaResolution = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.rhoResolution = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.cannyThresholdLinking = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.cannyThreshold = new System.Windows.Forms.NumericUpDown();
			this.edges = new System.Windows.Forms.TabPage();
			this.numEdgesLines = new System.Windows.Forms.Label();
			this.edgesFilteredImage = new Emgu.CV.UI.ImageBox();
			this.edgesParameters = new System.Windows.Forms.TextBox();
			this.saveEdgesButton = new System.Windows.Forms.Button();
			this.edgesResultImage = new Emgu.CV.UI.ImageBox();
			this.edgesSourceImage = new Emgu.CV.UI.ImageBox();
			this.label18 = new System.Windows.Forms.Label();
			this.sobelOrder = new System.Windows.Forms.NumericUpDown();
			this.label17 = new System.Windows.Forms.Label();
			this.sobelAperture = new System.Windows.Forms.NumericUpDown();
			this.label19 = new System.Windows.Forms.Label();
			this.edgesThreshold = new System.Windows.Forms.NumericUpDown();
			this.label20 = new System.Windows.Forms.Label();
			this.erosion = new System.Windows.Forms.NumericUpDown();
			this.tabControl1.SuspendLayout();
			this.hough.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.addWeightScalar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.addWeightBeta)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.addWeightAlpha)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.filteredImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.termEpsilon)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.termIterations)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxLevel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.colorRadius)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spatialRadius)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.blurSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.resultImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sourceImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.threshold)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gapBetweenLines)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.minLineWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.thetaResolution)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rhoResolution)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cannyThresholdLinking)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cannyThreshold)).BeginInit();
			this.edges.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edgesFilteredImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edgesResultImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edgesSourceImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sobelOrder)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sobelAperture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edgesThreshold)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.erosion)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.hough);
			this.tabControl1.Controls.Add(this.edges);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(1023, 627);
			this.tabControl1.TabIndex = 0;
			// 
			// hough
			// 
			this.hough.Controls.Add(this.numLines);
			this.hough.Controls.Add(this.label16);
			this.hough.Controls.Add(this.addWeightScalar);
			this.hough.Controls.Add(this.label15);
			this.hough.Controls.Add(this.addWeightBeta);
			this.hough.Controls.Add(this.label14);
			this.hough.Controls.Add(this.addWeightAlpha);
			this.hough.Controls.Add(this.filteredImage);
			this.hough.Controls.Add(this.label13);
			this.hough.Controls.Add(this.termEpsilon);
			this.hough.Controls.Add(this.label12);
			this.hough.Controls.Add(this.termIterations);
			this.hough.Controls.Add(this.label11);
			this.hough.Controls.Add(this.maxLevel);
			this.hough.Controls.Add(this.label10);
			this.hough.Controls.Add(this.colorRadius);
			this.hough.Controls.Add(this.label9);
			this.hough.Controls.Add(this.spatialRadius);
			this.hough.Controls.Add(this.label8);
			this.hough.Controls.Add(this.blurSize);
			this.hough.Controls.Add(this.houghParameters);
			this.hough.Controls.Add(this.saveButton);
			this.hough.Controls.Add(this.resultImage);
			this.hough.Controls.Add(this.sourceImage);
			this.hough.Controls.Add(this.label7);
			this.hough.Controls.Add(this.threshold);
			this.hough.Controls.Add(this.label6);
			this.hough.Controls.Add(this.gapBetweenLines);
			this.hough.Controls.Add(this.label5);
			this.hough.Controls.Add(this.minLineWidth);
			this.hough.Controls.Add(this.label4);
			this.hough.Controls.Add(this.thetaResolution);
			this.hough.Controls.Add(this.label3);
			this.hough.Controls.Add(this.rhoResolution);
			this.hough.Controls.Add(this.label2);
			this.hough.Controls.Add(this.cannyThresholdLinking);
			this.hough.Controls.Add(this.label1);
			this.hough.Controls.Add(this.cannyThreshold);
			this.hough.Location = new System.Drawing.Point(4, 22);
			this.hough.Name = "hough";
			this.hough.Padding = new System.Windows.Forms.Padding(3);
			this.hough.Size = new System.Drawing.Size(1015, 601);
			this.hough.TabIndex = 0;
			this.hough.Text = "HoughLines";
			this.hough.UseVisualStyleBackColor = true;
			// 
			// numLines
			// 
			this.numLines.AutoSize = true;
			this.numLines.Location = new System.Drawing.Point(439, 280);
			this.numLines.Name = "numLines";
			this.numLines.Size = new System.Drawing.Size(61, 13);
			this.numLines.TabIndex = 37;
			this.numLines.Text = "hough lines";
			// 
			// label16
			// 
			this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(6, 475);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(89, 13);
			this.label16.TabIndex = 36;
			this.label16.Text = "addWeightScalar";
			this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// addWeightScalar
			// 
			this.addWeightScalar.DecimalPlaces = 1;
			this.addWeightScalar.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.addWeightScalar.Location = new System.Drawing.Point(153, 471);
			this.addWeightScalar.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.addWeightScalar.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
			this.addWeightScalar.Name = "addWeightScalar";
			this.addWeightScalar.Size = new System.Drawing.Size(120, 20);
			this.addWeightScalar.TabIndex = 35;
			this.addWeightScalar.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label15
			// 
			this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(6, 449);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(81, 13);
			this.label15.TabIndex = 34;
			this.label15.Text = "addWeightBeta";
			this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// addWeightBeta
			// 
			this.addWeightBeta.DecimalPlaces = 1;
			this.addWeightBeta.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.addWeightBeta.Location = new System.Drawing.Point(153, 445);
			this.addWeightBeta.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.addWeightBeta.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
			this.addWeightBeta.Name = "addWeightBeta";
			this.addWeightBeta.Size = new System.Drawing.Size(120, 20);
			this.addWeightBeta.TabIndex = 33;
			this.addWeightBeta.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label14
			// 
			this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(6, 423);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(86, 13);
			this.label14.TabIndex = 32;
			this.label14.Text = "addWeightAlpha";
			this.label14.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// addWeightAlpha
			// 
			this.addWeightAlpha.DecimalPlaces = 1;
			this.addWeightAlpha.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.addWeightAlpha.Location = new System.Drawing.Point(153, 419);
			this.addWeightAlpha.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.addWeightAlpha.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
			this.addWeightAlpha.Name = "addWeightAlpha";
			this.addWeightAlpha.Size = new System.Drawing.Size(120, 20);
			this.addWeightAlpha.TabIndex = 31;
			this.addWeightAlpha.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// filteredImage
			// 
			this.filteredImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.filteredImage.Location = new System.Drawing.Point(633, 6);
			this.filteredImage.Name = "filteredImage";
			this.filteredImage.Size = new System.Drawing.Size(185, 271);
			this.filteredImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.filteredImage.TabIndex = 30;
			this.filteredImage.TabStop = false;
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(6, 379);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(61, 13);
			this.label13.TabIndex = 29;
			this.label13.Text = "termEpsilon";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// termEpsilon
			// 
			this.termEpsilon.DecimalPlaces = 1;
			this.termEpsilon.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.termEpsilon.Location = new System.Drawing.Point(153, 375);
			this.termEpsilon.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.termEpsilon.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.termEpsilon.Name = "termEpsilon";
			this.termEpsilon.Size = new System.Drawing.Size(120, 20);
			this.termEpsilon.TabIndex = 28;
			this.termEpsilon.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label12
			// 
			this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(6, 353);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(70, 13);
			this.label12.TabIndex = 27;
			this.label12.Text = "termIterations";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// termIterations
			// 
			this.termIterations.DecimalPlaces = 1;
			this.termIterations.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.termIterations.Location = new System.Drawing.Point(153, 349);
			this.termIterations.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.termIterations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.termIterations.Name = "termIterations";
			this.termIterations.Size = new System.Drawing.Size(120, 20);
			this.termIterations.TabIndex = 26;
			this.termIterations.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(6, 327);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(52, 13);
			this.label11.TabIndex = 25;
			this.label11.Text = "maxLevel";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// maxLevel
			// 
			this.maxLevel.DecimalPlaces = 1;
			this.maxLevel.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.maxLevel.Location = new System.Drawing.Point(153, 323);
			this.maxLevel.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.maxLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.maxLevel.Name = "maxLevel";
			this.maxLevel.Size = new System.Drawing.Size(120, 20);
			this.maxLevel.TabIndex = 24;
			this.maxLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6, 301);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(63, 13);
			this.label10.TabIndex = 23;
			this.label10.Text = "colorRadius";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// colorRadius
			// 
			this.colorRadius.DecimalPlaces = 1;
			this.colorRadius.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.colorRadius.Location = new System.Drawing.Point(153, 297);
			this.colorRadius.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.colorRadius.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.colorRadius.Name = "colorRadius";
			this.colorRadius.Size = new System.Drawing.Size(120, 20);
			this.colorRadius.TabIndex = 22;
			this.colorRadius.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(6, 275);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(70, 13);
			this.label9.TabIndex = 21;
			this.label9.Text = "spatialRadius";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// spatialRadius
			// 
			this.spatialRadius.DecimalPlaces = 1;
			this.spatialRadius.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.spatialRadius.Location = new System.Drawing.Point(153, 271);
			this.spatialRadius.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.spatialRadius.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this.spatialRadius.Name = "spatialRadius";
			this.spatialRadius.Size = new System.Drawing.Size(120, 20);
			this.spatialRadius.TabIndex = 20;
			this.spatialRadius.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 249);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(44, 13);
			this.label8.TabIndex = 19;
			this.label8.Text = "blurSize";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// blurSize
			// 
			this.blurSize.Location = new System.Drawing.Point(153, 245);
			this.blurSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
			this.blurSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.blurSize.Name = "blurSize";
			this.blurSize.Size = new System.Drawing.Size(120, 20);
			this.blurSize.TabIndex = 18;
			this.blurSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// houghParameters
			// 
			this.houghParameters.Location = new System.Drawing.Point(153, 8);
			this.houghParameters.Name = "houghParameters";
			this.houghParameters.Size = new System.Drawing.Size(120, 20);
			this.houghParameters.TabIndex = 17;
			this.houghParameters.Text = "hough";
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(9, 6);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(75, 23);
			this.saveButton.TabIndex = 16;
			this.saveButton.Text = "SAVE";
			this.saveButton.UseVisualStyleBackColor = true;
			// 
			// resultImage
			// 
			this.resultImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.resultImage.Location = new System.Drawing.Point(442, 6);
			this.resultImage.Name = "resultImage";
			this.resultImage.Size = new System.Drawing.Size(185, 271);
			this.resultImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.resultImage.TabIndex = 15;
			this.resultImage.TabStop = false;
			// 
			// sourceImage
			// 
			this.sourceImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.sourceImage.InitialImage = null;
			this.sourceImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.sourceImage.Location = new System.Drawing.Point(824, 6);
			this.sourceImage.Name = "sourceImage";
			this.sourceImage.Size = new System.Drawing.Size(185, 271);
			this.sourceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.sourceImage.TabIndex = 14;
			this.sourceImage.TabStop = false;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 205);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(50, 13);
			this.label7.TabIndex = 13;
			this.label7.Text = "threshold";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// threshold
			// 
			this.threshold.Location = new System.Drawing.Point(153, 201);
			this.threshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.threshold.Name = "threshold";
			this.threshold.Size = new System.Drawing.Size(120, 20);
			this.threshold.TabIndex = 12;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 179);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(92, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "gapBetweenLines";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// gapBetweenLines
			// 
			this.gapBetweenLines.Location = new System.Drawing.Point(153, 175);
			this.gapBetweenLines.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.gapBetweenLines.Name = "gapBetweenLines";
			this.gapBetweenLines.Size = new System.Drawing.Size(120, 20);
			this.gapBetweenLines.TabIndex = 10;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 153);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(71, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "minLineWidth";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// minLineWidth
			// 
			this.minLineWidth.Location = new System.Drawing.Point(153, 149);
			this.minLineWidth.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.minLineWidth.Name = "minLineWidth";
			this.minLineWidth.Size = new System.Drawing.Size(120, 20);
			this.minLineWidth.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 127);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(81, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "thetaResolution";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// thetaResolution
			// 
			this.thetaResolution.DecimalPlaces = 1;
			this.thetaResolution.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.thetaResolution.Location = new System.Drawing.Point(153, 123);
			this.thetaResolution.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.thetaResolution.Name = "thetaResolution";
			this.thetaResolution.Size = new System.Drawing.Size(120, 20);
			this.thetaResolution.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 101);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "rhoResolution";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// rhoResolution
			// 
			this.rhoResolution.DecimalPlaces = 1;
			this.rhoResolution.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.rhoResolution.Location = new System.Drawing.Point(153, 97);
			this.rhoResolution.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.rhoResolution.Name = "rhoResolution";
			this.rhoResolution.Size = new System.Drawing.Size(120, 20);
			this.rhoResolution.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(117, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "cannyThresholdLinking";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cannyThresholdLinking
			// 
			this.cannyThresholdLinking.Location = new System.Drawing.Point(153, 71);
			this.cannyThresholdLinking.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.cannyThresholdLinking.Name = "cannyThresholdLinking";
			this.cannyThresholdLinking.Size = new System.Drawing.Size(120, 20);
			this.cannyThresholdLinking.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "cannyThreshold";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cannyThreshold
			// 
			this.cannyThreshold.Location = new System.Drawing.Point(153, 45);
			this.cannyThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.cannyThreshold.Name = "cannyThreshold";
			this.cannyThreshold.Size = new System.Drawing.Size(120, 20);
			this.cannyThreshold.TabIndex = 0;
			// 
			// edges
			// 
			this.edges.Controls.Add(this.label20);
			this.edges.Controls.Add(this.erosion);
			this.edges.Controls.Add(this.label19);
			this.edges.Controls.Add(this.edgesThreshold);
			this.edges.Controls.Add(this.label17);
			this.edges.Controls.Add(this.sobelAperture);
			this.edges.Controls.Add(this.numEdgesLines);
			this.edges.Controls.Add(this.edgesFilteredImage);
			this.edges.Controls.Add(this.edgesParameters);
			this.edges.Controls.Add(this.saveEdgesButton);
			this.edges.Controls.Add(this.edgesResultImage);
			this.edges.Controls.Add(this.edgesSourceImage);
			this.edges.Controls.Add(this.label18);
			this.edges.Controls.Add(this.sobelOrder);
			this.edges.Location = new System.Drawing.Point(4, 22);
			this.edges.Name = "edges";
			this.edges.Padding = new System.Windows.Forms.Padding(3);
			this.edges.Size = new System.Drawing.Size(1015, 601);
			this.edges.TabIndex = 1;
			this.edges.Text = "Edges";
			this.edges.UseVisualStyleBackColor = true;
			// 
			// numEdgesLines
			// 
			this.numEdgesLines.AutoSize = true;
			this.numEdgesLines.Location = new System.Drawing.Point(439, 280);
			this.numEdgesLines.Name = "numEdgesLines";
			this.numEdgesLines.Size = new System.Drawing.Size(60, 13);
			this.numEdgesLines.TabIndex = 45;
			this.numEdgesLines.Text = "edges lines";
			// 
			// edgesFilteredImage
			// 
			this.edgesFilteredImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.edgesFilteredImage.Location = new System.Drawing.Point(633, 6);
			this.edgesFilteredImage.Name = "edgesFilteredImage";
			this.edgesFilteredImage.Size = new System.Drawing.Size(185, 271);
			this.edgesFilteredImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.edgesFilteredImage.TabIndex = 44;
			this.edgesFilteredImage.TabStop = false;
			// 
			// edgesParameters
			// 
			this.edgesParameters.Location = new System.Drawing.Point(153, 8);
			this.edgesParameters.Name = "edgesParameters";
			this.edgesParameters.Size = new System.Drawing.Size(120, 20);
			this.edgesParameters.TabIndex = 43;
			this.edgesParameters.Text = "edges";
			// 
			// saveEdgesButton
			// 
			this.saveEdgesButton.Location = new System.Drawing.Point(9, 6);
			this.saveEdgesButton.Name = "saveEdgesButton";
			this.saveEdgesButton.Size = new System.Drawing.Size(75, 23);
			this.saveEdgesButton.TabIndex = 42;
			this.saveEdgesButton.Text = "SAVE";
			this.saveEdgesButton.UseVisualStyleBackColor = true;
			// 
			// edgesResultImage
			// 
			this.edgesResultImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.edgesResultImage.Location = new System.Drawing.Point(442, 6);
			this.edgesResultImage.Name = "edgesResultImage";
			this.edgesResultImage.Size = new System.Drawing.Size(185, 271);
			this.edgesResultImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.edgesResultImage.TabIndex = 41;
			this.edgesResultImage.TabStop = false;
			// 
			// edgesSourceImage
			// 
			this.edgesSourceImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.edgesSourceImage.InitialImage = null;
			this.edgesSourceImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.edgesSourceImage.Location = new System.Drawing.Point(824, 6);
			this.edgesSourceImage.Name = "edgesSourceImage";
			this.edgesSourceImage.Size = new System.Drawing.Size(185, 271);
			this.edgesSourceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.edgesSourceImage.TabIndex = 40;
			this.edgesSourceImage.TabStop = false;
			// 
			// label18
			// 
			this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(6, 49);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(58, 13);
			this.label18.TabIndex = 39;
			this.label18.Text = "sobelOrder";
			this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sobelOrder
			// 
			this.sobelOrder.Location = new System.Drawing.Point(153, 45);
			this.sobelOrder.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.sobelOrder.Name = "sobelOrder";
			this.sobelOrder.Size = new System.Drawing.Size(120, 20);
			this.sobelOrder.TabIndex = 38;
			// 
			// label17
			// 
			this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(6, 75);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(72, 13);
			this.label17.TabIndex = 47;
			this.label17.Text = "sobelAperture";
			this.label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sobelAperture
			// 
			this.sobelAperture.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.sobelAperture.Location = new System.Drawing.Point(153, 71);
			this.sobelAperture.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.sobelAperture.Name = "sobelAperture";
			this.sobelAperture.Size = new System.Drawing.Size(120, 20);
			this.sobelAperture.TabIndex = 46;
			this.sobelAperture.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label19
			// 
			this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(6, 101);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(83, 13);
			this.label19.TabIndex = 49;
			this.label19.Text = "edgesThreshold";
			this.label19.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// edgesThreshold
			// 
			this.edgesThreshold.Location = new System.Drawing.Point(153, 97);
			this.edgesThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.edgesThreshold.Name = "edgesThreshold";
			this.edgesThreshold.Size = new System.Drawing.Size(120, 20);
			this.edgesThreshold.TabIndex = 48;
			this.edgesThreshold.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label20
			// 
			this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(6, 127);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(41, 13);
			this.label20.TabIndex = 51;
			this.label20.Text = "erosion";
			this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// erosion
			// 
			this.erosion.Location = new System.Drawing.Point(153, 123);
			this.erosion.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.erosion.Name = "erosion";
			this.erosion.Size = new System.Drawing.Size(120, 20);
			this.erosion.TabIndex = 50;
			this.erosion.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1047, 651);
			this.Controls.Add(this.tabControl1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.tabControl1.ResumeLayout(false);
			this.hough.ResumeLayout(false);
			this.hough.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.addWeightScalar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.addWeightBeta)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.addWeightAlpha)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.filteredImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.termEpsilon)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.termIterations)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxLevel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.colorRadius)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spatialRadius)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.blurSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.resultImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sourceImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.threshold)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gapBetweenLines)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.minLineWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.thetaResolution)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rhoResolution)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cannyThresholdLinking)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cannyThreshold)).EndInit();
			this.edges.ResumeLayout(false);
			this.edges.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.edgesFilteredImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edgesResultImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edgesSourceImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sobelOrder)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sobelAperture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edgesThreshold)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.erosion)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage hough;
		private System.Windows.Forms.TabPage edges;
		private System.Windows.Forms.NumericUpDown cannyThreshold;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown cannyThresholdLinking;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown rhoResolution;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown gapBetweenLines;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown minLineWidth;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown thetaResolution;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown threshold;
		private Emgu.CV.UI.ImageBox resultImage;
		private Emgu.CV.UI.ImageBox sourceImage;
		private System.Windows.Forms.TextBox houghParameters;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown blurSize;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.NumericUpDown termEpsilon;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.NumericUpDown termIterations;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown maxLevel;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown colorRadius;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown spatialRadius;
		private Emgu.CV.UI.ImageBox filteredImage;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.NumericUpDown addWeightScalar;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.NumericUpDown addWeightBeta;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown addWeightAlpha;
		private System.Windows.Forms.Label numLines;
		private System.Windows.Forms.Label numEdgesLines;
		private Emgu.CV.UI.ImageBox edgesFilteredImage;
		private System.Windows.Forms.TextBox edgesParameters;
		private System.Windows.Forms.Button saveEdgesButton;
		private Emgu.CV.UI.ImageBox edgesResultImage;
		private Emgu.CV.UI.ImageBox edgesSourceImage;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.NumericUpDown sobelOrder;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.NumericUpDown sobelAperture;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.NumericUpDown edgesThreshold;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.NumericUpDown erosion;
	}
}

