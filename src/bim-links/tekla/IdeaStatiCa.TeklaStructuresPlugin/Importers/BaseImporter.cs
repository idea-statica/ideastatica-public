using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using TSM = Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	public abstract class BaseImporter<T> : StringIdentifierImporter<T>
		where T : IIdeaObject
	{
		protected IModelClient Model { get; }

		protected IPluginLogger PlugInLogger { get; }

		protected BaseImporter(IModelClient model, IPluginLogger plugInLogger)
		{
			PlugInLogger = plugInLogger;
			Model = model;
		}

		public override T Check(Identifier<T> identifier)
		{
			var cachedObject = Model.GetCachedObject(identifier);
			if (cachedObject is T typedObject)
			{
				return typedObject;
			}
			return default;
		}

		/// <summary>
		/// Adds the part a weld or bolt group names to <paramref name="connectedParts"/>, as whichever of plate or
		/// member it was imported as, and reports it when it was imported as neither. <c>CheckMaybe</c> probes the
		/// object cache rather than importing, so a part the walk has not reached yet is silently absent - and
		/// fabrication left holding one part is dropped downstream as unbuildable, taking with it the only thing that
		/// said these two pieces of steel are one connection.
		/// </summary>
		protected void AddConnectedPart(TSM.ModelObject part, List<IIdeaObjectConnectable> connectedParts, string owner, string role)
		{
			if (part == null)
			{
				PlugInLogger.LogInformation($"{owner} {role}: the source names no part");
				return;
			}

			int before = connectedParts.Count;
			AddIfAlreadyImported<IIdeaPlate>(part, connectedParts);
			AddIfAlreadyImported<IIdeaMember1D>(part, connectedParts);
			if (connectedParts.Count != before)
			{
				return;
			}

			var named = part as TSM.Part;
			var describe = named != null
				? $"'{named.Name}' profile '{named.Profile?.ProfileString}'"
				: $"'{part.GetType().Name}'";
			PlugInLogger.LogInformation($"{owner} {role}: part {describe} guid {part.Identifier.GUID} is cached as neither plate nor member - dropped from ConnectedParts");
		}

		private void AddIfAlreadyImported<TConnectable>(TSM.ModelObject part, List<IIdeaObjectConnectable> connectedParts)
			where TConnectable : IIdeaObjectConnectable
		{
			var id = part.Identifier.GUID.ToString();
			if (CheckMaybe<TConnectable>(id) == null)
			{
				return;
			}

			var imported = GetMaybe<TConnectable>(id);
			if (imported != null)
			{
				connectedParts.Add(imported);
			}
		}
	}
}
