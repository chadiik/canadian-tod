﻿using System;
using System.Collections.Generic;
using Vector2 = System.Drawing.Point;
using PointF = System.Drawing.PointF;

namespace chadiik.geom {

	public class PathUtils {

		public static float Area ( List<Vector2> polygon ) {

			int pathLength = polygon.Count;
			float area = 0;

			for ( int prev = pathLength - 1, next = 0; next < pathLength; prev = next++ )
				area += polygon [ prev ].X * polygon [ next ].Y - polygon [ next ].X * polygon [ prev ].Y;

			return area * 0.5f;
		}

		public static bool IsClockWise ( List<Vector2> path ) {

			return Area ( path ) < 0;
		}

		public static bool IsClockWise ( float polygonArea ) {

			return polygonArea < 0;
		}

		public static PointF GetNormal(Vector2 p, Vector2 c, Vector2 n) {

			/*
			Vector2 pc = new Vector2(p.X - c.X, p.Y - c.Y),
				cn = new Vector2(c.X - n.X, c.Y - n.Y);
			pc.Set(-pc.y, pc.x);
			cn.Set(-cn.y, cn.x);
			*/

			Vector2 pc = new Vector2(-(p.Y - c.Y), p.X - c.X),
				cn = new Vector2(-(c.Y - n.Y), c.X - n.X);

			float pcm = (float)Math.Sqrt(pc.X * pc.X + pc.Y * pc.Y),
				cnm = (float)Math.Sqrt(cn.X * cn.X + cn.Y * cn.Y);

			// return (pc / pcm + cn / cnm).normalized;
			PointF normal = new PointF(pc.X / pcm + cn.X / cnm, pc.Y / pcm + cn.Y / cnm);
			float nMag = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y);
			return new PointF(normal.X / nMag, normal.Y / nMag);
		}

		public static Vector2 CalculateMeanCenter ( List<Vector2> path ) {

			int numVerts = path.Count;
			Vector2 c = new Vector2();
			for ( int i = 0; i < numVerts; i++ )
				c.Offset(path [ i ]);

			return new Vector2(c.X / numVerts, c.Y / numVerts);
		}

		public static Vector2 FindCentroid(List<Vector2> pts) {

			int nPts = pts.Count;
			Vector2 off = pts[0];
			double twicearea = 0;
			double x = 0;
			double y = 0;
			Vector2 p1, p2;
			double f;
			for (int i = 0, j = nPts - 1; i < nPts; j = i++) {
				p1 = pts[i];
				p2 = pts[j];
				f = (p1.X - off.X) * (p2.Y - off.Y) - (p2.X - off.X) * (p1.Y - off.Y);
				twicearea += f;
				x += (p1.X + p2.X - 2 * off.X) * f;
				y += (p1.Y + p2.Y - 2 * off.Y) * f;
			}
			f = twicearea * 3;

			return new Vector2((int)(x / f + off.X), (int)(y / f + off.Y));
		}

		public static double Magnitude(Vector2 p) {
			return Math.Sqrt(p.X * p.X + p.Y * p.Y);
		}

		/*
		 * C# port of three.js (v 90) / ShapePath.js / IsPointInsidePolygon (...)
		 * @author zz85 / http://www.lab4games.net/zz85/blog
		 * 
		 * MIT License
		 * https://opensource.org/licenses/MIT
		 * 
		 * Copyright (C) 2010-2018 three.js authors (https://threejs.org)
		 * 
		 */
		public static bool Contains ( List<Vector2> path, Vector2 point ) {

			int polyLen = path.Count;

			// inPt on polygon contour => immediate success    or
			// toggling of inside/outside at every single! intersection point of an edge
			//  with the horizontal line through inPt, left of inPt
			//  not counting lowerY endpoints of edges and whole edges on that line
			bool inside = false;
			for ( int p = polyLen - 1, q = 0; q < polyLen; p = q++ ) {

				var edgeLowPt = path[ p ];
				var edgeHighPt = path[ q ];

				var edgeDx = edgeHighPt.X - edgeLowPt.X;
				var edgeDy = edgeHighPt.Y - edgeLowPt.Y;

				if ( Math.Abs ( edgeDy ) > float.Epsilon ) {

					// not parallel
					if ( edgeDy < 0 ) {

						edgeLowPt = path [ q ]; edgeDx = -edgeDx;
						edgeHighPt = path [ p ]; edgeDy = -edgeDy;
					}
					if ( ( point.Y < edgeLowPt.Y ) || ( point.Y > edgeHighPt.Y ) ) continue;

					if ( point.Y == edgeLowPt.Y ) {

						if ( point.X == edgeLowPt.Y ) return true;      // inPt is on contour ?
																		// continue;				// no intersection or edgeLowPt => doesn't count !!!
					}
					else {

						var perpEdge = edgeDy * ( point.X - edgeLowPt.X ) - edgeDx * ( point.Y - edgeLowPt.Y );
						if ( perpEdge == 0 ) return true;      // inPt is on contour ?
						if ( perpEdge < 0 ) continue;
						inside = !inside;       // true intersection left of inPt
					}
				}
				else {

					// parallel or collinear
					if ( point.Y != edgeLowPt.Y ) continue;         // parallel
																	// edge lies on the same horizontal line as inPt
					if ( ( ( edgeHighPt.X <= point.X ) && ( point.X <= edgeLowPt.X ) ) ||
						 ( ( edgeLowPt.X <= point.X ) && ( point.X <= edgeHighPt.X ) ) ) return true; // inPt: Point on contour !
																									  // continue;
				}
			}

			return inside;
		}

		// line intercept math by Paul Bourke http://paulbourke.net/geometry/pointlineplane/
		// Determine the intersection point of two line segments
		// Return FALSE if the lines don't intersect
		public static bool SegmentIntersect ( Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection ) {
			return SegmentIntersect( p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, out intersection);
		}

		public static bool SegmentIntersect(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, out Vector2 intersection) {

			// Check if none of the lines are of length 0
			if ((x1 == x2 && y1 == y2) || (x3 == x4 && y3 == y4)) {
				intersection = default(Vector2);
				return false;
			}

			float denominator = ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

			// Lines are parallel
			if (Math.Abs(denominator) < float.Epsilon) {
				intersection = default(Vector2);
				return false;
			}

			float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denominator;
			float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denominator;

			// is the intersection along the segments
			if (ua < 0 || ua > 1 || ub < 0 || ub > 1) {
				intersection = default(Vector2);
				return false;
			}

			// Return a object with the x and y coordinates of the intersection
			float x = x1 + ua * (x2 - x1);
			float y = y1 + ua * (y2 - y1);

			intersection = new Vector2((int)x, (int)y);
			return true;
		}

		// https://stackoverflow.com/a/1501725/1712403
		public static double DistanceToSegmentSquared(Vector2 p, Vector2 v, Vector2 w) {
			float dx = w.X - v.X,
				dy = w.Y - v.Y;
			float l2 = dx * dx + dy * dy;
			if (l2 == 0) return (dx = v.X - p.X) * dx + (dy = v.Y - p.Y) * dy;
			float t = Math.Max(0, Math.Min(1, (((p.X - v.X) * dx + (p.Y - v.Y) * dy) / l2)));

			dx = v.X + t * (w.X - v.X) - p.X;
			dy = v.Y + t * (w.Y - v.Y) - p.Y;
			return dx * dx + dy * dy;
		}

		public static double DistanceToSegment ( Vector2 p, Vector2 v, Vector2 w ) { return Math.Sqrt ( DistanceToSegmentSquared ( p, v, w ) ); }
	}
}