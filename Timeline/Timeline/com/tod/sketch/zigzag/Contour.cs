using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace com.tod.sketch {

	public class Contour {

		public List<Point> points;
		public double area;

		public Contour(List<Point> points, double area) {
			this.points = points;
			this.area = area;
		}

		public void Simplify(int tolerance, bool highestQuality = false) {
			points = chadiik.algorithms.SimplifyJS.Simplify(points, tolerance, highestQuality);
		}

		public void Subdivide(double maxEdgeLength) {

			int numVertices = points.Count;
			List<Point> newPoints = new List<Point>((int)Math.Ceiling(numVertices * (.75 / maxEdgeLength)));

			newPoints.Add(new Point ( points[0].X, points[0].Y));
			for (int i = 1; i < numVertices; i++) {

				Point a = points[i - 1];
				Point b = points[i];

				Point aToB = new Point(b.X - a.X, b.Y - a.Y);
				double distance = chadiik.geom.PathUtils.Magnitude(aToB);
				int subdivisions = (int)Math.Ceiling(distance / maxEdgeLength);
				if (subdivisions > 1) {

					Point aToBSubdivision = new Point(aToB.X / subdivisions, aToB.Y / subdivisions);
					for (int s = 1; s < subdivisions; s++) {

						newPoints.Add(new Point ( a.X + aToBSubdivision.X * s, a.Y + aToBSubdivision.Y * s));
					}
				}

				newPoints.Add(points[i]);
			}

			points = newPoints;
		}

		public void Draw(IInputOutputArray image, MCvScalar lineColor, int lineThickness) {
			for(int i = 0, numPoints = points.Count; i < numPoints; i++) {
				CvInvoke.Line(image, points[i], points[(i + 1) % numPoints], lineColor, lineThickness);
			}
		}

		public static List<Contour> Extract(IInputOutputArray binary) {

			List<Contour> contours = new List<Contour>();
			using (VectorOfVectorOfPoint vContours = new VectorOfVectorOfPoint()) {
				CvInvoke.FindContours(binary, vContours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
				int numContours = vContours.Size;
				for (int i = 0; i < numContours; i++) {
					using (VectorOfPoint contour = vContours[i]) {
						double area = CvInvoke.ContourArea(contour, false);
						if (area != 0) {
							contours.Add(new Contour(contour.ToArray().ToList(), area));
						}
					}
				}
			}

			return contours;
		}
	}
}
