using Models = IdeaStatiCa.CheckbotPlugin.Models;
using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Utils
{
	internal static class ModelObjectExtension
	{
		public static Models.ModelObject FromProto(this Protos.ModelObject modelObject)
			=> new(modelObject.Type.FromProto(), modelObject.Id);

		public static Protos.ModelObject ToProto(this Models.ModelObject modelObject)
		{
			return new()
			{
				Id = modelObject.Id,
				Type = modelObject.Type.ToProto()
			};
		}
	}

	internal static class ModelObjectTypeExtension
	{
		public static Models.ModelObjectType FromProto(this Protos.ModelObjectType objectType)
		{
			return objectType switch
			{
				Protos.ModelObjectType.ConnectionPoint => Models.ModelObjectType.ConnectionPoint,
				Protos.ModelObjectType.Substructure => Models.ModelObjectType.Substructure,
				Protos.ModelObjectType.Member => Models.ModelObjectType.Member,
				Protos.ModelObjectType.Node => Models.ModelObjectType.Node,
				Protos.ModelObjectType.CrossSection => Models.ModelObjectType.CrossSection,
				Protos.ModelObjectType.Material => Models.ModelObjectType.Material,
				_ => throw new NotImplementedException(),
			};
		}

		public static Protos.ModelObjectType ToProto(this Models.ModelObjectType objectType)
		{
			return objectType switch
			{
				Models.ModelObjectType.ConnectionPoint => Protos.ModelObjectType.ConnectionPoint,
				Models.ModelObjectType.Substructure => Protos.ModelObjectType.Substructure,
				Models.ModelObjectType.Member => Protos.ModelObjectType.Member,
				Models.ModelObjectType.Node => Protos.ModelObjectType.Node,
				Models.ModelObjectType.CrossSection => Protos.ModelObjectType.CrossSection,
				Models.ModelObjectType.Material => Protos.ModelObjectType.Material,
				_ => throw new NotImplementedException(),
			};
		}
	}

	internal static class ModelExportOptionsExtension
	{
		public static Protos.ModelExportOptions ToProto(this Models.ModelExportOptions options)
		{
			return new Protos.ModelExportOptions()
			{
				AllCrossSectionsAsGeneral = options.AllCrossSectionsAsGeneral,
				WithResults = options.WithResults
			};
		}
	}
}