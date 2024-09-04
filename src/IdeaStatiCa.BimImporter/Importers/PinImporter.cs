
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class PinImporter : AbstractImporter<IIdeaPin>
	{
		public PinImporter(IPluginLogger logger) : base(logger)
		{
		}


		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaPin pin)
		{
			switch (pin)
			{
				case IIdeaPinByName pinByName:
					return CreatePinByName(pinByName);

				case IIdeaPinByParameters pinByParameters:
					return CreatePinByParameters(ctx, pinByParameters);
			}

			Logger.LogError($"Pin '{pin.Id}' is of unsupported type '{pin.GetType().Name}'.");
			throw new ConstraintException($"Pin '{pin.Id}' is of unsupported type '{pin.GetType().Name}'.");

		}

		private OpenElementId CreatePinByParameters(IImportContext ctx, IIdeaPinByParameters pinByParameters)
		{
			return new Pin()
			{
				Name = pinByParameters.Name,
				Diameter = pinByParameters.Diameter,
				HasPinCap = pinByParameters.HasPinCap,
				HoleDiameter = pinByParameters.HoleDiameter,
				PinCapDiameter = pinByParameters.PinCapDiameter,
				PinCapThickness = pinByParameters.PinCapThickness,
				PinOverlap = pinByParameters.PinOverlap,
				LoadFromLibrary = false,
				Material = ctx.Import(pinByParameters.Material),
			};
		}

		private OpenElementId CreatePinByName(IIdeaPinByName boltAssemblyByName)
		{
			return new Pin()
			{
				Name = boltAssemblyByName.Name,
				LoadFromLibrary = true,
			};
		}
	}
}