using System.Collections.Generic;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Result of the member
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class ResultOnMembers
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOnMembers()
		{
			Members = new List<ResultOnMember>();
		}

		/// <summary>
		/// Loading
		/// </summary>
		public IdeaRS.OpenModel.Result.Loading Loading { get; set; }

		/// <summary>
		/// List of result of members
		/// </summary>
		public List<ResultOnMember> Members { get; set; }
	}
}