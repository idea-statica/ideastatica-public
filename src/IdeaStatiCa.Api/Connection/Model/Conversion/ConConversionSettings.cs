using IdeaRS.OpenModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaStatiCa.Api.Connection.Model.Conversion
{
	[DataContract]
	public class ConConversionSettings
	{
		[DataMember]
		public CountryCode TargetDesignCode { get; set; }

		[DataMember]
		public List<ConversionMapping> Concrete { get; set; }

		[DataMember]
		public List<ConversionMapping> CrossSections { get; set; }

		[DataMember]
		public List<ConversionMapping> Fasteners { get; set; }

		[DataMember]
		public List<ConversionMapping> Steel { get; set; }

		[DataMember]
		public List<ConversionMapping> Welds { get; set; }

		[DataMember]
		public List<ConversionMapping> BoltGrade { get; set; }
	}

	[DataContract]
	public class ConversionMapping
	{
		[DataMember]
		public string SourceValue { get; set; }

		[DataMember]
		public string TargetValue { get; set; }
	}
}
