using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using Nito.Disposables.Internals;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatica.BimApiLink.Plugin
{
	internal class CadModelAdapter : IIdeaModel
	{
		private CadUserSelection _lastSelection;

		private readonly IBimApiImporter _bimApiImporter;
		private readonly ICadModel _cadModel;

		public CadModelAdapter(IBimApiImporter bimApiImporter, ICadModel cadModel)
		{
			_bimApiImporter = bimApiImporter;
			_cadModel = cadModel;
		}

		public ISet<IIdeaLoading> GetLoads()
		{
			return new HashSet<IIdeaLoading>();
		}

		public ISet<IIdeaMember1D> GetMembers()
		{
			return _cadModel.GetAllMembers()
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();
		}

		public OriginSettings GetOriginSettings()
			=> _cadModel.GetOriginSettings();

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out ISet<IIdeaConnectionPoint> connectionPoints)
		{
			CadUserSelection selection = _cadModel.GetUserSelection();
			_lastSelection = selection;
			nodes = new HashSet<IIdeaNode>();
			/*nodes = selection.Nodes
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();*/

			members = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

			connectionPoints = selection.ConnectionPoints
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out IIdeaConnectionPoint connectionPoint)
		{
			CadUserSelection selection = _cadModel.GetUserSelection();
			_lastSelection = selection;

			nodes = new HashSet<IIdeaNode>();
			/*nodes = selection.Nodes
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();*/

			members = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

			connectionPoint = selection.ConnectionPoints
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull().First();
		}
	}
}
