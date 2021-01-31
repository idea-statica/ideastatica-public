using System;
using System.Collections.Generic;
using System.Linq;
using CI.Geometry2D;
using CI.GiCL2D;
using CI.Mathematics;

namespace CI.Geometry3D
{
	/// <summary>
	/// Geometry operations related with Region3D
	/// </summary>
	public static partial class GeomOperation
	{
		/// <summary>
		/// Convert Region Points to Point List
		/// </summary>
		/// <param name="region">Geometry Region</param>
		/// <param name="listOfPoint">List of Region point</param>
		public static void ConvertRegionToPointList(IRegion3D region, List<IPoint3D> listOfPoint)
		{
			if (listOfPoint == null || region == null)
			{
				return;
			}

			if (region.Outline.Count == 0)
			{
				return;
			}

			List<IPoint3D> outlinePoints = new List<IPoint3D>();
			ConvertPolylineToPointList(region.Outline, outlinePoints, false);
			if (outlinePoints.Count > 0)
			{
				outlinePoints.RemoveAt(outlinePoints.Count - 1);
				listOfPoint.AddRange(outlinePoints);
			}

			foreach (IPolyLine3D polyline in region.Openings)
			{
				List<IPoint3D> openingPoints = new List<IPoint3D>();
				ConvertPolylineToPointList(polyline, openingPoints, false);
				if (openingPoints.Count > 0)
				{
					openingPoints.RemoveAt(openingPoints.Count - 1);
					listOfPoint.AddRange(openingPoints);
				}
			}
		}

		/// <summary>
		/// Get all segments of from the region (including idges of the openings)
		/// </summary>
		/// <param name="region"></param>
		/// <returns></returns>
		public static IEnumerable<ISegment3D> GetAllSegments(this IRegion3D region)
		{
			foreach(var s in region.Outline.Segments)
			{
				yield return s;
			}

			if (region.Openings != null && region.Openings.Any())
			{
				foreach (var o in region.Openings)
				{
					foreach (var s in o.Segments)
					{
						yield return s;
					}
				}
			}
		}

		/// <summary>
		/// Convert Region3D to Region2D
		/// </summary>
		/// <param name="region3D">3D Region</param>
		/// <returns>2D Region</returns>
		public static IRegion2D ConvertTo2D(IRegion3D region3D)
		{
			IRegion2D region2D = new Region2D
			{
				Outline = ConvertTo2D(region3D.Outline)
			};
			foreach (IPolyLine3D polyline in region3D.Openings)
			{
				region2D.Openings.Add(ConvertTo2D(polyline));
			}

			return region2D;
		}

		/// <summary>
		/// Converting Geometry2D.IRegion2D to Geometry3D.IRegion3D
		/// </summary>
		/// <param name="region2D">Geometry2D.IRegion2D</param>
		/// <returns>Geometry3D.IRegion3D</returns>
		public static IRegion3D ConvertTo3D(IRegion2D region2D)
		{
			if (region2D == null)
			{
				return null;
			}

			var region3D = new Region3D
			{
				Outline = ConvertTo3D(region2D.Outline)
			};
			foreach (var o in region2D.Openings)
			{
				region3D.AddOpening(ConvertTo3D(o));
			}

			return region3D;
		}

		public static double GetLengthOfSmallestArcSegment(IPolyLine3D polyLine)
		{
			double length = 0.0;
			bool isFirst = true;

			foreach (ISegment3D segment in polyLine.Segments)
			{
				if(segment is ArcSegment3D)
				{
					if (isFirst)
					{
						length = GetLength(segment);
						isFirst = false;
					}
					else
					{
						length = Math.Min(length, GetLength(segment));
					}
				}
			}

			return length;
		}

		public static double GetRadiusOfSmallestArcSegment(IPolyLine3D polyline)
		{
			double radius = 0.0;
			bool isFirst = true;

			foreach (ISegment3D segment in polyline.Segments)
			{
				var arc = segment as ArcSegment3D;
				if (arc != null)
				{
					if (isFirst)
					{
						radius = GetRadius(arc);
						isFirst = false;
					}
					else
					{
						radius = Math.Min(radius, GetRadius(arc));
					}
				}
			}

			return radius;
		}

