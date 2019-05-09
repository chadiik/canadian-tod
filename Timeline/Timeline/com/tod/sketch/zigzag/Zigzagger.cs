using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.tod.sketch {

	public class Zigzagger {

		class Bundle {

			struct Intersection {
				public Segment segment;
				public Point point;

				public static Comparison<Intersection> DistanceSort(Point p) {
					return (Intersection a, Intersection b) => {
						double vx = a.point.X - p.X,
							vy = a.point.Y - p.Y;
						double da = vx * vx + vy * vy;

						vx = b.point.X - p.X;
						vy = b.point.Y - p.Y;
						double db = vx * vx + vy * vy;

						if (da < db) return -1;
						else if (da > db) return 1;
						return 0;
					};
				}
			}

			public Threshold threshold;
			public Probe probe;
			public List<Point> path;

			public Bundle(int x, int y, Threshold threshold) {
				this.threshold = threshold;

				probe = new Probe(this.threshold.Angle, this.threshold.SpreadAngle) { x = x, y = y };
				path = new List<Point> { probe.Point };
			}

			public void Process(Image<Gray, byte> regionsMap, List<LinkedContour> linkedContours) {

				// cache & setup
				byte[,,] data = regionsMap.Data;
				int brightness = threshold.Brightness;
				List<Intersection> intersections = new List<Intersection>();
				List<Segment> intersectionCandidates = new List<Segment>();
				const double intersectionCandidateDistanceSquared = 9.0 * 9.0;
				const double probeLength = 3.0;

				int probeBreakout = 10000;
				double direction = 1.0;
				int outOfBounds = 0;

				while (outOfBounds < 2 && probeBreakout-- > 0) {

					Point previousPosition = probe.Point;
					probe.Advance(direction * probeLength);

					if (data[probe.Y, probe.X, 0] == brightness) {
						outOfBounds = 0;
					}
					else {
						intersectionCandidates.Clear();
						Point currentPosition = probe.Point;
						foreach (LinkedContour linkedContour in linkedContours)
							foreach (Segment segment in linkedContour)
								if (true || segment.DistanceSquaredTo(currentPosition) < intersectionCandidateDistanceSquared)
									intersectionCandidates.Add(segment);

						//Logger.Instance.WriteLog("intersectionCandidates x {0}", intersectionCandidates.Count);

						intersections.Clear();
						Point intersectionPoint;
						foreach (Segment segment in intersectionCandidates)
							if (segment.Intersects(previousPosition, currentPosition, out intersectionPoint))
								intersections.Add(new Intersection { segment = segment, point = intersectionPoint });

						//Logger.Instance.WriteLog("intersections x {0}", intersections.Count);

						if (intersections.Count > 0) {
							if (intersections.Count > 1)
								intersections.Sort(Intersection.DistanceSort(previousPosition));

							intersections[0].segment.Mark();
							intersections[0].segment.Split(intersections[0].point);
							probe.x = previousPosition.X;//intersections[0].point.X;
							probe.y = previousPosition.Y;//intersections[0].point.Y;

							path.Add(intersections[0].point);

							direction = -direction;
						}
						outOfBounds++;
					}
				}
			}
		}

		public List<LinkedContour> linkedContours;
		public Threshold[] thresholds;
		public Image<Gray, byte> regionsMap;

		public Zigzagger(List<LinkedContour> linkedContours, Threshold[] thresholds, Image<Gray, byte> regionsMap) {

			this.linkedContours = linkedContours;
			this.thresholds = thresholds;
			this.regionsMap = regionsMap;

			this.linkedContours.Sort(DistanceSort(new Point(this.regionsMap.Width / 2, this.regionsMap.Height / 2)));

			// preview
			Image<Gray, byte> contoursPreview = new Image<Gray, byte>(this.regionsMap.Size);
			MCvScalar white = new Bgr(Color.White).MCvScalar;
			for (int i = 0; i < this.linkedContours.Count; i++) {
				double t = 1.0 - (double)i / this.linkedContours.Count;
				this.linkedContours[i].Visualize(contoursPreview, new MCvScalar(t * 255.0, t * 255.0, t * 255.0), 1);
			}
			Sketch.ShowProcessImage(contoursPreview, "LinkedContours");
			// end preview

			MCvScalar red = new Bgr(Color.Red).MCvScalar,
				blue = new Bgr(Color.LightBlue).MCvScalar;

			int breakout = 3;
			while (breakout-- > 0) {
				Image<Bgr, byte> bundlesPreview = new Image<Bgr, byte>(this.regionsMap.Size);
				foreach (LinkedContour linkedContour in this.linkedContours) {
					Segment head = linkedContour.Head;
					if (head != null) {
						Point regionStart = head.RegionStart;
						Threshold threshold = GetThreshold(regionStart.X, regionStart.Y);

						if (threshold != null) {
							CvInvoke.Circle(bundlesPreview, regionStart, 1, new MCvScalar(0, 255, 0), 2);
							Bundle bundle = new Bundle(regionStart.X, regionStart.Y, threshold);
							bundle.Process(this.regionsMap, this.linkedContours);

							Zigzag.Visualize(bundle.path, bundlesPreview, white, 1, false);
						}
					}
				}

				foreach (LinkedContour linkedContour in this.linkedContours)
					linkedContour.Visualize(bundlesPreview, blue, 1);

				Sketch.ShowProcessImage(bundlesPreview, "Bundles");

				Image<Bgr, byte> rebuildPreview = new Image<Bgr, byte>(this.regionsMap.Size);
				foreach (LinkedContour linkedContour in this.linkedContours) {
					linkedContour.Rebuild();

					linkedContour.Visualize(rebuildPreview, blue, 1);
				}

				Sketch.ShowProcessImage(rebuildPreview, "Rebuild");
			}
		}

		private Threshold GetThreshold(int x, int y) {
			byte brightness = regionsMap.Data[y, x, 0];
			foreach (Threshold threshold in thresholds)
				if (brightness == threshold.Brightness)
					return threshold;

			return null;
		}

		public static Comparison<LinkedContour> DistanceSort(Point p) {
			return (LinkedContour a, LinkedContour b) => {
				double da = a.Head.DistanceSquaredTo(p);
				double db = b.Head.DistanceSquaredTo(p);

				if (da < db) return -1;
				else if (da > db) return 1;
				return 0;
			};
		}
	}
}
