using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;

namespace com.tod.sketch {

	public class Threshold {

		public int low = 0, high = 255;

		public List<Contour> GetContours(Image<Gray, byte> source) {

			Image<Gray, byte> binary = new Image<Gray, byte>(source.Size);
			CvInvoke.Threshold(source, binary, low, low, ThresholdType.Binary);
			//Sketch.ShowProcessImage(binary, string.Format("Threshold [{0}]", low));

			List<Contour> contours = Contour.Extract(binary);
			return contours;
		}
	}
}