		public static double GetLengthOfSmallestArcSegment(IRegion3D region)
		{
			double length = GetLengthOfSmallestArcSegment(region.Outline);
			if (region.Openings.Any())
			{
				if (length.IsZero(1e-6))
				{
					length = double.PositiveInfinity;
				}

				foreach (IPolyLine3D opening in region.Openings)
				{
					var l = GetLengthOfSmallestArcSegment(opening);
					if (!l.IsZero(1e-6))
					{
						length = Math.Min(length, l);
					}
				}

				if (double.IsPositiveInfinity(length))
				{
					return 0;
				}
			}

			return length;
		}

		public static double GetRadiusOfSmallestArcSegment(IRegion3D region)
		{
			var radius = GetRadiusOfSmallestArcSegment(region.Outline);
			if (region.Openings.Any())
			{
				if (radius.IsZero(1e-6))
				{
					radius = double.PositiveInfinity;
				}

				foreach (IPolyLine3D opening in region.Openings)
				{
					var l = GetRadiusOfSmallestArcSegment(opening);
					if (!l.IsZero(1e-6))
					{
						radius = Math.Min(radius, l);
					}
				}

				if (double.IsPositiveInfinity(radius))
				{
					return 0;
				}
			}

			return radius;
		}

		/// <summary>
		/// Calculate the Length of smallest segment of given region
		/// </summary>
		/// <param name="region">IRegion3D</param>
		/// <returns>Length of smallest segment of given region</returns>
		public static double GetLengthOfSmallestSegment(IRegion3D region)
		{
			double length = GetLengthOfSmallestSegment(region.Outline);
			foreach (IPolyLine3D opening in region.Openings)
			{
				length = Math.Min(length, GetLengthOfSmallestSegment(opening));
			}

			return length;
		}

		/// <summary>
		/// Prepare new region by divide the arc segments into several linear segments
		/// </summary>
		/// <param name="region">IRegion3D</param>
		/// <param name="distance">Maximum distance of a line segment which is created from curved segments</param>
		/// <returns>New PolyLine3D</returns>
		public static IRegion3D GetLinearSegments(IRegion3D region, double distance)
		{
			IRegion3D newRegion = new Region3D
			{
				Outline = GetLinearSegments(region.Outline, distance)
			};
			foreach (IPolyLine3D opening in region.Openings)
			{
				newRegion.AddOpening(GetLinearSegments(opening, distance));
			}

			return newRegion;
		}

		/// <summary>
		/// Prepare new region by divide the arc segments into several linear segments
		/// </summary>
		/// <param name="region">IRegion3D</param>
		/// <param name="numberOfParts">Number of linear parts</param>
		/// <returns>New PolyLine3D</returns>
		public static IRegion3D GetLinearSegments(IRegion3D region, int numberOfParts)
		{
			IRegion3D newRegion = new Region3D
			{
				Outline = GetLinearSegments(region.Outline, numberOfParts)
			};
			foreach (IPolyLine3D opening in region.Openings)
			{
				newRegion.AddOpening(GetLinearSegments(opening, numberOfParts));
			}

			return newRegion;
		}

		/// <summary>
		/// Calculates LCS matrix of the plane of region according to points
		/// </summary>
		/// <param name="region">region of the plane</param>
		/// <returns>Matrix44 of LCS</returns>
		public static IMatrix44 GetMatrixPlane(IRegion3D region)
		{
			if (region == null)
			{
				return null;
			}

			ISegment3D firstSeg = region.Outline[0];
			Vector3D firstVec = Subtract(firstSeg.StartPoint, firstSeg.EndPoint);
			IPoint3D pointInPlane = null;

			// loop for 1 not 0
			for (int i = 1; i < region.Outline.Count; i++)
			{
				ISegment3D nextSeg = region.Outline[i];
				Vector3D nextVec = Subtract(nextSeg.StartPoint, nextSeg.EndPoint);
				if (!IsCollinear(firstVec, nextVec, MathConstants.ZeroWeak))
				{
					pointInPlane = nextSeg.EndPoint;
					break;
				}
			}

			if (pointInPlane != null)
			{
				return GetLCSMatrix(firstSeg.StartPoint, firstSeg.EndPoint, pointInPlane, Plane.XY);
			}

			return new Matrix44();
		}

