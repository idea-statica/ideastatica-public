using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.Extension
{
	public static class ModelExtension
	{
		/// <summary>
		/// Get element represent OpenModel or Model in ModelBIM
		/// </summary>
		/// <param name="model"></param>
		public static ISIntermediate GetModelElement(this SModel model, bool throwOnError = true)
		{
			if (model.TryGetFirstElement("OpenModel", out ISIntermediate openModel))
			{
				return openModel;
			}

			if (model.TryGetFirstElement("ModelBIM;Model", out ISIntermediate modelBim))
			{
				return modelBim;
			}

			if (model.TryGetFirstElement("ArrayOfModelBIM;ModelBIM;Model", out ISIntermediate arrayOfModelBimFirst))
			{
				return arrayOfModelBimFirst;
			}

			if (throwOnError)
			{
				throw new InvalidOperationException($"VersioningService GetModelElement - model not found. Root item: {model?.RootItem?.GetElementName()}");
			}

			return null;
		}
	}
}
