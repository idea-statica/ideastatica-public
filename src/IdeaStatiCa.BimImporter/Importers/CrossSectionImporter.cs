using IdeaRS.OpenModel;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class CrossSectionImporter : AbstractImporter<IIdeaCrossSection>
	{
		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaCrossSection css)
		{
			CrossSection iomCss;

			if (css is IIdeaCrossSectionByParameters cssParametric)
			{
				iomCss = CreateCssParametric(ctx, cssParametric);
			}
			else if (css is IIdeaCrossSectionByCenterLine cssCentreLine)
			{
				iomCss = CreateCssCentreLine(ctx, cssCentreLine);
			}
			else if (css is IIdeaCrossSectionByComponents cssComponents)
			{
				iomCss = CreateCssComponents(ctx, cssComponents);
			}
			else
			{
				throw new NotImplementedException("Cross-section must be instance of IIdeaCrossSectionByParameters, " +
					"IIdeaCrossSectionByCenterLine, or IIdeaCrossSectionByComponents");
			}

			iomCss.Name = css.Name;
			iomCss.CrossSectionRotation = css.Rotation;

			return iomCss;
		}

		private CrossSection CreateCssParametric(IImportContext ctx, IIdeaCrossSectionByParameters cssParametric)
		{
			return new CrossSectionParameter()
			{
				CrossSectionType = cssParametric.Type,
				Material = ctx.Import(cssParametric.Material),
				Parameters = new List<Parameter>(cssParametric.Parameters)
			};
		}

		private CrossSection CreateCssCentreLine(IImportContext ctx, IIdeaCrossSectionByCenterLine cssCentreLine)
		{
			return new CrossSectionGeneralColdFormed()
			{
				CrossSectionType = cssCentreLine.Type,
				Material = ctx.Import(cssCentreLine.Material),
				Centerline = cssCentreLine.CenterLine,
				Radius = cssCentreLine.Radius,
				Thickness = cssCentreLine.Thickness
			};
		}

		private CrossSection CreateCssComponents(IImportContext ctx, IIdeaCrossSectionByComponents cssComponents)
		{
			return new CrossSectionComponent()
			{
				Components = cssComponents.Components
					.Select(component => new CssComponent()
					{
						Geometry = component.Geometry,
						Material = ctx.Import(component.Material),
						Phase = component.Phase
					})
					.ToList()
			};
		}
	}
}