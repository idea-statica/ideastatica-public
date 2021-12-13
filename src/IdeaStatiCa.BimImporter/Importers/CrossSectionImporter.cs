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
				case IIdeaCrossSectionByName cssParametric:
					return CreateCssParametric(ctx, cssParametric);

				case IIdeaCrossSectionByCenterLine cssCentreLine:
					return CreateCssCentreLine(ctx, cssCentreLine);

				case IIdeaCrossSectionByComponents cssComponents:
					return CreateCssComponents(ctx, cssComponents);

				case IIdeaCrossSectionByName cssNamed:
					return CreateCssNamed(ctx, cssNamed);
			}

			Logger.LogError($"Cross-section '{css.Id}' is of unsupported type '{css.GetType().Name}'.");
			throw new ConstraintException($"Cross-section '{css.Id}' is of unsupported type '{css.GetType().Name}'.");
		}

		private CrossSection CreateCssParametric(IImportContext ctx, IIdeaCrossSectionByName cssParametric)
		{
			Logger.LogTrace($"Importing cross-section {cssParametric.Id} by parameters");

			if (cssParametric.Type == CrossSectionType.OneComponentCss)
			{
				Logger.LogError($"Cross-section '{cssParametric.Id}' must not be of type {nameof(CrossSectionType.OneComponentCss)}.");
				throw new ConstraintException($"Cross-section '{cssParametric.Id}' must not be of type {nameof(CrossSectionType.OneComponentCss)}.");
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
			Logger.LogTrace($"Importing cross-section {cssCentreLine.Id} by center line");

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
			HashSet<IIdeaCrossSectionComponent> components = cssComponents.Components;

			Logger.LogTrace($"Importing cross-section {cssComponents.Id} with {components.Count} components.");

			return new CrossSectionComponent()
			{
				Components = components
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
			string name = cssNamed.Name;

			Logger.LogTrace($"Importing cross-section {cssNamed.Id} by name '{name}'.");

			if (string.IsNullOrEmpty(name))
			{
				Logger.LogError($"Cross-section '{cssNamed.Id}' has empty/null name.");
				throw new ConstraintException($"Cross-section '{cssNamed.Id}' has empty/null name.");
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
						Value = name
					}
				}
			};
		}
	}
}