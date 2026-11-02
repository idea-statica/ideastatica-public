using System;
using System.Collections.Generic;
using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	///// <summary>
	///// Type of results
	///// </summary>
	//[Obfuscation(Feature = "renaming")]
	//public enum ResultType
	//{
	//	/// <summary>
	//	/// Internal forces
	//	/// </summary>
	//	InternalForces,

	//	/// <summary>
	//	/// Deformation
	//	/// </summary>
	//	Deformation
	//}

	/// <summary>
	/// Result of the member
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[Obsolete]
	public class ResultOfMember2
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOfMember2()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="member">Member</param>
		public ResultOfMember2(Member member)
		{
			Member = member;
			Results = new List<ResultInSection>();
			//Loading = loading;
			//Increment = increment;
		}

		/// <summary>
		/// Member
		/// </summary>
		public Member Member { get; set; }

		/// <summary>
		/// List of result
		/// </summary>
		public List<ResultInSection> Results { get; set; }
	}

	/// <summary>
	/// Result of member
	/// </summary>
	[Obsolete]
	public class ResultOnMembers2
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOnMembers2()
		{
		}

		/// <summary>
		/// Constructor
		/// <param name="resultType">Type of result</param>
		/// </summary>
		public ResultOnMembers2(ResultType resultType)
		{
			Results = new List<ResultOfMember2>();
			ResultType = resultType;
		}

		/// <summary>
		/// Type of  result
		/// </summary>
		public ResultType ResultType { get; set; }

		/// <summary>
		/// List of results of members
		/// </summary>
		public List<ResultOfMember2> Results { get; set; }
	}
}