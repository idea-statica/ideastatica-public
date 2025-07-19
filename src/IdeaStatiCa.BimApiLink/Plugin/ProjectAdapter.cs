using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	internal class ProjectAdapter : IProject
	{
		private readonly IProject _project;
		private readonly IBimApiImporter _bimApiImporter;

		public ProjectAdapter(IProject project, IBimApiImporter bimApiImporter)
		{
			_project = project;
			_bimApiImporter = bimApiImporter;
		}

		public string GetBimApiId(int iomId)
			=> _project.GetBimApiId(iomId);

		public IIdeaObject GetBimObject(int id)
		{
			IIdeaObject obj = _bimApiImporter.Get(_project.GetIdentifier(id));

			return obj;
		}

		public int GetIomId(string bimApiId)
			=> _project.GetIomId(bimApiId);

		public int GetIomId(IIdeaObject obj)
			=> _project.GetIomId(obj);

		public IIdeaPersistenceToken GetPersistenceToken(int iomId)
			=> _project.GetPersistenceToken(iomId);
	}
}