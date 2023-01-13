using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.BimApi;
using IdeaStatica.BimApiLink.Identifiers;
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
			nodes = selection.ConnectionPoints
				.Select(x => _bimApiImporter.Get(x).Node)
				.WhereNotNull()
				.ToHashSet();

			members = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

			/*connectionPoints = selection.ConnectionPoints
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();*/

			connectionPoints = (new List<IIdeaConnectionPoint>() { new IdeaConnectionPoint("cp1") }).ToHashSet();
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out IIdeaConnectionPoint connectionPoint)
		{
			CadUserSelection selection = _cadModel.GetUserSelection();
			_lastSelection = selection;

			connectionPoint = selection.ConnectionPoints
				.Select(x =>
				_bimApiImporter.Get(x))
				.WhereNotNull().First();

			nodes = new HashSet<IIdeaNode>();

			nodes.Add(connectionPoint.Node);


			members = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

			foreach (var ideaMember in members)
			{
				nodes.Add(ideaMember.Elements.First().Segment.StartNode);
				nodes.Add(ideaMember.Elements.Last().Segment.EndNode);
				var no = (ideaMember.Token as StringIdentifier<IIdeaMember1D>).Id;
				var id = new StringIdentifier<IIdeaConnectedMember>(no);
				var cm = _bimApiImporter.Get(id);
				(connectionPoint.ConnectedMembers as List<IIdeaConnectedMember>).Add(cm);
			};

			foreach (var item in selection.Objects)
			{
				switch (item.ObjectType.Name)
				{
					case nameof(IIdeaPlate):
						{
							var plate = _bimApiImporter.Get(item) as IIdeaPlate;
							(connectionPoint.Plates as List<IIdeaPlate>).Add(plate);
							break;
						}
				}
			}

		}
	}
}
