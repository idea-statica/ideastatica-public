using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Data of the pin grid
	/// </summary>
	[DataContract]
	public class PinGrid : FastenerGridBase
	{
		/// <summary>
		/// Replaceable pin
		/// </summary>
		[DataMember]
		public bool IsReplaceable { get; set; }

		/// <summary>
		/// Pin
		/// </summary>
		[DataMember]
		public ReferenceElement Pin { get; set; }
	}
}
