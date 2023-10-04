using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using BimApiLinkCadExample.CadExampleApi;

namespace BimApiLinkCadExample.Importers
{
	internal abstract class BaseImporter<T> : IntIdentifierImporter<T> where T : IIdeaObject
	{
		protected ICadGeometryApi Model { get; }

		protected BaseImporter(ICadGeometryApi model)
		{
			Model = model;
		}
	}
}
