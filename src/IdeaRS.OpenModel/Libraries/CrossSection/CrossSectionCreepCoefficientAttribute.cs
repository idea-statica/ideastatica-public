using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Cross-section creep coefficient attribute
	/// </summary>
	/// <example> 
	/// This sample shows how to create attributes for creep calculation.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// //Set creep definition - on component 0, creep and notional size are calculated, humidity 60%
	/// CrossSectionCreepCoefficientAttribute creepAttr = new CrossSectionCreepCoefficientAttribute();
	/// creepAttr.Element = new ReferenceElement(css);
	/// CrossSectionCreepCoefficientData creepComp = new CrossSectionCreepCoefficientData();
	/// creepComp.CreepInput = InputValueType.Calculate;
	/// creepComp.ComponentIndex = new List&lt;int&gt;() { 0 };
	/// creepComp.Humidity = 0.6;
	/// creepComp.NotionalSizeInput = InputValueType.Calculate;
	/// creepAttr.Components.Add(creepComp);
	/// openModel.AddObject(creepAttr);
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Libraries.CrossSection.ComponentsCreepCoefficientAttribute,CI.CrossSection")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class CrossSectionCreepCoefficientAttribute : OpenAttribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CrossSectionCreepCoefficientAttribute()
		{
			Components = new List<CrossSectionCreepCoefficientData>();
		}

		/// <summary>
		/// List of components
		/// </summary>
		public List<CrossSectionCreepCoefficientData> Components { get; set; }
	}

	/// <summary>
	/// Input value type
	/// </summary>
	[DataContract]
	public enum InputValueType
	{
		/// <summary>
		/// Calculate by the code
		/// </summary>
		Calculate = 0,

		/// <summary>
		/// User input
		/// </summary>
		UserInput = 1
	}

	/// <summary>
	/// Cross-section component creep coefficient data
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.CrossSection.ComponentCreepCoefficientDataItem,CI.CrossSection")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class CrossSectionCreepCoefficientData
	{
		/// <summary>
		/// Zero based index of component
		/// </summary>
		[XmlArrayItem("Item")]
		public List<int> ComponentIndex { get; set; }

		/// <summary>
		/// User-defined curvature for creep coefficient { x = t[day], y = ϕ }
		/// </summary>
		public InputValueType CreepInput { get; set; }

		/// <summary>
		/// User creep coefficient
		/// </summary>
		public Polygon2D UserCreepCoeeficient { get; set; }

		/// <summary>
		/// Humidity in the relative value
		/// </summary>
		public double Humidity { get; set; }

		/// <summary>
		/// Input type of Notional size
		/// </summary>
		public InputValueType NotionalSizeInput { get; set; }

		/// <summary>
		/// User-defined notional size
		/// </summary>
		public double NotionalSize { get; set; }
	}
}
