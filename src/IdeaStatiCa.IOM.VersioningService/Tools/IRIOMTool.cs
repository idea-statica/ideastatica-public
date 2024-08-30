using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IOM.VersioningService.Tools
{
	public static class IRIOMTool
	{
		public static void CreateIOMReferenceElement(SObject sourceObject, string referenceType, string referenceObjectId)
		{
			var referenceElement = sourceObject.CreateElementProperty("ReferenceElement");
			referenceElement.CreateElementProperty("TypeName").ChangeElementValue(referenceType);
			referenceElement.CreateElementProperty("Id").ChangeElementValue(referenceObjectId);
		}

		public static void CreateProperty(ISIntermediate source, ISIntermediate destination, string propertyName)
		{
			var propertyValue = source.TryGetElementValue(propertyName);
			if (!string.IsNullOrEmpty(propertyValue))
			{
				destination.CreateElementProperty(propertyName).ChangeElementValue(propertyValue);
			}
		}
	}
}
