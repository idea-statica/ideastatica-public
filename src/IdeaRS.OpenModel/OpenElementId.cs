using System.Runtime.Serialization;
namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Open element base class
	/// POS - class can not be abstract -it causes problems with serialization
	/// </summary>
	[DataContract]
	public class OpenElementId : OpenObject
	{
		/// <summary>
		/// Element Id
		/// </summary>
		[DataMember]
		public System.Int32 Id { get; set; }
	}
}