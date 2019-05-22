using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace com.tod.core {

	public struct Coo {
		public float x, y;
		public bool down;

		public Coo(float x, float y, bool down) {
			this.x = x;
			this.y = y;
			this.down = down;
		}

		public Point ToPoint() {
			return new Point((int)x, (int)y);
		}

		public override string ToString() {
			return string.Format("[{0},{1},{2}]", x.ToString(".0"), y.ToString(".0"), down);
		}
	}

	public class Line {

		public List<Coo> path;

		private bool m_HintPen = false;
		private bool m_HintPenValue = false;

		public Line() {
			path = new List<Coo>();
		}

		public Line(List<Coo> path) {
			this.path = path;
		}

		/// <summary>Hint the line that the next added coo should be set to (true=down or false=up)</summary>
		public bool HintPen {
			set {
				m_HintPen = true;
				m_HintPenValue = value;
			}
		}

		public int NumVertices {
			get {
				return path.Count;
			}
		}

		public void Add(Coo point) {
			if (m_HintPen) {
				m_HintPen = false;
				point.down = m_HintPenValue;
			}

			path.Add(point);
		}

		public void Clear() {
			path.Clear();
		}

		public void BreakEnds() {
			int n = path.Count;
			if (n > 0) {
				Coo c = path[0];
				c.down = false;
				path[0] = c;

				c = path[n - 1];
				c.down = false;
				path[n - 1] = c;
			}
		}

		public override string ToString() {
			return string.Format("Line({0}): {1}", path.Count, string.Join(", ", path.Select((coo) => coo.ToString()).ToArray()));
		}

		public static List<Coo> Merge(List<Line> lines) {
			List<Coo> merged = new List<Coo>();
			foreach (Line line in lines) merged.AddRange(line.path);
			return merged;
		}

		public static List<Line> Convert(List<sketch.TP> points) {

			Line line = null;
			List<Line> lines = new List<Line>();

			bool penDown = false;
			for (int i = 0, numPoints = points.Count; i < numPoints; i++) {
				if (points[i].IsDown) {
					if (!penDown) {
						penDown = true;
						lines.Add(line = new Line());
						line.Add(new Coo(points[i].x, points[i].y, false));
					}
					else {
						line.Add(new Coo(points[i].x, points[i].y, true));
					}
				}
				else {
					penDown = false;
				}
			}

			Sanitize(lines);
			return lines;
		}

		public static void Sanitize(List<Line> lines) {
			for (int i = 0; i < lines.Count; i++) {
				if (lines[i].NumVertices < 3)
					lines.RemoveAt(i--);
				else
					lines[i].BreakEnds();
			}
		}

		public static void Visualize(List<Line> lines, Emgu.CV.IInputOutputArray image, Emgu.CV.Structure.MCvScalar lineColor, int lineThickness) {
			foreach (Line line in lines) {
				List<Coo> points = line.path;
				for (int i = 1, numPoints = points.Count; i < numPoints; i++)
					if (points[i - 1].down && points[i].down)
						Emgu.CV.CvInvoke.Line(image, points[i - 1].ToPoint(), points[i].ToPoint(), lineColor, lineThickness);
			}
		}

		public static string Print(List<Line> lines) {
			return string.Format("Lines x {0}\n:{1}", lines.Count, string.Join("\n\t", lines.Select((line) => line.ToString()).ToArray()));
		}
	}
}
