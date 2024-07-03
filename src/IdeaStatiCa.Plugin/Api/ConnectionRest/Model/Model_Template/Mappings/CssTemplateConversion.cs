namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Template
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
