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

		public void Close() {
			if (points[0] != points[points.Count - 1])
				points.Add(points[0]);
		}

		public bool IsWithin(Rectangle rect) {
			int numVertices = points.Count;
			for(int i = 0; i < numVertices; i++)
				if (!rect.Contains(points[i]))
					return false;
			
			return true;
		}

		public void Subdivide(double maxEdgeLength) {

			int numPoints = points.Count;
			List<Point> newPoints = new List<Point>((int)Math.Ceiling(numPoints * (.75 / maxEdgeLength)));

			newPoints.Add(new Point ( points[0].X, points[0].Y));
			for (int i = 1; i < numPoints; i++) {

				Point a = points[i - 1];
				Point b = points[i];

				PointF aToB = new PointF(b.X - a.X, b.Y - a.Y);
				double distance = Math.Sqrt(aToB.X * aToB.X + aToB.Y * aToB.Y);
				int subdivisions = (int)Math.Ceiling(distance / maxEdgeLength);
				//Logger.Instance.WriteLog("Subdivide(distance={0}, subdivisions={1})", distance.ToString(".00"), subdivisions);
				if (subdivisions > 1) {

					PointF aToBSubdivision = new PointF(aToB.X / subdivisions, aToB.Y / subdivisions);
					for (int s = 1; s < subdivisions; s++) {

						newPoints.Add(new Point ( (int)(a.X + aToBSubdivision.X * s), (int)(a.Y + aToBSubdivision.Y * s)));
					}
				}

				newPoints.Add(points[i]);
			}

			points = newPoints;
		}

		public bool ContainedIn(List<Contour> contours) {

			int numPoints = points.Count;
			foreach (Contour compare in contours) {

				if (compare.points.Count == numPoints) {
					int i = 0;
					for (; i < numPoints; i++)
						if (points[i] != compare.points[i])
							break;

					if (i == numPoints) {
						Logger.Instance.WriteLog("Contour contained in other");
						return true;
					}
				}
			}

			return false;
		}

		public void Visualize(IInputOutputArray image, MCvScalar lineColor, int lineThickness) {
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

		public static Contour FromRect(Rectangle rect) {
			return new Contour(new List<Point> {
				new Point(rect.X, rect.Y),
				new Point(rect.X + rect.Width, rect.Y),
				new Point(rect.X + rect.Width, rect.Y + rect.Height),
				new Point(rect.X, rect.Y + rect.Height)
			}, rect.Width * rect.Height);
		}
	}
}
