using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.vision {

	class FaceDetection {

		public static int FACE_MIN_N = 8;
		public static int EYE_MIN_N = 3;

		public static int MIN_FACE_SIZE = 30;
		public static float MIN_EYES_RATIO = 1f / 10f;
		public static float MAX_EYES_RATIO = 1f / 3f;

		public static int POOL_SIZE = 6;

		public event ImageEvent FaceDetected;

		public FacesPool pool;

		private int w, h, left, top;
		private Rectangle _roi;
		private UMat _temp;

		public FaceDetection(Rectangle sourceSize, float marginH, float marginV) {
			w = sourceSize.Width;
			h = sourceSize.Height;
			int wm = Math.Max(1, (int)(w * marginH));
			int hm = Math.Max(1, (int)(h * marginV));
			left = wm;
			top = hm;
			w = w - wm * 2;
			h = h - hm * 2;

			_roi = new Rectangle(left, top, w, h);

			_temp = new UMat(512, 512, Emgu.CV.CvEnum.DepthType.Default, 3);
			pool = new FacesPool(POOL_SIZE);

			face = new CascadeClassifier(Config.files.facesHC);
			eye = new CascadeClassifier(Config.files.eyesHC);

			minSize = new Size(MIN_FACE_SIZE, MIN_FACE_SIZE);
			minSizeEye = new Size(1, 1);
			maxSizeEye = new Size(100, 100);
		}

		public bool Process(Mat input, Mat image) {

			pool.sharedMat = input;
			pool.sharedMatDebug = image;

			long detectionTime;
			List<Rectangle> faces = new List<Rectangle>();
			List<Rectangle> eyes = new List<Rectangle>();

			CvInvoke.Rectangle(image, _roi, new Bgr(Color.Black).MCvScalar, 1);

			Detect(
			  image,
			  faces, eyes,
			  out detectionTime);


			pool.Eyes = eyes;
			int numFaces = faces.Count;
			if (numFaces > 0) {
				for (int i = 0; i < numFaces; i++) {
					if (_roi.Contains(faces[i])) {
						pool.Process(faces[i]);
					}
				}

				FaceDetected?.Invoke(image);
				pool.CalculateScores();
			}

			return faces.Count > 0;
		}

		#region emgu
		//----------------------------------------------------------------------------
		//  Copyright (C) 2004-2016 by EMGU Corporation. All rights reserved.       
		//----------------------------------------------------------------------------

		private static CascadeClassifier face;
		private static CascadeClassifier eye;

		private static Size minSize;
		private static Size minSizeEye;
		private static Size maxSizeEye;

		public static void Detect(IInputArray image, List<Rectangle> faces, List<Rectangle> eyes, out long detectionTime) {
			Stopwatch watch;

			using (InputArray iaImage = image.GetInputArray()) {
				//Read the HaarCascade objects
				//using (face)
				//using (eye) {
				watch = Stopwatch.StartNew();

				using (UMat ugray = new UMat()) {
					CvInvoke.CvtColor(image, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

					//normalizes brightness and increases contrast of the image
					CvInvoke.EqualizeHist(ugray, ugray);

					//Detect the faces  from the gray scale image and store the locations as rectangle
					//The first dimensional is the channel
					//The second dimension is the index of the rectangle in the specific channel                     
					Rectangle[] facesDetected = face.DetectMultiScale(
					   ugray,
					   1.1,
					   FACE_MIN_N,
					   minSize);

					faces.AddRange(facesDetected);

					foreach (Rectangle f in facesDetected) {
						//Get the region of interest on the faces
						using (UMat faceRegion = new UMat(ugray, f)) {

							minSizeEye.Width = (int)((float)f.Height * MIN_EYES_RATIO);
							minSizeEye.Height = minSizeEye.Width;

							maxSizeEye.Width = (int)((float)f.Height * MAX_EYES_RATIO);
							maxSizeEye.Height = maxSizeEye.Width;

							Rectangle[] eyesDetected = eye.DetectMultiScale(
							   faceRegion,
							   1.1,
							   EYE_MIN_N,
							   minSizeEye, maxSizeEye);

							foreach (Rectangle e in eyesDetected) {
								Rectangle eyeRect = e;
								eyeRect.Offset(f.X, f.Y);
								eyes.Add(eyeRect);
							}
						}
					}
					//}
					watch.Stop();
				}
				detectionTime = watch.ElapsedMilliseconds;
			}
		}
		#endregion
	}
}
