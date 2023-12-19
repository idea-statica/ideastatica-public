using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal abstract class BaseImporter<T> : StringIdentifierImporter<T>
		where T : IIdeaObject
	{
		protected ModelClient Model { get; }

		protected IPluginLogger PlugInLogger { get; }

		protected BaseImporter(ModelClient model, IPluginLogger plugInLogger)
		{
			PlugInLogger = plugInLogger;
			Model = model;
		}
	}
}
