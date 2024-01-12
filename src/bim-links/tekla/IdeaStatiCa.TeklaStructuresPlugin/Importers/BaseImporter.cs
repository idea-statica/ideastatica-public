using IdeaStatiCa.BimApi;
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
	}
}
