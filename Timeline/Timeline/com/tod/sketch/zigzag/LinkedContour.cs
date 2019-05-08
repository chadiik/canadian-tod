using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch {

	public class Segment {

		public Segment previous, next;

		private Point m_A;

		public Segment(Point a) {
			m_A = a;
		}

		public Point A {
			get { return m_A; }
		}

		public bool TryGetB(out Point b) {
			b = Point.Empty;
			if (next != null) {
				b = next.A;
				return true;
			}
			return false;
		}
	}

	public class LinkedContour {

		public List<Segment> heads;

		public void Draw(IInputOutputArray image, MCvScalar lineColor, int lineThickness) {
			foreach (Segment head in heads) {
				Segment active = head;
				while(active != null) {
					Point b;
					if(active.TryGetB(out b))
						CvInvoke.Line(image, active.A, b, lineColor, lineThickness);

					active = active.next;
					if (active == head)
						break;
				}
			}
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

			List<Segment> heads = new List<Segment> { head };
			return new LinkedContour { heads = heads };
		}
	}
}
