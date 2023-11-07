namespace IdeaStatiCa.CheckbotPlugin.Common.Mappers
{
	public static partial class Mapper
	{
		public static Protos.ModelExportOptions Map(Models.ModelExportOptions source)
		{
			return new Protos.ModelExportOptions
			{
				WithResults = source.WithResults,
				AllCrossSectionsAsGeneral = source.AllCrossSectionsAsGeneral
			};
		}

		public static Models.ModelExportOptions Map(Protos.ModelExportOptions source)
		{
			return new Models.ModelExportOptions
			{
				WithResults = source.WithResults,
				AllCrossSectionsAsGeneral = source.AllCrossSectionsAsGeneral
			};
		}
	}
}