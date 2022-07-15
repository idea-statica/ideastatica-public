using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal abstract class AbstractImporter<T> : IImporter<T> where T : IIdeaObject
	{
		protected IPluginLogger Logger { get; private set; }

		protected AbstractImporter(IPluginLogger logger)
		{
			Logger = logger;
		}

		public OpenElementId Import(IImportContext ctx, T obj)
		{
			Logger.LogDebug($"Importing {obj.GetType().Name}, id '{obj.Id ?? "<null>"}', name '{obj.Name ?? "<null>"}'");

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

		public object Import(IImportContext ctx, T obj, ConnectionData connectionData)
		{
			Logger.LogDebug($"Importing {obj.GetType().Name}, id '{obj.Id ?? "<null>"}', name '{obj.Name ?? "<null>"}'");

			if (ctx == null)
			{
				throw new ArgumentNullException(nameof(ctx));
			}

			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (connectionData == null)
			{
				throw new ArgumentNullException(nameof(connectionData));
			}

			if (string.IsNullOrEmpty(obj.Id))
			{
				throw new InvalidOperationException("Object must specify non-empty Id.");
			}

			return ImportInternal(ctx, obj, connectionData);
		}

		protected virtual object ImportInternal(IImportContext ctx, T obj, ConnectionData connectionData)
		{
			throw new System.NotImplementedException();
		}
	}
}