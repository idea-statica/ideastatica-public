using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Diagnostics;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal abstract class AbstractImporter<T> : IImporter<T> where T : IIdeaObject
	{
		private readonly static IIdeaLogger Logger = IdeaDiagnostics.GetLogger("ideastatica.bimimporter.abstractimporter");

		public ReferenceElement Import(ImportContext ctx, T obj)
		{
			Logger.LogDebug($"Importing {obj.GetType().Name}, id '{obj.Id ?? "<empty>"}', name '{obj.Name ?? "<empty>"}'");

			if (string.IsNullOrEmpty(obj.Id))
			{
				throw new Exception("Object must specify non-empty Id.");
			}

			if (ctx.ReferenceElements.TryGetValue(obj.Id, out ReferenceElement refElm))
			{
				Logger.LogDebug($"Reusing already imported object, open model id '{refElm.Id}'");
				return refElm;
			}

			return ImportInternal(ctx, obj);
		}

		protected abstract ReferenceElement ImportInternal(ImportContext ctx, T obj);
	}
}