using System;
using System.Runtime.Serialization;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Template
{
	/// <summary>
	/// Base class for template conversion instances
	/// All inherited classes must be defined as KnownTypes due to data serialization
	/// </summary>
	[DataContract]
	[KnownType(typeof(BoltAssemblyTemplateConversion))]
	[KnownType(typeof(BoltGradeTemplateConversion))]
	[KnownType(typeof(CleatTemplateConversion))]
	[KnownType(typeof(ConcreteTemplateConversion))]
	[KnownType(typeof(CssTemplateConversion))]
	[KnownType(typeof(ElectrodeTemplateConversion))]
	[KnownType(typeof(MemberTemplateConversion))]
	[KnownType(typeof(PlateMaterialTemplateConversion))]
	[KnownType(typeof(TimberTemplateConversion))]
	[KnownType(typeof(PinTemplateConversion))]
	public class BaseTemplateConversion
	{
		/// <summary>
		/// Original value in the template
		/// </summary>
		[DataMember]
		public string OriginalValue { get; set; }
		/// <summary>
		/// Original id in the template
		/// </summary>
		[DataMember]
		public string OriginalTemplateId { get; set; }
		/// <summary>
		/// New value in the template after it's applied
		/// </summary>
		[DataMember]
		public string NewValue { get => NewElement?.Name; set => NewElement = new SelectedElement(value); }
		/// <summary>
		/// Description
		/// </summary>
		[DataMember]
		public string Description { get; set; }
		/// <summary>
		/// New id value for the template application
		/// </summary>
		[DataMember]
		public string NewTemplateId { get; set; }
		/// <summary>
		/// New element for conversion
		/// It can be defined either as element Name, or with specific MPRL id
		/// </summary>
		[DataMember]
		public SelectedElement NewElement { get; set; }
	}

	[DataContract]
	public class SelectedElement
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid? TableId { get; private set; }

		[DataMember]
		public Guid? ElementId { get; private set; }

		[DataMember]
		public TableContainerType ContainerType { get; set; }

		/// <summary>
		/// Prefferred constructor with specific element Id & table Id
		/// </summary>
		/// <param name="tableId"></param>
		/// <param name="elementId"></param>
		public SelectedElement(Guid tableId, Guid elementId)
		{
			TableId = tableId;
			ElementId = elementId;
		}

		/// <summary>
		/// Constructor for records in template where table & element ID is not available
		/// </summary>
		/// <param name="name"></param>
		public SelectedElement(string name)
		{
			Name = name;
			TableId = null;
			ElementId = null;
		}
	}
}