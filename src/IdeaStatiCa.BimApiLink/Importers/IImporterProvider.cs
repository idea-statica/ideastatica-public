using System;

namespace IdeaStatiCa.BimApiLink.Importers
{
	public interface IImporterProvider
	{
		IImporter GetProvider(Type type);
	}
}