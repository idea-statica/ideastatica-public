using CadExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// Base of the importers of CAD model objects, which Checkbot asks for by their CAD id.
	/// </summary>
	internal abstract class BaseImporter<T> : IntIdentifierImporter<T>
		where T : IIdeaObject
	{
		protected BaseImporter(ICadGeometryApi cad)
		{
			Cad = cad;
		}

		/// <summary>
		/// The CAD application the importer reads from.
		/// </summary>
		protected ICadGeometryApi Cad { get; }
	}
}
