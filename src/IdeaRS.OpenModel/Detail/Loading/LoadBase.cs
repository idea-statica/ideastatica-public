using System.Runtime.Serialization;
using System.Xml.Serialization;
namespace IdeaRS.OpenModel.Detail.Loading
{
	/// <summary>
	/// Load base object
	/// </summary>
	[XmlInclude(typeof(LineLoad))]
	[XmlInclude(typeof(PointLoad3D))]
	[XmlInclude(typeof(SurfaceLoad3D))]
	[XmlInclude(typeof(ForcesLoad3D))]
	public abstract class LoadBase : OpenObject
	{
		/// <summary>
		/// Element Id
		/// </summary>
		[DataMember]
		public System.Int32 Id { get; set; }
	}
}
