using IdeaRS.OpenModel.Concrete;
using IdeaRS.OpenModel.CrossSection;
using System.Xml.Serialization;
namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Open attribute base class
	/// </summary>
	[XmlInclude(typeof(CrossSectionThermalAttribute))]
	[XmlInclude(typeof(CrossSectionCreepCoefficientAttribute))]
	[XmlInclude(typeof(ConcreteMemberDataEc2))]
	public abstract class OpenAttribute : OpenObject
	{
		/// <summary>
		/// Element for attribute
		/// </summary>
		public ReferenceElement Element { get; set; }
	}
}
