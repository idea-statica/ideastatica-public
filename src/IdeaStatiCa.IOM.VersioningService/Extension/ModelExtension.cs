using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using System;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.Extension
{
	static public class ModelExtension
	{
		/// <summary>
		/// Get element represent OpenModel or Model in ModelBIM
		/// </summary>
		/// <param name="model"></param>
		static public ISIntermediate GetModelElement(this SModel model)
		{
			var openmodelCollection = model.GetElements("OpenModel");
			if (openmodelCollection != null && openmodelCollection.Count() == 1)
			{
				return openmodelCollection.First();
			}

			var modelCollection = model.GetElements("ModelBIM;Model");
			if (modelCollection != null && modelCollection.Count() == 1)
			{
				return modelCollection.First();
			}

			var modelBIMCollection = model.GetElements("ArrayOfModelBIM;ModelBIM");
			if (modelBIMCollection != null && modelBIMCollection.Any())
			{
				var modelBIM = modelBIMCollection.First();

				if (modelBIM != null)
				{
					var arraymodelCollection = modelBIM.GetElements("Model");
					if (arraymodelCollection != null && arraymodelCollection.Count() == 1)
					{
						return arraymodelCollection.First();
					}
				}
			}

			throw new InvalidOperationException($"VersioningService GetModelElement - model not found. Root item: {model?.RootItem?.GetElementName()}");
		}
	}
}
