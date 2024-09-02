using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Cross-section
	/// </summary>
	[XmlInclude(typeof(CrossSectionParameter))]
	[XmlInclude(typeof(CrossSectionComponent))]
	[XmlInclude(typeof(CrossSectionGeneralColdFormed))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public abstract class CrossSection : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CrossSection()
		{
		}

		/// <summary>
		/// Name of cross-section
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// Rotation of Cross - Section
		/// </summary>
		public double CrossSectionRotation { get; set; }


		/// <summary>
		/// Specifies that the cross-section is in its principal axis.
		/// </summary>
		public bool IsInPrincipal { get; set; }
	}
}