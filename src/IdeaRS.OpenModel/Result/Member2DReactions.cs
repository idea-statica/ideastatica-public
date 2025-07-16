using System.Collections.Generic;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// The storage of reactions from surrounding structural  elements for <see cref="IdeaRS.OpenModel.Model.Member2D"/>
	/// </summary>
	/// <typeparam name="T">Type of the primitive values which are stored in sections</typeparam>
	[Obfuscation(Feature = "renaming")]
	public class Member2DReactions<T> where T : struct
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Member2DReactions()
		{
			BoundaryReactions = new List<ValuesInSegmentSections<T>>();
			Loadings = new List<Loading>();
		}


		/// <summary>
		/// ID of <see cref="IdeaRS.OpenModel.Model.Member2D"/> in <see cref="IdeaRS.OpenModel.OpenModel"/>
		/// </summary>
		public int MemberId { get; set; }

		/// <summary>
		/// Definitions of loadings which are stored
		/// </summary>
		public List<Loading> Loadings { get; set; }

		/// <summary>
		/// Reactions on the boundary <see cref="IdeaRS.OpenModel.Geometry3D.PolyLine3D"/>
		/// Values correspond to each other by their array indices
		/// </summary>
		public List<ValuesInSegmentSections<T>> BoundaryReactions { get; set; }

		/// <summary>
		/// Unbalanced forces in inner points in the imported area of the member.
		/// Each point is define by mesh intersection of the member.
		/// </summary>
		public List<ValuesInPoint<T>> UnbalancedForcesInInnerPoints { get; set; }
	}
}
