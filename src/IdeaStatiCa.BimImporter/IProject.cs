using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter
{
	public interface IProject
	{
		int GetIomId(string bimId);

		IIdeaObject GetBimObject(int id);

		int GetIomId(IIdeaObject obj);

		void Save(string path);

		void Load(string path);
	}
}