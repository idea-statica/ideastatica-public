using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace ImporterWrappers
{
	public class LoadCaseImporterBase : IntIdentifierImporter<IIdeaLoadCase>
	{
		public override IIdeaLoadCase? Create(int id)
		{
			throw new NotImplementedException();
		}
	}
}
