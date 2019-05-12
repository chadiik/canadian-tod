using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;

namespace com.tod.sketch.path {
	class PathOrder {

		//private static QuadTree<OTP> _tree;
		public static List<OTP> _edges;
		private static List<OTP> _unordered;
		private static List<OTP> _ordered;
		private static ImageData _map;

		public static List<OTP> Order(List<TP> unordered, ImageData map, Image<Bgr, byte> debug) {
			if (unordered.Count < 3) new List<OTP>();

			_map = map;
			map.CreateHsv();

			//_bounds = new RectangleF(0f, 0f, 1f, 1f);
			//_tree = new QuadTree<OTP>(_bounds);

			_unordered = new List<OTP>();

			OTP start = new OTP(unordered[0]);
			OTP current = start;
			for (int i = 1; i < unordered.Count; i++) {
				_unordered.Add(current);
				current.next = new OTP(unordered[i]);
				current.hue = map.GetHsv(current.point.x, current.point.y, 0) + map.GetHsv(current.point.x, current.point.y, 2) * .25f;
				current.brightness = map.GetHsv(current.point.x, current.point.y, 2);

				current = current.next;
			}

			_ordered = new List<OTP>();
			_ordered.Add(new OTP(TP.PenUp));
			_ordered.Add(new OTP(TP.PenUp));
			GetOrderedList(new OTP(new TP(.5f, .5f)));

			float avgDistance = _avgDistance / (float)_avgOps;
			float avgHue = _avgHue / (float)_avgOps;
			Logger.Instance.SilentLog("Ops = {0} | Distance avg = {1} | Hue difference avg = {2}", _avgOps.ToString(), avgDistance.ToString("0.00000"), avgHue.ToString("0.00000"));
			Logger.Instance.SilentLog("Removed nodes = {0}", _removedNodes.ToString());

			return _ordered;
		}

		private struct DetailParams {
			public double minSize, sizeRange, brightPower, distancePower, alaunch, breakdistance, detailSizeThreshold, minSteps, stepsSizeMultiplier;
		}

		private static Dictionary<string, DetailParams> detailPresets = new Dictionary<string, DetailParams>()  {
			{ "default", new DetailParams { minSize = .0065, sizeRange = .0925, brightPower = 1.2, distancePower = 1.5, alaunch = 30, breakdistance = Math.Pow(.06, 2), detailSizeThreshold = .1, minSteps = 4, stepsSizeMultiplier = 80 } },
			{ "0", new DetailParams { minSize = .0065, sizeRange = .1, brightPower = 1.7, distancePower = 1.5, alaunch = 30, breakdistance = Math.Pow(.06, 2), detailSizeThreshold = .1, minSteps = 4, stepsSizeMultiplier = 80  } },
			{ "1", new DetailParams { minSize = .0065, sizeRange = .1, brightPower = .7, distancePower = 1.7, alaunch = 30, breakdistance = Math.Pow(.06, 2), detailSizeThreshold = .1, minSteps = 4, stepsSizeMultiplier = 80  } },
			{ "base", new DetailParams { minSize = .0065, sizeRange = .2, brightPower = .7, distancePower = 4, alaunch = 30, breakdistance = Math.Pow(.06, 2), detailSizeThreshold = .1, minSteps = 4, stepsSizeMultiplier = 80  } },
			{ "2", new DetailParams { minSize = .01, sizeRange = .3, brightPower = .7, distancePower = 4, alaunch = 90, breakdistance = .05, detailSizeThreshold = .1, minSteps = 4, stepsSizeMultiplier = 80  } },
			{ "exp", new DetailParams { minSize = .012, sizeRange = .3, brightPower = 1.5, distancePower = 2, alaunch = 90, breakdistance = .05, detailSizeThreshold = .1, minSteps = 4, stepsSizeMultiplier = 80  } },
			{ "3", new DetailParams { minSize = .015, sizeRange = .25, brightPower = 1.5, distancePower = 3, alaunch = 45, breakdistance = .01, detailSizeThreshold = .00, minSteps = 4, stepsSizeMultiplier = 80  } },
            { "4", new DetailParams { minSize = .01, sizeRange = .3, brightPower = .5, distancePower = 4, alaunch = 180, breakdistance = .05, detailSizeThreshold = .1, minSteps = 2, stepsSizeMultiplier = 20  } }
        };

