using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using System.Globalization;
using System.Text.RegularExpressions;
using TS = Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class ConcreteBlockImporter : BaseImporter<IIdeaConcreteBlock>
	{
		public ConcreteBlockImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaConcreteBlock Create(string id)
		{
			var item = Model.GetItemByHandler(id);

			if (item == null || !(item is TS.Beam beam))
			{
				PlugInLogger.LogDebug($"ConcreteBlockImporter: Item {id} is not a Beam or is null. Skipping.");
				return null;
			}

			if (beam.Type != TS.Beam.BeamTypeEnum.PAD_FOOTING && beam.Type != TS.Beam.BeamTypeEnum.STRIP_FOOTING)
			{
				PlugInLogger.LogDebug($"ConcreteBlockImporter: Beam {id} is not a concrete footing. Skipping.");
				return null;
			}

			double width, length;
			if (!ExtractDimensions(beam.Profile.ProfileString, out width, out length))
			{
				PlugInLogger.LogDebug($"ConcreteBlockImporter: Failed to extract dimensions from profile '{beam.Profile.ProfileString}' for {id}.");
				return null;
			}

			var ideaConcreteBlock = new ConcreteBlock(id)
			{
				MaterialNo = beam.Material.MaterialString,
				Height = MemberHelper.GetPartLength(beam).MilimetersToMeters(),
				Lenght = length.MilimetersToMeters(),
				Width = width.MilimetersToMeters(),
			};

			PlugInLogger.LogInformation($"ConcreteBlockImporter: Created ConcreteBlock {id} - Material: {ideaConcreteBlock.MaterialNo}, H: {ideaConcreteBlock.Height}, L: {ideaConcreteBlock.Lenght}, W: {ideaConcreteBlock.Width}");
			return ideaConcreteBlock;
		}

		private static bool ExtractDimensions(string input, out double width, out double length)
		{
			width = 0;
			length = 0;

			if (string.IsNullOrWhiteSpace(input))
			{
				return false;
			}

			Match match = Regex.Match(input, @"\s*(\d+(\.\d+)?)\s*[*xX]\s*(\d+(\.\d+)?)\s*");

			if (!match.Success)
			{
				return false;
			}

			bool isWidthParsed = double.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out width);
			bool isLengthParsed = double.TryParse(match.Groups[3].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out length);

			return isWidthParsed && isLengthParsed;
		}
	}
}