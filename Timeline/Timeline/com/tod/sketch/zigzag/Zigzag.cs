using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.tod.core;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace com.tod.sketch {

	public class Zigzag {

		public class Parameters {

			public Threshold[] thresholds;
			public int precision = 1;

			public static Parameters Default(int numThresholds) {
				Threshold[] thresholds = new Threshold[numThresholds];

				for(int i = 0; i < numThresholds; i++) {
					int low = (int)((double)(i + 1) / (numThresholds + 1) * 255);
					int high = (int)((double)(i + 2) / (numThresholds + 1) * 255);
					Threshold threshold = new Threshold { low = low, high = high };
					thresholds[i] = threshold;
				}

				return new Parameters() { thresholds = thresholds };
			}
		}

		public event SketchComplete SketchCompleted;

		public void Draw(Portrait portrait, Parameters parameters) {

			Image<Gray, byte>  filteredSource = Preprocess(portrait);

			List<Contour> allContours = new List<Contour>();
			for (int i = 0; i < parameters.thresholds.Length; i++) {
				Threshold threshold = parameters.thresholds[i];
				List<Contour> contours = threshold.GetContours(filteredSource);
				foreach (Contour contour in contours) {
					int preNum = contour.points.Count;
					contour.Simplify(parameters.precision);
					//contour.Subdivide(parameters.precision);
				}
				allContours.AddRange(contours);
			}

			// preview
			Image<Gray, byte> contoursPreview = new Image<Gray, byte>(filteredSource.Size);
			MCvScalar lineColor = new Bgr(Color.White).MCvScalar;
			foreach (Contour contour in allContours) contour.Draw(contoursPreview, lineColor, 1);
			Sketch.ShowProcessImage(contoursPreview, string.Format("Contours [{0}]", "all"));
			// end preview

			SketchCompleted?.Invoke(new List<TP>() { new TP(1000, 1000), new TP(1000, 1000) });
		}

		private Image<Gray, byte> Preprocess(Portrait portrait) {

			const int hMax = 500;

			// Scale and apply easy filter
			double scale = 1;
			Image<Bgr, byte> sourceImage = portrait.source.Clone();
			sourceImage = TODFilters.ScaleToFit(sourceImage, hMax, out scale);
			Sketch.ShowProcessImage(sourceImage, "source");

			portrait.ScaleRect(scale);

			CvInvoke.FastNlMeansDenoisingColored(sourceImage, sourceImage, 3f, 10f, 7, 13);

			// Clahe
			const double clipLimit = 4.0;
			Size tileGridSize = new Size(5, 5);

			Image<Gray, byte> grayFrame = sourceImage.Convert<Gray, byte>();
			grayFrame._EqualizeHist();
			Sketch.ShowProcessImage(grayFrame, "Equalized Histogram");

			grayFrame = grayFrame.SmoothBlur(4, 4);

			CvInvoke.CLAHE(grayFrame, clipLimit, tileGridSize, grayFrame);
			Image<Gray, byte> filtered = grayFrame;
			Sketch.ShowProcessImage(filtered, "Clahe");

			//TODFilters.Sharpen(coloredGF, 12);

			return filtered;
		}
	}
}
