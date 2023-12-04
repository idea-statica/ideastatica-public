namespace IdeaStatiCa.CheckbotPlugin.Common.Mappers
{
	public static partial class Mapper
	{
		public static Protos.ProjectInfoResp Map(Models.ProjectInfo source)
		{
			return new Protos.ProjectInfoResp
			{
				Name = source.Name,
				CountryCode = MapCountryCode(source.CountryCode),
				// TODO: CountryCodeConfig = ???
			};
		}

		public static Models.ProjectInfo Map(Protos.ProjectInfoResp source)
		{
			return new Models.ProjectInfo(source.Name, MapCountryCode(source.CountryCode));
		}

		private static Protos.CountryCode MapCountryCode(IdeaRS.OpenModel.CountryCode source)
		{
			return source switch
			{
				// TODO: IdeaRS.OpenModel.CountryCode.None => ???
				IdeaRS.OpenModel.CountryCode.ECEN => Protos.CountryCode.Ecen,
				IdeaRS.OpenModel.CountryCode.India => Protos.CountryCode.India,
				// TODO: IdeaRS.OpenModel.CountryCode.SIA => ???
				IdeaRS.OpenModel.CountryCode.American => Protos.CountryCode.American,
				IdeaRS.OpenModel.CountryCode.Canada => Protos.CountryCode.Canada,
				IdeaRS.OpenModel.CountryCode.Australia => Protos.CountryCode.Australia,
				IdeaRS.OpenModel.CountryCode.RUS => Protos.CountryCode.Rus,
				IdeaRS.OpenModel.CountryCode.CHN => Protos.CountryCode.Chn,
				IdeaRS.OpenModel.CountryCode.HKG => Protos.CountryCode.Hkg,
				_ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(source), (int)source, typeof(IdeaRS.OpenModel.CountryCode))
			};
		}

		private static IdeaRS.OpenModel.CountryCode MapCountryCode(Protos.CountryCode source)
		{
			return source switch
			{
				Protos.CountryCode.Ecen => IdeaRS.OpenModel.CountryCode.ECEN,
				Protos.CountryCode.American => IdeaRS.OpenModel.CountryCode.American,
				Protos.CountryCode.Canada => IdeaRS.OpenModel.CountryCode.Canada,
				Protos.CountryCode.Australia => IdeaRS.OpenModel.CountryCode.Australia,
				Protos.CountryCode.Rus => IdeaRS.OpenModel.CountryCode.RUS,
				Protos.CountryCode.Chn => IdeaRS.OpenModel.CountryCode.CHN,
				Protos.CountryCode.India => IdeaRS.OpenModel.CountryCode.India,
				Protos.CountryCode.Hkg => IdeaRS.OpenModel.CountryCode.HKG,
				_ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(source), (int)source, typeof(Protos.CountryCode))
			};
		}
	}
}