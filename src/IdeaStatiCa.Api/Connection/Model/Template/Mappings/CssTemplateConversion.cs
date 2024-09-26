namespace IdeaStatiCa.Api.Connection.Model
{
	public enum CssConversionEnum
	{
		Cleat,
		Stub,
		Other
	}

	public class CssTemplateConversion : BaseTemplateConversion
	{
		public CssType CssType { get; set; } = CssType.All;

		public CssConversionEnum CssConversionType { get; set; } = CssConversionEnum.Other;

		public string ProfileName { get; set; }
		//Height
		//Width
	}
}
