using CI.Geometry3D;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace IdeaStatiCa.BIM.Common
{
	/// <summary>
	/// One end of a member set against the member nearest to it: where the end lies against that member's contact band,
	/// the test <see cref="ItemsSorter"/> joins a passing member by, and how far that member's centre line stays outside
	/// the box a node at this end starts with.
	/// </summary>
	internal sealed class MemberEndMiss
	{
		private MemberEndMiss(
			Member member,
			bool atBegin,
			Member nearest,
			double relativePosition,
			Point positionFromAxis,
			Rect band,
			Point positionFromCentreLine,
			double nodeBoxOverflow)
		{
			Member = member;
			AtBegin = atBegin;
			Nearest = nearest;
			RelativePosition = relativePosition;
			PositionFromAxis = positionFromAxis;
			Band = band;
			PositionFromCentreLine = positionFromCentreLine;
			NodeBoxOverflow = nodeBoxOverflow;
		}

		public Member Member { get; }

		public bool AtBegin { get; }

		public IPoint3D Location => AtBegin ? Member.Begin : Member.End;

		/// <summary>The member whose centre line passes closest to <see cref="Location"/>; null when there is no other.</summary>
		public Member Nearest { get; }

		/// <summary>Where <see cref="Location"/> projects along <see cref="Nearest"/>: 0 at its begin, 1 at its end, outside that range past either.</summary>
		public double RelativePosition { get; }

		/// <summary>
		/// <see cref="Location"/> in <see cref="Nearest"/>'s cross-section plane, measured from its LCS axis - the point the
		/// contact test checks against <see cref="Band"/>.
		/// </summary>
		public Point PositionFromAxis { get; }

		/// <summary>The contact band of <see cref="Nearest"/>, in the frame of <see cref="PositionFromAxis"/>.</summary>
		public Rect Band { get; }

		/// <summary>
		/// <see cref="PositionFromAxis"/> measured from the nearest point of <see cref="Nearest"/>'s centre line instead.
		/// The two differ when the LCS origin is off the centre line, and the band then sits off the part.
		/// </summary>
		public Point PositionFromCentreLine { get; }

		/// <summary>
		/// How far the nearest point of <see cref="Nearest"/>'s centre line lies outside the box a node at this end starts
		/// with; zero when inside.
		/// </summary>
		public double NodeBoxOverflow { get; }

		/// <summary>
		/// Measures both ends of each of <paramref name="members"/> against the nearest other member of
		/// <paramref name="among"/>.
		/// </summary>
		public static IReadOnlyList<MemberEndMiss> Measure(IEnumerable<Member> members, IEnumerable<Member> among, SorterSettings settings)
		{
			var candidates = among.ToList();
			var misses = new List<MemberEndMiss>();
			foreach (var member in members)
			{
				misses.Add(MeasureEnd(member, atBegin: true, candidates, settings));
				misses.Add(MeasureEnd(member, atBegin: false, candidates, settings));
			}

			return misses;
		}

		private static MemberEndMiss MeasureEnd(Member member, bool atBegin, List<Member> candidates, SorterSettings settings)
		{
			var location = atBegin ? member.Begin : member.End;
			var node = new ItemsSorter.Node(0, location, ItemsSorter.NodeBox(member, atBegin, settings), member);

			MemberEndMiss nearest = null;
			var nearestDistance = double.PositiveInfinity;
			foreach (var candidate in candidates)
			{
				if (ItemComparer<Member>.Instance.Equals(candidate, member))
				{
					continue;
				}

				var inCandidate = candidate.LCS.TransformToLCS(location);
				var relativePosition = candidate.PositionAlong(inCandidate);
				var onCentreLine = candidate.GetPointOnRelativePosition(relativePosition);
				var distance = GeomOperation.Distance(location, onCentreLine);
				if (double.IsNaN(distance) || distance >= nearestDistance)
				{
					continue;
				}

				var centreLineInCandidate = candidate.LCS.TransformToLCS(onCentreLine);
				nearestDistance = distance;
				nearest = new MemberEndMiss(
					member,
					atBegin,
					candidate,
					relativePosition,
					new Point(inCandidate.Y, inCandidate.Z),
					candidate.ContactBand(settings),
					new Point(inCandidate.Y - centreLineInCandidate.Y, inCandidate.Z - centreLineInCandidate.Z),
					node.BoxOverflow(onCentreLine.ToMediaPoint(), node.Surroundings).Length);
			}

			return nearest ?? new MemberEndMiss(member, atBegin, null, double.NaN, default(Point), Rect.Empty, default(Point), double.NaN);
		}
	}
}
