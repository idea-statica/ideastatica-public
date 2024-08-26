using System.Runtime.Serialization;

namespace IdeaStatiCa.Api.Connection.Model
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
		public string NewValue { get; set; }
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
	}
}