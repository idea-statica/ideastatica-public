using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Diagnostics;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal abstract class AbstractImporter<T> : IImporter<T> where T : IIdeaObject
	{
		private readonly static IIdeaLogger _logger = IdeaDiagnostics.GetLogger("ideastatica.bimimporter.abstractimporter");

		public OpenElementId Import(IImportContext ctx, T obj)
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

			return ImportInternal(ctx, obj);
		}

		protected abstract OpenElementId ImportInternal(IImportContext ctx, T obj);
	}
}