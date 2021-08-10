using IdeaRS.OpenModel;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class CrossSectionImporter : AbstractImporter<IIdeaCrossSection>
	{
		public CrossSectionImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaCrossSection css)
		{
			CrossSection iomCss = CreateCrossSection(ctx, css);

			iomCss.Name = css.Name;
			iomCss.CrossSectionRotation = css.Rotation;

			return iomCss;
		}

		private CrossSection CreateCrossSection(IImportContext ctx, IIdeaCrossSection css)
		{
			switch (css)
			{
				case IIdeaCrossSectionByParameters cssParametric:
					return CreateCssParametric(ctx, cssParametric);

				case IIdeaCrossSectionByCenterLine cssCentreLine:
					return CreateCssCentreLine(ctx, cssCentreLine);

				case IIdeaCrossSectionByComponents cssComponents:
					return CreateCssComponents(ctx, cssComponents);

				case IIdeaCrossSectionByName cssNamed:
					return CreateCssNamed(ctx, cssNamed)
			}
			throw new ConstraintException($"Unsupported cross-section type '{css.GetType().Name}'.");
		}

		private CrossSection CreateCssParametric(IImportContext ctx, IIdeaCrossSectionByParameters cssParametric)
		{
			if (cssParametric.Type == CrossSectionType.OneComponentCss)
			{
				throw new ConstraintException($"Cross-section type cannot be {nameof(CrossSectionType.OneComponentCss)}, " +
					$"use {nameof(IIdeaCrossSectionByComponents)}.");
			}

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

		private CrossSection CreateCssNamed(IImportContext ctx, IIdeaCrossSectionByName cssNamed)
		{
			if (cssNamed.Name is null)
			{
				throw new ConstraintException($"Name property must not be null for {nameof(IIdeaCrossSectionByName)}.");
			}

			return new CrossSectionParameter()
			{
				CrossSectionType = CrossSectionType.UniqueName,
				Material = ctx.Import(cssNamed.Material),
				Parameters = new List<Parameter>()
				{
					new ParameterString()
					{
						Name = "UniqueName",
						Value = cssNamed.Name
					}
				}
			};
		}
	}
}