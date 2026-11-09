using CadExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using System;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// Imports a material by its name.
	/// </summary>
	internal class MaterialImporter : StringIdentifierImporter<IIdeaMaterial>
	{
		private readonly ICadGeometryApi _cad;

		public MaterialImporter(ICadGeometryApi cad)
		{
			_cad = cad;
		}

		/// <summary>
		/// The material with the given name. Checkbot looks the name up among the IDEA StatiCa library materials of
		/// that type.
		/// </summary>
		public override IIdeaMaterial Create(string name)
		{
			CadMaterial material = _cad.GetMaterial(name);

			return new IdeaMaterialByName(name)
			{
				Name = material.Name,
				MaterialType = ToIdea(material.Type),
			};
		}

		/// <summary>
		/// The IDEA StatiCa material type for a CAD material type.
		/// </summary>
		private static MaterialType ToIdea(CadMaterialType type)
		{
			switch (type)
			{
				case CadMaterialType.Steel:
					return MaterialType.Steel;
				case CadMaterialType.BoltGrade:
					return MaterialType.BoltGrade;
				default:
					throw new NotSupportedException($"Material type {type} is not supported.");
			}
		}
	}
}
