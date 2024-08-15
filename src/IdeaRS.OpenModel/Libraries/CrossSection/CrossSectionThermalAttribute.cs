using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Thermal conductivity limit
	/// </summary>
	[DataContract]
	public enum ThermalConductivityLimit
	{
		/// <summary>
		/// Upper limit
		/// </summary>
		Upper,

		/// <summary>
		/// Lower limit
		/// </summary>
		Lower
	}

	/// <summary>
	/// Cross-section thermal attribute
	/// </summary>
	/// <example> 
	/// This sample shows how to create thermal attributes on cross-section.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// Set thermal definition - on component 0, 
	/// CrossSectionThermalAttribute thermalAttr = new CrossSectionThermalAttribute();
	/// thermalAttr.Element = new ReferenceElement(css);
	/// CrossSectionComponentThermalData thermalComp = new CrossSectionComponentThermalData();
	/// thermalComp.MoistureContent = 0.015;
	/// thermalComp.ThermalConductivityLimit = ThermalConductivityLimit.Lower;
	/// thermalComp.ComponentIndex = 0;
	/// thermalAttr.Components.Add(thermalComp);
	/// openModel.AddObject(thermalAttr);
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Libraries.CrossSection.ComponentsThermalAttribute,CI.CrossSection")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class CrossSectionThermalAttribute : OpenAttribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CrossSectionThermalAttribute()
		{
			Components = new List<CrossSectionComponentThermalData>();
		}

		/// <summary>
		/// List of components
		/// </summary>
		public List<CrossSectionComponentThermalData> Components { get; set; }
	}

	/// <summary>
	/// Cross-section component thermal data
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.CrossSection.ComponentThermalDataItem,CI.CrossSection")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class CrossSectionComponentThermalData
	{
		/// <summary>
		/// Zero based index of component
		/// </summary>
		public int ComponentIndex { get; set; }

		/// <summary>
		/// Moisture content
		/// </summary>
		public double MoistureContent { get; set; }

		/// <summary>
		/// Thermal conductivity limit
		/// </summary>
		public ThermalConductivityLimit ThermalConductivityLimit { get; set; }
	}
}