		private static DetailParams preset = detailPresets["4"];
		private static int numDistanceBreaks = 0;

		public static void GetOrderedList(OTP startPosition) {
			OTP item = startPosition;
			OTP lastItem = item;
			Logger.Instance.SilentLog("!");

			#region edgesQuadrants

			List<bool> quadrantDone = new List<bool>();
			quadrantDone.Add(false);
			quadrantDone.Add(false);
			quadrantDone.Add(false);
			quadrantDone.Add(false);
			quadrantDone.Add(false);
			List<List<OTP>> quadrants = new List<List<OTP>>();
			quadrants.Add(new List<OTP>());
			quadrants.Add(new List<OTP>());
			quadrants.Add(new List<OTP>());
			quadrants.Add(new List<OTP>());
			quadrants.Add(new List<OTP>());

			quadrants[0].Add(new OTP(TP.PenUp));
			quadrants[1].Add(new OTP(TP.PenUp));
			quadrants[2].Add(new OTP(TP.PenUp));
			quadrants[3].Add(new OTP(TP.PenUp));
			quadrants[4].Add(new OTP(TP.PenUp));

			int lastQuadrant = 2;
			int numEdges = _edges.Count;
			for (int iEdge = 0; iEdge < numEdges; iEdge += 2) {
				OTP edge = _edges[iEdge];
				TP edgePoint = edge.point;
				int quadrant = 2;
				if (edgePoint.x > 0f && edgePoint.y > 0) {
					if (edgePoint.x < .5f) {
						if (edgePoint.y < .3f) {
							quadrant = 0;
						}
						else if (edgePoint.y > .7f) {
							quadrant = 3;
						}
					}
					else {
						if (edgePoint.y < .3f) {
							quadrant = 1;
						}
						else if (edgePoint.y > .7f) {
							quadrant = 4;
						}
					}
				}
				else {
					quadrant = lastQuadrant;
				}

				quadrants[quadrant].Add(edge);
				lastQuadrant = quadrant;
			}

			int waitForEdge = 400;
			#endregion


			double minSize = preset.minSize;
			double sizeRange = preset.sizeRange;
			numDistanceBreaks = 0;
			while (item != null && !item.visited) {
				int numOrdered = _ordered.Count;
				if (numOrdered > 2) {
					double vx = item.point.x - .5;
					double vy = item.point.y - .5;
					double distanceCenter = Math.Sqrt(vx * vx + vy * vy);
					double brightness = (Math.Pow((double)item.brightness, preset.brightPower) * Math.Pow(distanceCenter, preset.distancePower));
					double size = minSize + brightness * sizeRange;
                    size = minSize + Math.Pow(brightness * 10, 2) / 10.0 * sizeRange;
					if (size > minSize + sizeRange * preset.detailSizeThreshold) {
						DrawDetail(
							_ordered[numOrdered - 2].point,
							_ordered[numOrdered - 1].point,
							item.point,
							size,
							1 - brightness);
					}
					else {
						_ordered.Add(_ordered[numOrdered - 1]);
					}
				}

				_ordered.Add(item);

				item.visited = true;

				OTP closest = null;
				OTP closestParent = null;
				float minScore = float.MaxValue;

				OTP test = _unordered[0];
				OTP testItem = test.next;
				OTP lastTestItem = testItem;
				while (testItem != null) {
					if (item != testItem && !testItem.visited) {
						float score = Score(item, testItem);
						if (score < minScore) {
							closest = testItem;
							closestParent = lastTestItem;
							minScore = score;
						}
						lastTestItem = testItem;
					}
					testItem = testItem.next;
				}

				if (closest != null) {
					
					float distanceSquared = item.point.DistanceSquared(closest.point);
					if (distanceSquared > preset.breakdistance) {
						_ordered.Add(new OTP(TP.PenUp));
						numDistanceBreaks++;
					}

					closestParent.next = closest.next;
					_removedNodes++;

					#region quadrant edges
					TP edgePoint = closest.point;
					waitForEdge--;
					if (waitForEdge < 0) {
						int quadrant = 2;
						if (edgePoint.x < .5f) {
							if (edgePoint.y < .3f) {
								quadrant = 0;
							}
							else if (edgePoint.y > .7f) {
								quadrant = 3;
							}
						}
						else {
							if (edgePoint.y < .3f) {
								quadrant = 1;
							}
							else if (edgePoint.y > .7f) {
								quadrant = 4;
							}
						}

						waitForEdge = 20;
						if (!quadrantDone[quadrant]) {
							_ordered.Add(new OTP(TP.PenUp));
							Logger.Instance.SilentLog("QUADRANT {0} appended.", quadrant.ToString());
							waitForEdge = 100;
							quadrantDone[quadrant] = true;

							int numQuad = quadrants[quadrant].Count;
							for (int iEdge = 0; iEdge < numQuad; iEdge++) {
								_ordered.Add(quadrants[quadrant][iEdge]);
							}

							_ordered.Add(new OTP(TP.PenUp));
						}
					}
					#endregion
				}

				lastItem = item;
				item = closest;
			}

			Logger.Instance.WriteLog("numDistanceBreaks: " + numDistanceBreaks);
		}