		/// <summary>
		/// Prepaer a point on region from given region and point. 
		/// </summary>
		/// <param name="matrix">LCS matrix of baseGeometry</param>
		/// <param name="baseGeometry">IRegion3D</param>
		/// <param name="point">IPoint3D</param>
		/// <returns>New IPointOnRegion object</returns>
		public static IPointOnRegion GetPointOnRegion(IMatrix44 matrix, IRegion3D baseGeometry, IPoint3D point)
		{
			if (baseGeometry == null || point == null)
			{
				return null;
			}

			if (matrix == null)
			{
				matrix = GetMatrixPlane(baseGeometry);
			}

			IPoint3D pointInLCS = matrix.TransformToLCS(point);
			if (pointInLCS.Z.IsZero() == false)
			{
				throw new NotSupportedException("Screen point and selected region are not in same plane");
			}

			IPointOnRegion pointOnRegion = new PointOnRegion(baseGeometry, pointInLCS.X, pointInLCS.Y);
			return pointOnRegion;
		}

		/// <summary>
		/// Convert the points of segment based on baseGeometry
		/// </summary>
		/// <param name="matrix">LCS matrix of baseGeometry</param>
		/// <param name="baseGeometry">Based on this region, the given segment is modifed</param>
		/// <param name="segment">Input segment object</param>
		/// <param name="isIndependent">Segment is independent</param>
		/// <param name="modifyInput">If true, given segment object is modified. Otherwise prepare new segment object</param>
		/// <returns>Segment object which is based on baseGeometry</returns>
		public static ISegment3D GetSegmentOnRegion(IMatrix44 matrix, IRegion3D baseGeometry, ISegment3D segment, bool isIndependent = true, bool modifyInput = true)
		{
			if (baseGeometry == null || segment == null)
			{
				return null;
			}

			if (matrix == null)
			{
				matrix = GetMatrixPlane(baseGeometry);
			}

			if (!modifyInput)
			{
				segment = segment.CloneSegment();
			}

			ILineSegment3D line = segment as ILineSegment3D;
			if (line != null)
			{
				if (isIndependent)
				{
					line.StartPoint = GetPointOnRegion(matrix, baseGeometry, line.StartPoint);
				}

				line.EndPoint = GetPointOnRegion(matrix, baseGeometry, line.EndPoint);
				return line;
			}

			IArcSegment3D arc = segment as IArcSegment3D;
			if (arc != null)
			{
				if (isIndependent)
				{
					arc.StartPoint = GetPointOnRegion(matrix, baseGeometry, arc.StartPoint);
				}

				arc.IntermedPoint = GetPointOnRegion(matrix, baseGeometry, arc.IntermedPoint);
				arc.EndPoint = GetPointOnRegion(matrix, baseGeometry, arc.EndPoint);
				return arc;
			}

			throw new NotSupportedException();
		}

		/// <summary>
		/// Convert the points of polyline based on baseGeometry
		/// </summary>
		/// <param name="matrix">LCS matrix of baseGeometry</param>
		/// <param name="baseGeometry">Based on this region, the given polyline is modifed</param>
		/// <param name="polyline">Input polyline object</param>
		/// <param name="modifyInput">If true, given polyline object is modified. Otherwise prepare new polyline object</param>
		/// <returns>Polyline object which is based on baseGeometry</returns>
		public static IPolyLine3D GetPolylineOnRegion(IMatrix44 matrix, IRegion3D baseGeometry, IPolyLine3D polyline, bool modifyInput = true)
		{
			if (baseGeometry == null || polyline == null)
			{
				return null;
			}

			if (matrix == null)
			{
				matrix = GetMatrixPlane(baseGeometry);
			}

			if (!modifyInput)
			{
				polyline = new PolyLine3D(polyline);
			}

			bool isIndependent = true;
			if (polyline.IsClosed)
			{
				isIndependent = false;
			}

			ISegment3D firstSegment = null;
			IPoint3D lastEndPoint = null;
			foreach (ISegment3D segment in polyline.Segments)
			{
				GetSegmentOnRegion(matrix, baseGeometry, segment, isIndependent);
				if (lastEndPoint != null)
				{
					segment.StartPoint = lastEndPoint;
				}

				if (firstSegment == null)
				{
					firstSegment = segment;
				}

				lastEndPoint = segment.EndPoint;
				isIndependent = false;
			}

			if (firstSegment != null && lastEndPoint != null)
			{
				firstSegment.StartPoint = lastEndPoint;
			}

			return polyline;
		}

