using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Type of member
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public enum MemberType
	{
		/// <summary>
		/// Member 1D
		/// </summary>
		Member1D,

		/// <summary>
		/// Element 1D
		/// </summary>
		Element1D,

		/// <summary>
		/// LineSegment3D - it is used for definition of Member2D boundary reactions
		/// </summary>
		LineSegment3D,
	}

	/// <summary>
	/// Member identification
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class Member
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Member()
		{
			MemberType = MemberType.Member1D;
			Id = 0;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="memberType">Type of member</param>
		/// <param name="id">Id of member</param>
		public Member(MemberType memberType, int id)
		{
			MemberType = memberType;
			Id = id;
		}

		/// <summary>
		/// Type of member
		/// </summary>
		public MemberType MemberType { get; set; }

		/// <summary>
		/// Id of member
		/// </summary>
		public int Id { get; set; }
	}
}