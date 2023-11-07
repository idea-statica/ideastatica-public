using System;

namespace IdeaStatiCa.CheckbotPlugin.Common.Mappers
{
	public static partial class Mapper
	{
		public static Protos.ModelObject Map(Models.ModelObject source)
		{
			return new Protos.ModelObject
			{
				Type = MapModelObjectType(source.Type),
				Id = source.Id
			};
		}

		public static Models.ModelObject Map(Protos.ModelObject source)
		{
			return new Models.ModelObject(MapModelObjectType(source.Type), source.Id);
		}

		private static Protos.ModelObjectType MapModelObjectType(Models.ModelObjectType source)
		{
			return source switch
			{
				Models.ModelObjectType.ConnectionPoint => Protos.ModelObjectType.ConnectionPoint,
				Models.ModelObjectType.Substructure => Protos.ModelObjectType.Substructure,
				Models.ModelObjectType.Member => Protos.ModelObjectType.Member,
				Models.ModelObjectType.Node => Protos.ModelObjectType.Node,
				Models.ModelObjectType.CrossSection => Protos.ModelObjectType.CrossSection,
				Models.ModelObjectType.Material => Protos.ModelObjectType.Material,
				_ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(source), (int)source, typeof(Models.ModelObjectType))
			};
		}

		private static Models.ModelObjectType MapModelObjectType(Protos.ModelObjectType source)
		{
			return source switch
			{
				Protos.ModelObjectType.ConnectionPoint => Models.ModelObjectType.ConnectionPoint,
				Protos.ModelObjectType.Substructure => Models.ModelObjectType.Substructure,
				Protos.ModelObjectType.Member => Models.ModelObjectType.Member,
				Protos.ModelObjectType.Node => Models.ModelObjectType.Node,
				Protos.ModelObjectType.CrossSection => Models.ModelObjectType.CrossSection,
				Protos.ModelObjectType.Material => Models.ModelObjectType.Material,
				_ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(source), (int)source, typeof(Protos.ModelObjectType))
			};
		}
	}
}