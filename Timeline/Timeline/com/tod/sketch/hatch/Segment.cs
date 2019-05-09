using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch.hatch {

	public class Segment {

		public Point a, b;

		public static void Visualize(List<Segment> segments, IInputOutputArray image, MCvScalar lineColor, int lineThickness, bool label) {
			for (int i = 0, numSegments = segments.Count; i < numSegments; i++) {
				CvInvoke.Line(image, segments[i].a, segments[i].b, lineColor, lineThickness);

				if (label)
					CvInvoke.PutText(image, (i - 1).ToString(), segments[i].a, Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, lineColor, 1);
			}
		}
	}
}
