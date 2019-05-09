using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch {

	public class LinesExtraction {

		public class HoughParameters {

			/// <summary>The threshhold to find initial segments of strong edges</summary>
			public double cannyThreshold = 30.0;
			/// <summary>The threshold used for edge Linking</summary>
			public double cannyThresholdLinking = 60.0;
			/// <summary>Distance resolution in pixel-related units</summary>
			public double rhoResolution = 1.0;
			/// <summary>Angle resolution measured in radians</summary>
			public double thetaResolution = Math.PI / 60.0;
			/// <summary>Minimum width of a line</summary>
			public double minLineWidth = 5.0;
			/// <summary>Minimum gap between lines</summary>
			public double gapBetweenLines = 4.0;
			/// <summary>A line is returned by the function if the corresponding accumulator value is greater than threshold</summary>
			public int threshold = 7;
		}

		public static LineSegment2D[][] From(Image<Bgr, byte> source, HoughParameters parameters) {
			//source = source.SmoothBlur(2, 2);
			//CvInvoke.PyrMeanShiftFiltering(source, source, 2, 60, 1, new MCvTermCriteria(3, 1));
			//CvInvoke.HoughLines(source, lines, parameters.rhoResolution, parameters.thetaResolution, parameters.threshold, parameters.srn, parameters.stn);
			LineSegment2D[][] lines = source.HoughLines(parameters.cannyThreshold, parameters.cannyThresholdLinking, parameters.rhoResolution, parameters.thetaResolution, parameters.threshold, parameters.minLineWidth, parameters.gapBetweenLines);

			return lines;
		}
	}
}