		/// <summary>
		/// Convert the points of region based on baseGeometry
		/// </summary>
		/// <param name="baseGeometry">Based on this region, the given region is modifed</param>
		/// <param name="region">Input region object</param>
		/// <param name="modifyInput">If true, given region object is modified. Otherwise prepare new region object</param>
		/// <returns>Region object which is based on baseGeometry</returns>
		public static IRegion3D GetRegionOnRegion(IRegion3D baseGeometry, IRegion3D region, bool modifyInput = true)
		{
			if (baseGeometry == null || region == null)
			{
				return null;
			}

			if (!modifyInput)
			{
				region = new Region3D(region);
			}

			IMatrix44 matrix = GetMatrixPlane(baseGeometry);
			GetPolylineOnRegion(matrix, baseGeometry, region.Outline);
			foreach (IPolyLine3D opening in region.Openings)
			{
				GetPolylineOnRegion(matrix, baseGeometry, opening);
			}

			return region;
		}

		/// <summary>
		/// Calculate relative position of point on polyline.
		/// </summary>
		/// <param name="region">region</param>
		/// <param name="point">Point</param>
		/// <param name="relativeX">Relative Position along local X axis</param>
		/// <param name="relativeY">Relative Position along local Y axis</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>True if point exist in polyline</returns>
		public static bool GetRelativePosition(IRegion3D region, IPoint3D point, ref double relativeX, ref double relativeY, double toleranceLevel = MathConstants.ZeroWeak)
		{
			IMatrix44 matrix = GeomOperation.GetMatrixPlane(region);
			IPoint3D pointInLCS = matrix.TransformToLCS(point);
			if (pointInLCS.Z.IsZero() == false)
			{
				return false;
			}

			relativeX = pointInLCS.X;
			relativeY = pointInLCS.Y;
			return true;
		}

