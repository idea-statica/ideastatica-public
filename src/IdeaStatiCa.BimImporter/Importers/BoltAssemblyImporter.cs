using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class BoltAssemblyImporter : AbstractImporter<IIdeaBoltAssembly>
	{
		public BoltAssemblyImporter(IPluginLogger logger) : base(logger)
		{
		}


		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaBoltAssembly boltAssembly)
		{
			switch (boltAssembly)
			{
				case IIdeaBoltAssemblyByName boltAssemblyByName:
					return CreateBoltAssemblyByName(boltAssemblyByName);

				case IIdeaBoltAssemblyByParameters boltAssemblyByParameters:
					return CreateBoltAssemblyByParameters(ctx, boltAssemblyByParameters);
			}

			Logger.LogError($"Bolt Assembly '{boltAssembly.Id}' is of unsupported type '{boltAssembly.GetType().Name}'.");
			throw new ConstraintException($"Bolt Assembly '{boltAssembly.Id}' is of unsupported type '{boltAssembly.GetType().Name}'.");

		}

		private OpenElementId CreateBoltAssemblyByParameters(IImportContext ctx, IIdeaBoltAssemblyByParameters boltAssemblyByParameters)
		{
			return new BoltAssembly()
			{
				Name = boltAssemblyByParameters.Name,
				Diameter = boltAssemblyByParameters.Diameter,
				Borehole = boltAssemblyByParameters.BoreHole,
				HeadDiameter = boltAssemblyByParameters.HeadDiameter,
				DiagonalHeadDiameter = boltAssemblyByParameters.DiagonalHeadDiameter,
				HeadHeight = boltAssemblyByParameters.HeadHeight,
				GrossArea = boltAssemblyByParameters.GrossArea,
				TensileStressArea = boltAssemblyByParameters.TensileStressArea,
				NutThickness = boltAssemblyByParameters.NutThickness,
				WasherThickness = boltAssemblyByParameters.WasherThickness,
				WasherAtHead = boltAssemblyByParameters.WasherAtHead,
				WasherAtNut = boltAssemblyByParameters.WasherAtNut,
				LoadFromLibrary = false,
				BoltGrade = ctx.Import(boltAssemblyByParameters.BoltGrade),
			};
		}

		private OpenElementId CreateBoltAssemblyByName(IIdeaBoltAssemblyByName boltAssemblyByName)
		{
			return new BoltAssembly()
			{
				Name = boltAssemblyByName.Name,
				LoadFromLibrary = true,
			};
		}
	}
}