using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IOM.VersioningService.Tools
{
	public static class IRIOMTool
	{
		public static void CreateIOMReferenceElement(SObject sourceObject, string referenceType, string referenceObjectId)
		{
			sourceObject.CreateElementProperty("TypeName").ChangeElementValue(referenceType);
			sourceObject.CreateElementProperty("Id").ChangeElementValue(referenceObjectId);
		}

		public static void CreateProperty(ISIntermediate source, ISIntermediate destination, string propertyName)
		{
			var propertyValue = source.TryGetElementValue(propertyName);
			if (!string.IsNullOrEmpty(propertyValue))
			{
				destination.CreateElementProperty(propertyName).ChangeElementValue(propertyValue);
			}
		}

		public static void CopyProperty(ISIntermediate source, ISIntermediate destination, string sourcePropertyName, string destinationPropertyName)
		{
			var propertyValue = source.TryGetElementValue(sourcePropertyName);
			if (!string.IsNullOrEmpty(propertyValue))
			{
				destination.CreateElementProperty(destinationPropertyName).ChangeElementValue(propertyValue);
			}
		}
	}
}
