using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.Plugin;

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
	}
}
