﻿using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal abstract class AbstractImporter<T> : IImporter<T> where T : IIdeaObject
	{
		//private readonly static IIdeaLogger _logger = IdeaDiagnostics.GetLogger("ideastatica.bimimporter.abstractimporter");
		private readonly IPluginLogger _logger;

		protected AbstractImporter(IPluginLogger logger)
		{
			_logger = logger;
		}

		public OpenElementId Import(IImportContext ctx, T obj)
		{
			_logger.LogDebug($"Importing {obj.GetType().Name}, id '{obj.Id ?? "<null>"}', name '{obj.Name ?? "<null>"}'");

			if (ctx == null)
			{
				throw new ArgumentNullException(nameof(ctx));
			}

			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (string.IsNullOrEmpty(obj.Id))
			{
				throw new InvalidOperationException("Object must specify non-empty Id.");
			}

			return ImportInternal(ctx, obj);
		}

		protected abstract OpenElementId ImportInternal(IImportContext ctx, T obj);
	}
}