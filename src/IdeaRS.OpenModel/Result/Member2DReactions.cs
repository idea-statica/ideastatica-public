using System.Collections.Generic;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	[Obfuscation(Feature = "renaming")]
	public class Member2DReactions
	{
		public Member2DReactions()
		{
			BoundaryReactions = new List<ResultOnMember>();
		}

		/// <summary>
		/// Loading
		/// </summary>
		public IdeaRS.OpenModel.Result.Loading Loading { get; set; }

		public int MemberId { get; set; }

		public List<ResultOnMember> BoundaryReactions { get; set; }
	}
}
