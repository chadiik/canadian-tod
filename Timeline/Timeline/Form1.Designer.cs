namespace Timeline {
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
			this.mainTabControl = new System.Windows.Forms.TabControl();
			this.visionTab = new System.Windows.Forms.TabPage();
			this.processGallery = new System.Windows.Forms.FlowLayoutPanel();
			this.galleryItemTemplate = new Emgu.CV.UI.ImageBox();
			this.faceRecognition = new Emgu.CV.UI.ImageBox();
			this.candidateImage = new Emgu.CV.UI.ImageBox();
			this.sourceImage = new Emgu.CV.UI.ImageBox();
			this.debugPreview = new Emgu.CV.UI.ImageBox();
			this.log = new System.Windows.Forms.TextBox();
			this.mainTabControl.SuspendLayout();
			this.visionTab.SuspendLayout();
			this.processGallery.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.galleryItemTemplate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.faceRecognition)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.candidateImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sourceImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.debugPreview)).BeginInit();
			this.SuspendLayout();
			// 
			// mainTabControl
			// 
			this.mainTabControl.Controls.Add(this.visionTab);
			this.mainTabControl.Location = new System.Drawing.Point(375, 10);
			this.mainTabControl.Margin = new System.Windows.Forms.Padding(0);
			this.mainTabControl.Name = "mainTabControl";
			this.mainTabControl.SelectedIndex = 0;
			this.mainTabControl.Size = new System.Drawing.Size(800, 622);
			this.mainTabControl.TabIndex = 2;
			// 
			// visionTab
			// 
			this.visionTab.Controls.Add(this.processGallery);
			this.visionTab.Controls.Add(this.faceRecognition);
			this.visionTab.Controls.Add(this.candidateImage);
			this.visionTab.Controls.Add(this.sourceImage);
			this.visionTab.Location = new System.Drawing.Point(4, 22);
			this.visionTab.Name = "visionTab";
			this.visionTab.Padding = new System.Windows.Forms.Padding(3);
			this.visionTab.Size = new System.Drawing.Size(792, 596);
			this.visionTab.TabIndex = 0;
			this.visionTab.UseVisualStyleBackColor = true;
			// 
			// processGallery
			// 
			this.processGallery.AutoScroll = true;
			this.processGallery.Controls.Add(this.galleryItemTemplate);
			this.processGallery.Location = new System.Drawing.Point(6, 319);
			this.processGallery.Name = "processGallery";
			this.processGallery.Size = new System.Drawing.Size(780, 274);
			this.processGallery.TabIndex = 15;
			this.processGallery.WrapContents = false;
			// 
			// galleryItemTemplate
			// 
			this.galleryItemTemplate.Enabled = false;
			this.galleryItemTemplate.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.galleryItemTemplate.Location = new System.Drawing.Point(1, 1);
			this.galleryItemTemplate.Margin = new System.Windows.Forms.Padding(1);
			this.galleryItemTemplate.Name = "galleryItemTemplate";
			this.galleryItemTemplate.Size = new System.Drawing.Size(209, 270);
			this.galleryItemTemplate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.galleryItemTemplate.TabIndex = 14;
			this.galleryItemTemplate.TabStop = false;
			this.galleryItemTemplate.Visible = false;
			// 
			// faceRecognition
			// 
			this.faceRecognition.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
			this.faceRecognition.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.faceRecognition.Location = new System.Drawing.Point(404, 6);
			this.faceRecognition.Name = "faceRecognition";
			this.faceRecognition.Size = new System.Drawing.Size(183, 307);
			this.faceRecognition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.faceRecognition.TabIndex = 4;
			this.faceRecognition.TabStop = false;
			// 
			// candidateImage
			// 
			this.candidateImage.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
			this.candidateImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.candidateImage.Location = new System.Drawing.Point(603, 6);
			this.candidateImage.Name = "candidateImage";
			this.candidateImage.Size = new System.Drawing.Size(183, 307);
			this.candidateImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.candidateImage.TabIndex = 3;
			this.candidateImage.TabStop = false;
			// 
			// sourceImage
			// 
			this.sourceImage.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
			this.sourceImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.sourceImage.Location = new System.Drawing.Point(6, 6);
			this.sourceImage.Name = "sourceImage";
			this.sourceImage.Size = new System.Drawing.Size(381, 307);
			this.sourceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.sourceImage.TabIndex = 2;
			this.sourceImage.TabStop = false;
			// 
			// debugPreview
			// 
			this.debugPreview.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.debugPreview.Location = new System.Drawing.Point(291, 10);
			this.debugPreview.Name = "debugPreview";
			this.debugPreview.Size = new System.Drawing.Size(81, 99);
			this.debugPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.debugPreview.TabIndex = 5;
			this.debugPreview.TabStop = false;
			// 
			// log
			// 
			this.log.AcceptsReturn = true;
			this.log.Location = new System.Drawing.Point(12, 10);
			this.log.Multiline = true;
			this.log.Name = "log";
			this.log.Size = new System.Drawing.Size(360, 622);
			this.log.TabIndex = 6;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1184, 641);
			this.Controls.Add(this.debugPreview);
			this.Controls.Add(this.log);
			this.Controls.Add(this.mainTabControl);
			this.Name = "Form1";
			this.Text = "The Obsessive Drafter 3.0";
			this.mainTabControl.ResumeLayout(false);
			this.visionTab.ResumeLayout(false);
			this.processGallery.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.galleryItemTemplate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.faceRecognition)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.candidateImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sourceImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.debugPreview)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl mainTabControl;
		private System.Windows.Forms.TabPage visionTab;
		private Emgu.CV.UI.ImageBox sourceImage;
		private Emgu.CV.UI.ImageBox candidateImage;
		private Emgu.CV.UI.ImageBox faceRecognition;
		private Emgu.CV.UI.ImageBox debugPreview;
		private System.Windows.Forms.FlowLayoutPanel processGallery;
		public Emgu.CV.UI.ImageBox galleryItemTemplate;
		private System.Windows.Forms.TextBox log;
	}
}

