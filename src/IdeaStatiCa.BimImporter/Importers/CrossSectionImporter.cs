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
		private readonly IImporter<IIdeaMaterial> _materialImporter;

		public CrossSectionImporter(IImporter<IIdeaMaterial> materialImporter)
		{
			_materialImporter = materialImporter;
		}

		protected override OpenElementId ImportInternal(ImportContext ctx, IIdeaCrossSection css)
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
				throw new NotImplementedException();
			}

			iomCss.Name = css.Name;
			iomCss.CrossSectionRotation = css.Rotation;

			return iomCss;
		}

		private CrossSection CreateCssParametric(ImportContext ctx, IIdeaCrossSectionByParameters cssParametric)
		{
			return new CrossSectionParameter()
			{
				CrossSectionType = cssParametric.Type,
				Material = _materialImporter.Import(ctx, cssParametric.Material),
				Parameters = new List<Parameter>(cssParametric.Parameters)
			};
		}

		private CrossSection CreateCssCentreLine(ImportContext ctx, IIdeaCrossSectionByCenterLine cssCentreLine)
		{
			return new CrossSectionGeneralColdFormed()
			{
				CrossSectionType = cssCentreLine.Type,
				Material = _materialImporter.Import(ctx, cssCentreLine.Material),
				Centerline = cssCentreLine.CenterLine,
				Radius = cssCentreLine.Radius,
				Thickness = cssCentreLine.Thickness
			};
		}

		private CrossSection CreateCssComponents(ImportContext ctx, IIdeaCrossSectionByComponents cssComponents)
		{
			return new CrossSectionComponent()
			{
				Components = cssComponents.Components
					.Select(component => new CssComponent()
					{
						Geometry = component.Geometry,
						Material = _materialImporter.Import(ctx, component.Material),
						Phase = component.Phase
					})
					.ToList()
			};
		}
	}
}