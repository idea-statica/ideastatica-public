using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Diagnostics;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal abstract class AbstractImporter<T> : IImporter<T> where T : IIdeaObject
	{
		private readonly static IIdeaLogger _logger = IdeaDiagnostics.GetLogger("ideastatica.bimimporter.abstractimporter");

		public ReferenceElement Import(ImportContext ctx, T obj)
		{
			if (ctx == null)
			{
				throw new ArgumentNullException(nameof(ctx));
			}

			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			_logger.LogDebug($"Importing {obj.GetType().Name}, id '{obj.Id ?? "<empty>"}', name '{obj.Name ?? "<empty>"}'");

			if (string.IsNullOrEmpty(obj.Id))
			{
				throw new Exception("Object must specify non-empty Id.");
			}

			if (ctx.IdeaObjects.TryGetValue(obj.Id, out IIdeaObject ideaObject))
			{
				if (!ReferenceEquals(ideaObject, obj))
				{
					throw new ConstraintException($"Objects '{ideaObject}' and '{obj}' have the same Id '{obj.Id}'.");
				}
			}

			if (ctx.ReferenceElements.TryGetValue(obj.Id, out ReferenceElement refElm))
			{
				_logger.LogDebug($"Reusing already imported object, open model id '{refElm.Id}'");
				return refElm;
			}

			OpenElementId iomElm = ImportInternal(ctx, obj);
			refElm = new ReferenceElement(iomElm);

			ctx.Add(iomElm);
			ctx.ReferenceElements.Add(obj.Id, refElm);
			ctx.IdeaObjects.Add(obj.Id, obj);

			return refElm;
		}

		protected abstract OpenElementId ImportInternal(ImportContext ctx, T obj);
	}
}