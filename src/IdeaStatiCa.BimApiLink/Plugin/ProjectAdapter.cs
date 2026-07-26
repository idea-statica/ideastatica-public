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
			// Return null for an id the link cannot resolve — one with no stored persistence token, or a token that is
			// not an identifier. Callers (BimImporter.ImportGroup, CadApplication) filter nulls by design; the underlying
			// GetIdentifier/GetPersistenceToken throw for such ids, which would otherwise abort the whole group and fail
			// a Connections sync when a group carries a derived, un-tokenised entity (#35688).
			try
			{
				return _bimApiImporter.Get(_project.GetIdentifier(id));
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}
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