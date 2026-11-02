using System.Collections.Generic;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{


	/// <summary>
	/// Result of section on the member
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class ResultOnSection : ResultBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOnSection()
		{
			Results = new List<SectionResultBase>();
			AbsoluteRelative = Result.AbsoluteRelative.Absolute;
		}

		/// <summary>
		/// Absolute of relative value of position is used
		/// </summary>
		public AbsoluteRelative AbsoluteRelative { get; set; }

		/// <summary>
		/// Position of section
		/// </summary>
		public double Position { get; set; }


		/// <summary>
		/// List of result of members
		/// </summary>
		public List<SectionResultBase> Results { get; set; }
	}

	/// <summary>
	/// Absolute of relative value of position is used
	/// </summary>
	public enum AbsoluteRelative
	{
		/// <summary>
		/// Position is defined absolutely
		/// </summary>
		Absolute = 0,

		/// <summary>
		/// Position is defined relatively
		/// </summary>
		Relative = 1,
	}
}