		/// <summary>
		/// Check the given polyline is valid
		/// </summary>
		/// <param name="region">The region to be validated</param>
		/// <returns>True if the polyline is valid</returns>
		public static bool IsValid(IRegion3D region)
		{
			if (region.Outline.IsClosed == false)
			{
				return false;
			}

			if (IsValid(region.Outline) == false)
			{
				return false;
			}

			foreach (IPolyLine3D opening in region.Openings)
			{
				if (opening.IsClosed == false)
				{
					return false;
				}

				if (IsValid(opening) == false)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Creates Matrix44 from Region3D data
		/// </summary>
		/// <param name="region">Region3D</param>
		/// <returns>Matrix44</returns>
		public static Matrix44 MatrixRegion(IRegion3D region)
		{
			Region3D reg = region as Region3D;
			IPoint3D origin = region.Outline[0].StartPoint;
			return new Matrix44(origin, reg.LCS.VecX, reg.LCS.VecY, reg.LCS.VecZ);
		}

		/// <summary>
		/// Validates the arc segment in region.
		/// </summary>
		/// <param name="region">IRegion3D</param>
		/// <returns>Validated Region3D</returns>
		public static IRegion3D GetValidRegion(IRegion3D region)
		{
			if (region == null)
			{
				return null;
			}

			IRegion3D newRegion = new Region3D
			{
				Outline = GetValidPolyLine(region.Outline)
			};
			foreach (IPolyLine3D opening in region.Openings)
			{
				newRegion.AddOpening(GetValidPolyLine(opening));
			}

			return newRegion;
		}

		/// <summary>
		/// Calculate inner partial polyline by intersecting region and polyline
		/// </summary>
		/// <param name="region">IRegion3D</param>
		/// <param name="innerPolyline">Polyline3D used to calculate inner partial polyline</param>
		/// <param name="innerPolylineList">List of inner partial polyline prepared from the given polygon and polyline</param>
		/// <returns>IntersectionResults</returns>
		public static IntersectionResults Intersect(IRegion3D region, IPolyLine3D innerPolyline, ICollection<IPolyLine3D> innerPolylineList)
		{
			if (region == null || innerPolyline == null || innerPolylineList == null)
			{
				return IntersectionResults.Undefined;
			}

			IPolyLine3D outerPolyline = region.Outline;
			List<IPoint3D> listOfIntersectionPoint = new List<IPoint3D>();
			Intersect(outerPolyline, innerPolyline, listOfIntersectionPoint);
			foreach (IPolyLine3D opening in region.Openings)
			{
				Intersect(opening, innerPolyline, listOfIntersectionPoint);
			}

			if (listOfIntersectionPoint.Count < 1)
			{
				IPoint3D point = GetPointOnPolyLine(innerPolyline, 0.5);
				return IsPointOn(region, point);
			}

			SortedSet<double> relativePositionSet = new SortedSet<double>();
			relativePositionSet.Add(0);
			relativePositionSet.Add(1);
			foreach (IPoint3D point in listOfIntersectionPoint)
			{
				double relativePosition = 0;
				if (GetRelativePosition(innerPolyline, point, ref relativePosition))
				{
					if (!relativePositionSet.Contains(relativePosition))
					{
						relativePositionSet.Add(relativePosition);
					}
				}
			}

			IList<ISegment3D> splittedSegments = SplitPolyline(innerPolyline, relativePositionSet);
			if (splittedSegments == null)
			{
				return IntersectionResults.Undefined;
			}

			IntersectionResults interSectionResult = IntersectionResults.Undefined;
			foreach (ISegment3D segment in splittedSegments)
			{
				IPoint3D point = new Point3D();
				if (GetPointOnSegment(segment, 0.5, ref point))
				{
					IntersectionResults result = IsPointOn(region, point);
					interSectionResult |= result;
					if (result == IntersectionResults.Inside || result == IntersectionResults.OnBorderCurve)
					{
						IPolyLine3D polyline = new PolyLine3D();
						if (innerPolylineList.Count < 1)
						{
							polyline.Add(segment);
						}
						else
						{
							IPolyLine3D existingPolyline = innerPolylineList.Last();
							if (existingPolyline.Count < 1)
							{
								polyline.Add(segment);
							}
							else
							{
								if (existingPolyline.Count > 1)
								{
									ISegment3D lastSegment = existingPolyline.Segments.Last();
									if (lastSegment.EndPoint.Equals(segment.StartPoint))
									{
										polyline = existingPolyline;
									}
								}

								polyline.Add(segment);
							}
						}

						if (!innerPolylineList.Contains(polyline))
						{
							innerPolylineList.Add(polyline);
						}
					}
				}
			}

			if (innerPolylineList.Count < 1)
			{
				return IntersectionResults.OnBorderNode | IntersectionResults.Outside;
			}

			return interSectionResult;
		}

		/// <summary>
		/// Determine Point On Region
		/// </summary>
		/// <param name="region">Region</param>
		/// <param name="pointOnRegion">Point to be checked</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>Intersection Results</returns>
		public static IntersectionResults IsPointOn(IRegion3D region, IPoint3D pointOnRegion, double toleranceLevel = MathConstants.ZeroWeak)
		{
			IntersectionResults regionResult = IsPointOn(region.Outline, pointOnRegion, toleranceLevel);
			if (regionResult != IntersectionResults.Inside)
			{
				return regionResult;
			}

			foreach (IPolyLine3D opening in region.Openings)
			{
				IntersectionResults openingResult = IsPointOn(opening, pointOnRegion, toleranceLevel);
				if (openingResult == IntersectionResults.Inside)
				{
					return IntersectionResults.OnInSideOpening;
				}
				else if (openingResult == IntersectionResults.Outside)
				{
					continue;
				}
				else
				{
					return openingResult;
				}
			}

			return regionResult;
		}

		/// <summary>
		/// Move region according to given vector
		/// </summary>
		/// <param name="region">Region to be moved</param>
		/// <param name="displacement">Displacement vector</param>
		public static void Move(IRegion3D region, Vector3D displacement)
		{
			Move(region.Outline, displacement);

			foreach (IPolyLine3D opening in region.Openings)
			{
				Move(opening, displacement);
			}
		}

		/// <summary>
		/// Calculate the points of region for a given interval
		/// </summary>
		/// <param name="region">Geometry Region</param>
		/// <param name="interval">Interval</param>
		/// <param name="listOfPoint">List of region points</param>
		public static void PointsAtInterval(IRegion3D region, double interval, List<IPoint3D> listOfPoint)
		{
			if (listOfPoint == null || region == null)
			{
				return;
			}

			if (region.Outline.Count == 0)
			{
				return;
			}

			if (interval.IsLesserOrEqual(0) || interval.IsGreaterOrEqual(1))
			{
				return;
			}

			PointsAtInterval(region.Outline, interval, listOfPoint);
			foreach (IPolyLine3D polyline in region.Openings)
			{
				List<IPoint3D> openingPoints = new List<IPoint3D>();
				PointsAtInterval(polyline, interval, openingPoints);
				listOfPoint.AddRange(openingPoints);
			}
		}

		/// <summary>
		/// Calculate points at a given position for each segment
		/// </summary>
		/// <param name="region">Geometry Region</param>
		/// <param name="relativePosition">Relative position of each segment</param>
		/// <param name="listOfPoint">List of points</param>
		public static void PointsAtPosition(IRegion3D region, double relativePosition, ICollection<IPoint3D> listOfPoint)
		{
			if (listOfPoint == null || region == null)
			{
				return;
			}

			if (region.Outline.Count == 0)
			{
				return;
			}

			PointsAtPosition(region.Outline, relativePosition, listOfPoint);
			foreach (IPolyLine3D polyline in region.Openings)
			{
				PointsAtPosition(polyline, relativePosition, listOfPoint);
			}
		}

		/// <summary>
		/// Prepare a region without reflex angles
		/// If any segment exists with reflex angle, divide it into two
		/// </summary>
		/// <param name="region">Region</param>
		/// <returns>New region without reflex angle</returns>
		public static IRegion3D SplitSegmentWithReflexAngle(IRegion3D region)
		{
			if (region == null)
			{
				return null;
			}

			IRegion3D newRegion = new Region3D
			{
				Outline = SplitSegmentWithReflexAngle(region.Outline)
			};
			foreach (IPolyLine3D opening in region.Openings)
			{
				newRegion.AddOpening(SplitSegmentWithReflexAngle(opening));
			}

			return newRegion;
		}

		/// <summary>
		/// Transform a given region to GCS using the matrix
		/// </summary>
		/// <param name="matrix">Matrix which is used to transform</param>
		/// <param name="region">Region which is to be transformed</param>
		public static void TransformToGCS(IMatrix44 matrix, IRegion3D region)
		{
			TransformToGCS(matrix, region.Outline);
			foreach (IPolyLine3D opening in region.Openings)
			{
				TransformToGCS(matrix, opening);
			}
		}

		/// <summary>
		/// Transform a given region to LCS using the matrix
		/// </summary>
		/// <param name="matrix">Matrix which is used to transform</param>
		/// <param name="region">Region which is to be transformed</param>
		public static void TransformToLCS(IMatrix44 matrix, IRegion3D region)
		{
			TransformToLCS(matrix, region.Outline);
			foreach (IPolyLine3D opening in region.Openings)
			{
				TransformToLCS(matrix, opening);
			}
		}

		public static IPolyLine3D TrimmedRegion(Matrix44 lcs, IPolyLine3D trimmedRegion, IPolyLine3D trimRegion, out bool done)
		{
			done = false;

			List<IPoint3D> listOfIntersectionPoint = new List<IPoint3D>();
			Intersect(trimmedRegion, trimRegion, listOfIntersectionPoint);
			if (listOfIntersectionPoint.Count > 0)
			{
				done = true;
				IPolyLine3D trimmedRegionCopy = new PolyLine3D(trimmedRegion);
				IPolyLine3D trimRegionCopy = new PolyLine3D(trimRegion);

				IPolyLine2D trimmedPolyLine2D = ConvertTo2D(lcs, trimmedRegionCopy);
				IPolyLine2D trimPolyLine2D = ConvertTo2D(lcs, trimRegionCopy);
				Region2D trimmedRegion2D = new Region2D(trimmedPolyLine2D);
				Region2D trimRegion2D = new Region2D(trimPolyLine2D);

				// odecte od "trimmedRegion2D" "trimRegion2D".
				var clipper = new ClipperController(trimmedRegion2D, trimRegion2D);
				IRegion2D[] regions = clipper.Difference().ToArray();

				if (regions.Length > 0)
				{
					IPolyLine3D adaptedRegion = ConvertTo3D(regions[0].Outline);
					TransformToGCS(lcs, adaptedRegion);
					return adaptedRegion;
				}
			}
			return trimmedRegion;
		}

		/// <summary>
		/// Create a new region as union of two input regions
		/// </summary>
		/// <param name="a">region a</param>
		/// <param name="b">region b</param>
		/// <param name="result">returns result region = a + b</param>
		/// <returns>false if the operation failed</returns>
		public static bool MergeRegions(IRegion3D a, IRegion3D b, out IRegion3D result)
		{
			List<Tuple<ISegment3D, ISegment3D, bool>> foundPairs = new List<Tuple<ISegment3D, ISegment3D, bool>>();
			var res = new Region3D(a);

			//simple solution - regiony se dotýkají, nepřekrývají
			foreach (var segA in res.Outline.Segments)
			{
				foreach(var segB in b.Outline.Segments)
				{
					// default used tolerance is too big MathConstants.ZeroGeneral = 1E-10 
					// tolerance to find the same segments is set to 1E-4m = 0.0001m = 0.1mm, maximum sensitivity to find same points on segment is 1E-6 (tested)
					if (IsEqualWithTolerance(segA, segB, MathConstants.ZeroWeak))
					{
						foundPairs.Add(new Tuple<ISegment3D, ISegment3D, bool>(segA, segB, false));
						break;
					}
					else if (IsEqualWithTolerance(segA, Reverse(segB), MathConstants.ZeroWeak))
					{
						foundPairs.Add(new Tuple<ISegment3D, ISegment3D, bool>(segA, segB, true));
						break;
					}
				}
			}

			if (foundPairs.Any())
			{
				Tuple<ISegment3D, ISegment3D, bool> first = foundPairs[0];
				IPolyLine3D polylineB = new PolyLine3D(b.Outline);
				var segmentB = first.Item2;

				if (!first.Item3)
				{
					polylineB = Reverse(polylineB);
				}

				foreach (var seg in polylineB.Segments)
				{
					if (IsEqual(segmentB, seg) || IsEqual(segmentB, Reverse(seg)))
					{
						segmentB = seg;
					}
				}

				var polyline = new PolyLine3D();
				int segInx = GetIndexOfSegment(polylineB, segmentB);
				for (int i = segInx + 1; i < polylineB.Segments.Count(); i++)
				{
					polyline.Add(polylineB[i].CloneSegment());
				}

				for (int i = 0; i < segInx; i++)
				{
					polyline.Add(polylineB[i].CloneSegment());
				}

				int resSegInx = GetIndexOfSegment(res.Outline, first.Item1);
				var prevSeg = GetPreviousSegment(res.Outline, resSegInx);

				ISegment3D baseSeg = prevSeg;
				foreach (var seg in polyline.Segments)
				{
					res.Outline.InsertSegmentAfterSegment(baseSeg, seg);
					baseSeg = seg;
				}

				int baseSegInx = GetIndexOfSegment(res.Outline, baseSeg);
				var nextSeg = GetNextSegment(res.Outline, baseSegInx);
				res.Outline.Remove(nextSeg);

				foreach (var opening in b.Openings)
				{
					res.AddOpening(opening);
				}

				result = res;
				return true;
			}

			result = null;
			return false;
		}
	}
}