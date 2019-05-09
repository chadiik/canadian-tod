using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch.zigzag {

	public class Segment : IEnumerable<Segment> {

		public Segment previous, next;

		private Point m_A;
		private int m_Marks = 0;

		public Segment(Point a) {
			m_A = a;
		}

		public Point A {
			get { return m_A; }
		}

		public Point RegionStart {
			get {
				if(previous != null && next != null) {

					PointF normal = chadiik.geom.PathUtils.GetNormal(previous.A, A, next.A);
					return new Point((int)(A.X + normal.X * 9), (int)(A.Y + normal.Y * 9));
				}

				return new Point(A.X + 3, A.Y + 3);
			}
		}

		public int Marks { get { return m_Marks; } }

		public void Mark() {
			m_Marks++;
		}

		public void UnmarkAll() {
			foreach (Segment segment in this)
				segment.m_Marks = 0;
		}

		public void Split(Point point) {
			Segment segment = new Segment(point) { previous = this, next = next };
			next = segment;
		}

		public void Break() {
			if (previous != null)
				previous.next = null;
			if (next != null)
				next.previous = null;
			previous = next = null;
		}

		public bool TryGetB(out Point b) {
			if (next != null) {
				b = next.A;
				return true;
			}
			b = default(Point);
			return false;
		}

		public List<Point> GetPath() {
			List<Point> path = new List<Point>();
			foreach (Segment segment in this)
				path.Add(segment.A);

			return path;
		}

		public double DistanceSquaredTo(Point b) {
			int dx = b.X - m_A.X;
			int dy = b.Y - m_A.Y;
			return dx * dx + dy * dy;
		}

		public bool Intersects(Point a, Point b, out Point intersection) {
			Point aB;
			if (TryGetB(out aB))
				return chadiik.geom.PathUtils.SegmentIntersect(A, aB, a, b, out intersection);

			intersection = default(Point);
			return false;
		}

		public bool Intersects(Segment b, out Point intersection) {
			Point aB, bB;
			if (TryGetB(out aB) && b.TryGetB(out bB))
				return chadiik.geom.PathUtils.SegmentIntersect(A, aB, b.A, bB, out intersection);

			intersection = default(Point);
			return false;
		}

		public double CalculateChainArea() {
			return chadiik.geom.PathUtils.Area(GetPath());
		}

		public Segment FindHead() {
			Segment leftMost = this;
			foreach(Segment segment in this) {
				if (segment.A.X < leftMost.A.X)
					leftMost = segment;
				else if (segment.A.X == leftMost.A.X && segment.A.Y < leftMost.A.Y)
					leftMost = segment;
			}

			return leftMost;
		}

		/*public void Subdivide(double maxEdgeLength) {

			int numPoints = points.Count;
			List<Point> newPoints = new List<Point>((int)Math.Ceiling(numPoints * (.75 / maxEdgeLength)));

			newPoints.Add(new Point(points[0].X, points[0].Y));
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

						newPoints.Add(new Point((int)(a.X + aToBSubdivision.X * s), (int)(a.Y + aToBSubdivision.Y * s)));
					}
				}

				newPoints.Add(points[i]);
			}

			points = newPoints;
		}*/

		public IEnumerator<Segment> GetEnumerator() {
			Segment active = this;
			while (active != null) {

				yield return active;
				active = active.next;
				if (active == this)
					break;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}

	public class LinkedContour : IEnumerable<Segment> {

		private List<Segment> m_Heads;

		public LinkedContour(List<Segment> heads) {
			m_Heads = heads;
		}

		public Segment Head { get { return m_Heads.Count > 0 ? m_Heads[0] : null; } }

		public void Rebuild() {
			
			Segment startOfChain = null, endOfChain = null;
			foreach (Segment head in m_Heads.ToArray()) {
				m_Heads.Remove(head);
				foreach (Segment segment in head) {
					if (segment.Marks != 0) {
						if (startOfChain == null)
							startOfChain = segment;
						else
							endOfChain = segment;
					}
					else {
						if (startOfChain != null && endOfChain != null && startOfChain.previous != null && endOfChain.next != null) {
							startOfChain.previous.next = endOfChain.next;
							endOfChain.next.previous = startOfChain.previous;

							startOfChain.previous = endOfChain;
							endOfChain.next = startOfChain;

							if (IsValid(startOfChain))
								m_Heads.Add(startOfChain);

							startOfChain = endOfChain = null;
						}
					}
				}
			}

			for(int i = 0; i < m_Heads.Count; i++) {
				m_Heads[i] = m_Heads[i].FindHead();
				m_Heads[i].UnmarkAll();
			}
		}

		public List<Point> GetPath() {
			List<Point> path = new List<Point>();
			foreach (Segment segment in this)
				path.Add(segment.A);

			return path;
		}

		public void Visualize(IInputOutputArray image, MCvScalar lineColor, int lineThickness) {

			MCvScalar yellow = new MCvScalar(0, 200, 200),
				red = new MCvScalar(0, 0, 255);
			Point b;
			foreach (Segment segment in this) {
				if (segment.TryGetB(out b))
					CvInvoke.Line(image, segment.A, b, segment.Marks == 0 ? lineColor : (segment.Marks == 1 ? yellow : red), lineThickness);
				CvInvoke.Circle(image, segment.A, 1, lineColor, 1);
			}
		}

		public static bool IsValid(Segment head) {
			const int minLength = 5;
			const double minArea = 160;

			return Math.Abs(head.CalculateChainArea()) > minArea;

			int countSegments = 0;
			foreach(Segment segment in head) {
				countSegments++;
				if (countSegments == minLength) {
					if (head.CalculateChainArea() > minArea)
						return true;
					break;
				}
			}
			return false;
		}

		public static LinkedContour FromContour(Contour contour) {

			int numPoints = contour.points.Count;
			if (numPoints < 2) return null;

			Segment active;
			Segment head = active = new Segment(contour.points[0]);
			for(int i = 1; i < numPoints; i++) {
				Segment next = new Segment(contour.points[i]) { previous = active };
				active.next = next;
				active = next;
			}

			active.next = head;

			List<Segment> heads = new List<Segment> { head.FindHead() };
			return new LinkedContour(heads);
		}

		public IEnumerator<Segment> GetEnumerator() {

			foreach (Segment head in m_Heads)
				foreach (Segment active in head)
					yield return active;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
