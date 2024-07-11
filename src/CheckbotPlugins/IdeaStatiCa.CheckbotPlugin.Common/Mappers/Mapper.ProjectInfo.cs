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

		private static Protos.CountryCode MapCountryCode(Models.CountryCode source)
		{
			return source switch
			{
				Models.CountryCode.ECEN => Protos.CountryCode.Ecen,
				Models.CountryCode.American => Protos.CountryCode.American,
				Models.CountryCode.Canada => Protos.CountryCode.Canada,
				Models.CountryCode.Australia => Protos.CountryCode.Australia,
				Models.CountryCode.RUS => Protos.CountryCode.Rus,
				Models.CountryCode.CHN => Protos.CountryCode.Chn,
				Models.CountryCode.India => Protos.CountryCode.India,
				Models.CountryCode.HKG => Protos.CountryCode.Hkg,
				_ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(source), (int)source, typeof(Models.CountryCode))
			};
		}

		private static Models.CountryCode MapCountryCode(Protos.CountryCode source)
		{
			return source switch
			{
				Protos.CountryCode.Ecen => Models.CountryCode.ECEN,
				Protos.CountryCode.American => Models.CountryCode.American,
				Protos.CountryCode.Canada => Models.CountryCode.Canada,
				Protos.CountryCode.Australia => Models.CountryCode.Australia,
				Protos.CountryCode.Rus => Models.CountryCode.RUS,
				Protos.CountryCode.Chn => Models.CountryCode.CHN,
				Protos.CountryCode.India => Models.CountryCode.India,
				Protos.CountryCode.Hkg => Models.CountryCode.HKG,
				_ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(source), (int)source, typeof(Protos.CountryCode))
			};
		}
	}
}