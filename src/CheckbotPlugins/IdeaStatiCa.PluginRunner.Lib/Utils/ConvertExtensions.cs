using Models = IdeaStatiCa.CheckbotPlugin.Models;
using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Utils
{
	internal static class ModelObjectExtension
	{
		public static Models.ModelObject Convert(this Protos.ModelObject modelObject)
		{
			return new Models.ModelObject(modelObject.Type.Convert(), modelObject.Id);
		}
	}

	internal static class ModelObjectTypeExtension
	{
		public static Models.ModelObjectType Convert(this Protos.ModelObjectType objectType)
		{
			return objectType switch
			{
				Protos.ModelObjectType.ConnectionPoint => Models.ModelObjectType.ConnectionPoint,
				Protos.ModelObjectType.Substructure => Models.ModelObjectType.ConnectionPoint,
				Protos.ModelObjectType.Member => Models.ModelObjectType.ConnectionPoint,
				Protos.ModelObjectType.Node => Models.ModelObjectType.ConnectionPoint,
				Protos.ModelObjectType.CrossSection => Models.ModelObjectType.ConnectionPoint,
				Protos.ModelObjectType.Material => Models.ModelObjectType.ConnectionPoint,
				_ => throw new NotImplementedException(),
			};
		}
	}
}