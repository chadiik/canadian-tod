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
			this.tabPage1 = new System.Windows.Forms.TabPage();
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
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.sourceImage = new Emgu.CV.UI.ImageBox();
			this.resultImage = new Emgu.CV.UI.ImageBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.threshold)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gapBetweenLines)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.minLineWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.thetaResolution)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rhoResolution)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cannyThresholdLinking)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cannyThreshold)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sourceImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.resultImage)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(1023, 627);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.resultImage);
			this.tabPage1.Controls.Add(this.sourceImage);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.threshold);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.gapBetweenLines);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.minLineWidth);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.thetaResolution);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.rhoResolution);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.cannyThresholdLinking);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.cannyThreshold);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(1015, 601);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "HoughLines";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 166);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(50, 13);
			this.label7.TabIndex = 13;
			this.label7.Text = "threshold";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// threshold
			// 
			this.threshold.Location = new System.Drawing.Point(153, 162);
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
			this.label6.Location = new System.Drawing.Point(6, 140);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(92, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "gapBetweenLines";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// gapBetweenLines
			// 
			this.gapBetweenLines.Location = new System.Drawing.Point(153, 136);
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
			this.label5.Location = new System.Drawing.Point(6, 114);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(71, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "minLineWidth";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// minLineWidth
			// 
			this.minLineWidth.Location = new System.Drawing.Point(153, 110);
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
			this.label4.Location = new System.Drawing.Point(6, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(81, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "thetaResolution";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// thetaResolution
			// 
			this.thetaResolution.Location = new System.Drawing.Point(153, 84);
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
			this.label3.Location = new System.Drawing.Point(6, 62);
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
			this.rhoResolution.Location = new System.Drawing.Point(153, 58);
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
			this.label2.Location = new System.Drawing.Point(6, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(117, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "cannyThresholdLinking";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cannyThresholdLinking
			// 
			this.cannyThresholdLinking.Location = new System.Drawing.Point(153, 32);
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
			this.label1.Location = new System.Drawing.Point(6, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "cannyThreshold";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cannyThreshold
			// 
			this.cannyThreshold.Location = new System.Drawing.Point(153, 6);
			this.cannyThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.cannyThreshold.Name = "cannyThreshold";
			this.cannyThreshold.Size = new System.Drawing.Size(120, 20);
			this.cannyThreshold.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(1015, 601);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// sourceImage
			// 
			this.sourceImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.sourceImage.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
			this.sourceImage.InitialImage = null;
			this.sourceImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.sourceImage.Location = new System.Drawing.Point(744, 6);
			this.sourceImage.Name = "sourceImage";
			this.sourceImage.Size = new System.Drawing.Size(265, 327);
			this.sourceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.sourceImage.TabIndex = 14;
			this.sourceImage.TabStop = false;
			// 
			// resultImage
			// 
			this.resultImage.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
			this.resultImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.resultImage.Location = new System.Drawing.Point(473, 6);
			this.resultImage.Name = "resultImage";
			this.resultImage.Size = new System.Drawing.Size(265, 327);
			this.resultImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.resultImage.TabIndex = 15;
			this.resultImage.TabStop = false;
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
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.threshold)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gapBetweenLines)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.minLineWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.thetaResolution)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rhoResolution)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cannyThresholdLinking)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cannyThreshold)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sourceImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.resultImage)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
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
	}
}

