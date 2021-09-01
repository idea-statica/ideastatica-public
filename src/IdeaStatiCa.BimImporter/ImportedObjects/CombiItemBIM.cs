using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.ImportedObjects
{
	public class CombiItemBIM : IIdeaCombiItem
	{
		public IIdeaLoadCase LoadCase { get; set; }

		public double Coeff { get; set; }

		public IIdeaCombiInput Combination { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }
	}
}
