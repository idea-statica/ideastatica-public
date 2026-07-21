using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	internal class ProjectAdapter : IProject, IBimIdMapAccess
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

		public IReadOnlyCollection<(int IomId, string SourceIdToken)> ExportIdMap()
			=> (_project as IBimIdMapAccess)?.ExportIdMap() ?? Array.Empty<(int, string)>();

		public void ImportIdMap(IEnumerable<(int IomId, string SourceIdToken)> entries)
			=> (_project as IBimIdMapAccess)?.ImportIdMap(entries);
	}
}