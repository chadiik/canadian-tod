using System.Collections;
using System.Collections.Generic;
using Vector2 = System.Drawing.Point;

namespace chadiik.algorithms {

	public class SimplifyJS {

		/*
		 * 
		 * BSD-2-Clause License
		 * https://opensource.org/licenses/BSD-2-Clause
		 * 
		 * Copyright (C) 2017, Vladimir Agafonkin
		 * For Simplify.js, a high-performance JS polyline simplification library
		 * http://mourner.github.io/simplify-js/
		 * 
		 * Copyright (C) 2018 Chady Karlitch (http://chadiik.com) C# (Unity) port
		 * 
		 */

		public static List<Vector2> Simplify ( List<Vector2> points, float tolerance, bool highestQuality = false ) {

			if ( points.Count <= 2 ) return points;

			var sqTolerance = tolerance * tolerance;

			points = highestQuality ? points : SimplifyRadialDist ( points, sqTolerance );
			points = SimplifyDouglasPeucker ( points, sqTolerance );

			return points;
		}

		public static List<Vector2> SimplifyDouglasPeucker ( List<Vector2> points, float sqTolerance ) {

			int last = points.Count - 1;

			List<Vector2> simplified = new List<Vector2>();
			simplified.Add ( points [ 0 ] );
			SimplifyDPStep ( points, 0, last, sqTolerance, simplified );
			simplified.Add ( points [ last ] );

			return simplified;
		}

		public static List<Vector2> SimplifyRadialDist ( List<Vector2> points, float sqTolerance ) {

			Vector2 prevPoint = points[0],
					point = default(Vector2);

			List<Vector2> newPoints = new List<Vector2>();
			newPoints.Add ( prevPoint );

			for ( int i = 1, len = points.Count; i < len; i++ ) {

				point = points [ i ];

				float vx = point.X - prevPoint.X,
						vy = point.Y - prevPoint.Y;

				if ( ( vx * vx + vy * vy ) > sqTolerance ) {

					newPoints.Add ( point );
					prevPoint = point;
				}
			}

			if ( prevPoint != point ) newPoints.Add ( point );

			return newPoints;
		}

		private static float GetSqSegDist ( Vector2 p, Vector2 p1, Vector2 p2 ) {

			float x = p1.X, y = p1.Y, dx = p2.X - x, dy = p2.Y - y;
			if ( dx != 0 || dy != 0 ) {

				float t = ((p.X - x) * dx + (p.Y - y) * dy) / (dx * dx + dy * dy);
				if ( t > 1 ) {

					x = p2.X;
					y = p2.Y;
				}
				else if ( t > 0 ) {

					x += dx * t;
					y += dy * t;
				}
			}

			dx = p.X - x;
			dy = p.Y - y;

			return dx * dx + dy * dy;
		}

		private static void SimplifyDPStep ( List<Vector2> points, int first, int last, float sqTolerance, List<Vector2> simplified ) {

			float maxSqDist = sqTolerance;
			int index = 0;

			for ( int i = first + 1; i < last; i++ ) {

				float sqDist = GetSqSegDist(points[i], points[first], points[last]);

				if ( sqDist > maxSqDist ) {

					index = i;
					maxSqDist = sqDist;
				}
			}

			if ( maxSqDist > sqTolerance ) {

				if ( index - first > 1 ) SimplifyDPStep ( points, first, index, sqTolerance, simplified );
				simplified.Add ( points [ index ] );
				if ( last - index > 1 ) SimplifyDPStep ( points, index, last, sqTolerance, simplified );
			}
		}
	}
}