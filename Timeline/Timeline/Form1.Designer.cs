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
			this.wallPreviewImage = new Emgu.CV.UI.ImageBox();
			this.processGallery = new System.Windows.Forms.FlowLayoutPanel();
			this.galleryItemTemplate = new Emgu.CV.UI.ImageBox();
			this.debugPreview = new Emgu.CV.UI.ImageBox();
			this.candidateImage = new Emgu.CV.UI.ImageBox();
			this.sourceImage = new Emgu.CV.UI.ImageBox();
			this.faceRecognition = new Emgu.CV.UI.ImageBox();
			this.log = new System.Windows.Forms.TextBox();
			this.streamLog = new System.Windows.Forms.TextBox();
			this.mainTabControl.SuspendLayout();
			this.visionTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.wallPreviewImage)).BeginInit();
			this.processGallery.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.galleryItemTemplate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.debugPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.candidateImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sourceImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.faceRecognition)).BeginInit();
			this.SuspendLayout();
			// 
			// mainTabControl
			// 
			this.mainTabControl.Controls.Add(this.visionTab);
			this.mainTabControl.Location = new System.Drawing.Point(324, 10);
			this.mainTabControl.Margin = new System.Windows.Forms.Padding(0);
			this.mainTabControl.Name = "mainTabControl";
			this.mainTabControl.SelectedIndex = 0;
			this.mainTabControl.Size = new System.Drawing.Size(615, 427);
			this.mainTabControl.TabIndex = 2;
			// 
			// visionTab
			// 
			this.visionTab.Controls.Add(this.wallPreviewImage);
			this.visionTab.Controls.Add(this.processGallery);
			this.visionTab.Controls.Add(this.debugPreview);
			this.visionTab.Controls.Add(this.candidateImage);
			this.visionTab.Controls.Add(this.sourceImage);
			this.visionTab.Location = new System.Drawing.Point(4, 22);
			this.visionTab.Name = "visionTab";
			this.visionTab.Padding = new System.Windows.Forms.Padding(3);
			this.visionTab.Size = new System.Drawing.Size(607, 401);
			this.visionTab.TabIndex = 0;
			this.visionTab.UseVisualStyleBackColor = true;
			// 
			// wallPreviewImage
			// 
			this.wallPreviewImage.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
			this.wallPreviewImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.wallPreviewImage.Location = new System.Drawing.Point(240, 6);
			this.wallPreviewImage.Name = "wallPreviewImage";
			this.wallPreviewImage.Size = new System.Drawing.Size(204, 126);
			this.wallPreviewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.wallPreviewImage.TabIndex = 16;
			this.wallPreviewImage.TabStop = false;
			// 
			// processGallery
			// 
			this.processGallery.AutoScroll = true;
			this.processGallery.Controls.Add(this.galleryItemTemplate);
			this.processGallery.Location = new System.Drawing.Point(6, 138);
			this.processGallery.Name = "processGallery";
			this.processGallery.Size = new System.Drawing.Size(461, 260);
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
			this.galleryItemTemplate.Size = new System.Drawing.Size(161, 258);
			this.galleryItemTemplate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.galleryItemTemplate.TabIndex = 14;
			this.galleryItemTemplate.TabStop = false;
			this.galleryItemTemplate.Visible = false;
			// 
			// debugPreview
			// 
			this.debugPreview.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.debugPreview.Location = new System.Drawing.Point(477, 139);
			this.debugPreview.Name = "debugPreview";
			this.debugPreview.Size = new System.Drawing.Size(104, 117);
			this.debugPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.debugPreview.TabIndex = 5;
			this.debugPreview.TabStop = false;
			// 
			// candidateImage
			// 
			this.candidateImage.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
			this.candidateImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.candidateImage.Location = new System.Drawing.Point(477, 6);
			this.candidateImage.Name = "candidateImage";
			this.candidateImage.Size = new System.Drawing.Size(101, 126);
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
			this.sourceImage.Size = new System.Drawing.Size(169, 126);
			this.sourceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.sourceImage.TabIndex = 2;
			this.sourceImage.TabStop = false;
			// 
			// faceRecognition
			// 
			this.faceRecognition.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.RightClickMenu;
			this.faceRecognition.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
			this.faceRecognition.Location = new System.Drawing.Point(217, 38);
			this.faceRecognition.Name = "faceRecognition";
			this.faceRecognition.Size = new System.Drawing.Size(101, 126);
			this.faceRecognition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.faceRecognition.TabIndex = 4;
			this.faceRecognition.TabStop = false;
			// 
			// log
			// 
			this.log.AcceptsReturn = true;
			this.log.Location = new System.Drawing.Point(12, 10);
			this.log.Multiline = true;
			this.log.Name = "log";
			this.log.Size = new System.Drawing.Size(199, 427);
			this.log.TabIndex = 6;
			// 
			// streamLog
			// 
			this.streamLog.AcceptsReturn = true;
			this.streamLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
			this.streamLog.Location = new System.Drawing.Point(217, 170);
			this.streamLog.Multiline = true;
			this.streamLog.Name = "streamLog";
			this.streamLog.Size = new System.Drawing.Size(104, 260);
			this.streamLog.TabIndex = 7;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 442);
			this.Controls.Add(this.streamLog);
			this.Controls.Add(this.log);
			this.Controls.Add(this.faceRecognition);
			this.Controls.Add(this.mainTabControl);
			this.Name = "Form1";
			this.Text = "The Obsessive Drafter 3.0";
			this.mainTabControl.ResumeLayout(false);
			this.visionTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.wallPreviewImage)).EndInit();
			this.processGallery.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.galleryItemTemplate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.debugPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.candidateImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sourceImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.faceRecognition)).EndInit();
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
        private System.Windows.Forms.TextBox streamLog;
        private Emgu.CV.UI.ImageBox wallPreviewImage;
    }
}

