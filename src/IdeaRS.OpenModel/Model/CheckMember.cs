using IdeaRS.OpenModel.Concrete;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Check member base class
	/// </summary>
	[XmlInclude(typeof(CheckMember1D))]
	public abstract class CheckMember : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CheckMember()
		{
		}

		/// <summary>
		/// Name of Element
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// Check meber 1D
	/// </summary>
	[OpenModelClass("CI.Services.Common.CheckMember1D,CI.ServicesCommon", "CI.StructModel.Structure.ICheckMember, CI.BasicTypes", typeof(CheckMember))]
	public class CheckMember1D : CheckMember
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CheckMember1D()
			: base()
		{
		}
	}
}