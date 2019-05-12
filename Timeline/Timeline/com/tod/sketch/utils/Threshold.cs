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

		public int Brightness { get { return low; } }
		public double Angle { get { return (.2 + (low / 255.0) * .7) * (Math.PI / 2.0); } }
		public double SpreadAngle { get { return Math.PI / 2.0 / 9.0; } }
		public double Spread { get { return Math.Pow((.3 + (low / 255.0) * .8), .7) * 10.0; } }

		public List<Contour> GetContours(Image<Gray, byte> source, bool modifySourceImage) {

			Image<Gray, byte> binary;
			if (modifySourceImage) {
				source._ThresholdBinary(new Gray(low), new Gray(low));
				binary = source;
			}
			else {
				binary = new Image<Gray, byte>(source.Size);
				CvInvoke.Threshold(source, binary, low, high, ThresholdType.Binary);
			}

			List<Contour> contours = Contour.Extract(binary);
			return contours;
		}

		public override string ToString() {
			return string.Format("Threshold({0}, {1}) -> Brightness={2}|Angle={3}|SpreadAngle={4}", low, high, Brightness, Angle.ToString(".00"), SpreadAngle.ToString(".00"));
		}
	}
}
