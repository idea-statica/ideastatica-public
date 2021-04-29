using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter
{
	public interface IProject
	{
		ConversionDictionaryString IdMapping { get; }

		int GetIomId(string bimId);

		IIdeaObject GetBimObject(int id);

		int GetIomId(IIdeaObject obj);

		void Load(IGeometry geometry, ConversionDictionaryString conversionTable);
	}
}