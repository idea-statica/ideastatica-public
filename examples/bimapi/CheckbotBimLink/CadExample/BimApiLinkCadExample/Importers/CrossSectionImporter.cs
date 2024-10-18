using BimApiLinkCadExample.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using BimApiLinkCadExample.CadExampleApi;

namespace BimApiLinkCadExample.Importers
{
	internal class CrossSectionImporter : StringIdentifierImporter<IIdeaCrossSection>
	{
		protected ICadGeometryApi Model { get; }

		public CrossSectionImporter(ICadGeometryApi model) 
		{
			Model = model;
		}

		public override IIdeaCrossSection Create(string id)
		{
			//For now only converting Cross-Section By Name,
			////Cross-sections can also be transfered from Parameters

			CadCrossSection css = Model.GetCrossSectionByName(id);

			return new CrossSectionByName(id)
			{
				MaterialNo = Model.GetMaterialByName(css.MaterialId).Id,
				Name = css.Name
			};
		}

		//private static CrossSectionByName GetCrossSectionByName(CadCrossSection css)
		//{
		//	return new CrossSectionByName(css.Name)
		//	{
		//		BoltGradeNo = css.,
		//		Name = css.Name
		//	};
		//}
	}
}