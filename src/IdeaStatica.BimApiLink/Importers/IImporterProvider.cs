using System;

namespace IdeaStatica.BimApiLink.Importers
{
	public interface IImporterProvider
	{
		IImporter GetProvider(Type type);
	}
}