		private static float PI = (float)Math.PI;
		private static float TWO_PI = (float)(Math.PI * 2);
		private static float PI_2 = (float)(Math.PI / 2);
		private static float A_MIN = (float)(220 * Math.PI / 180);
		private static float A_MAX = (float)(720 * Math.PI / 180);
		private static void DrawDetail(TP previous, TP current, TP next, double size, double angleRatio) {
			if (previous.x == TP.PenUp.x || current.x == TP.PenUp.x || next.x == TP.PenUp.x) return;
			//if (--_maxDo < 0) return;
			TP motion = new TP(current.x - previous.x, current.y - previous.y);
			motion.Normalize((float)size);

			TP motionLN = new TP(motion.y, - motion.x);
			current.x -= motionLN.x;
			current.y -= motionLN.y;
			double nextMotionAngle = 0;
			double motionAngle = Math.Atan2(motion.y, motion.x);
			double steps = preset.minSteps + preset.stepsSizeMultiplier * size;
			double da = Math.PI / steps * 2;
			TP lp = current;
			double a = 0;
			double i = 0;
			while (
				(da * i < A_MIN || AngleDistance(a, nextMotionAngle) > (float)(preset.alaunch * Math.PI / 180))
				&& da * i < A_MAX
				) {
				a = (motionAngle + da * i);
				double x = Math.Cos(a) * size;
				double y = Math.Sin(a) * size;
				TP np = new TP(current.x + (float)x, current.y + (float)y);
				nextMotionAngle = Math.Atan2(next.y - np.y, next.x - np.x) - PI_2;
				_ordered.Add(new OTP(np));
				lp = np;
				i += 1;
			}
		}

		private static float Score(OTP item, OTP neighbour) {
			float distance = (float)Math.Sqrt(item.point.DistanceSquared(neighbour.point));
			float deltaHue = RangedDistance(item.hue, neighbour.hue);

			_avgDistance += distance;
			_avgHue += deltaHue;
			_avgOps++;

			float hueWeight = .5f;

			float score =
				distance * 1f
				+
				deltaHue * hueWeight
				;
			return score;
		}

		private static float _avgDistance = 0f, _avgHue = 0f;
		private static int _avgOps = 0, _removedNodes = 0;
		private static float RangedDistance(float a, float b) {
			float max = Math.Max(a, b);
			float min = Math.Min(a, b);
			float d = max - min;
			if (d <= .5f) return d;
			return 1f - max + min;
		}

		private static double AngleDistance(double a, double b) {
			double da = b - a;
			if (da > PI) da -= TWO_PI;
			if (da < -PI) da += TWO_PI;
			return da;

			//double da = b - a + PI;
			//mod = (a, n) -> a - floor(a/n) * n
			da = da - Math.Floor(da / TWO_PI) * TWO_PI;
			return da - PI;
		}

	}
}
