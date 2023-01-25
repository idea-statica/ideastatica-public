using IdeaRS.OpenModel;
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
			IEnumerable<CadUserSelection> selections = _cadModel.GetUserSelections();

			nodes = new HashSet<IIdeaNode>();
			members = new HashSet<IIdeaMember1D>();
			connectionPoints = new HashSet<IIdeaConnectionPoint>();
			foreach (var selection in selections)
			{
				_lastSelection = selection;
				var connectionPoint = _bimApiImporter.Get(selection.ConnectionPoint);

				nodes.Add(connectionPoint.Node);

				var connectionMembers = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

				members.UnionWith(connectionMembers);

				ProcessConnectionMembers(nodes, members, connectionPoint);
				ProcessConnectionObjects(connectionPoint, selection);

				connectionPoints.Add(connectionPoint);
			}
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out IIdeaConnectionPoint connectionPoint)
		{
			CadUserSelection selection = _cadModel.GetUserSelection();
			_lastSelection = selection;

			connectionPoint = _bimApiImporter.Get(selection.ConnectionPoint);

			nodes = new HashSet<IIdeaNode>();

			nodes.Add(connectionPoint.Node);


			members = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();
			ProcessConnectionMembers(nodes, members, connectionPoint);
			ProcessConnectionObjects(connectionPoint, selection);

		}

		private void ProcessConnectionMembers(ISet<IIdeaNode> nodes, ISet<IIdeaMember1D> members, IIdeaConnectionPoint connectionPoint)
		{
			foreach (var ideaMember in members)
			{
				nodes.Add(ideaMember.Elements.First().Segment.StartNode);
				nodes.Add(ideaMember.Elements.Last().Segment.EndNode);
				var no = (ideaMember.Token as StringIdentifier<IIdeaMember1D>).Id;
				var id = new StringIdentifier<IIdeaConnectedMember>(no);
				var cm = _bimApiImporter.Get(id);
				(connectionPoint.ConnectedMembers as List<IIdeaConnectedMember>).Add(cm);
			};
		}

		private void ProcessConnectionObjects(IIdeaConnectionPoint connectionPoint, CadUserSelection selection)
		{
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
					case nameof(IIdeaCut):
						{
							var cut = _bimApiImporter.Get(item) as IIdeaCut;
							(connectionPoint.Cuts as List<IIdeaCut>).Add(cut);
							break;
						}

					case nameof(IIdeaWeld):
						{
							var weld = _bimApiImporter.Get(item) as IIdeaWeld;
							(connectionPoint.Welds as List<IIdeaWeld>).Add(weld);
							break;
						}
					case nameof(IIdeaBoltGrid):
						{
							var boltGrid = _bimApiImporter.Get(item) as IIdeaBoltGrid;
							(connectionPoint.BoltGrids as List<IIdeaBoltGrid>).Add(boltGrid);
							break;
						}
					case nameof(IIdeaAnchorGrid):
						{
							var anchorGrid = _bimApiImporter.Get(item) as IIdeaAnchorGrid;
							(connectionPoint.AnchorGrids as List<IIdeaAnchorGrid>).Add(anchorGrid);
							break;
						}
					case nameof(IIdeaConnectedMember):
						{
							var stiffeningMember = _bimApiImporter.Get(item) as IIdeaConnectedMember;
							stiffeningMember.ConnectedMemberType = IdeaConnectedMemberType.Stiffening;
							(connectionPoint.ConnectedMembers as List<IIdeaConnectedMember>).Add(stiffeningMember);
							break;
						}
				}
			}
		}
	}
}
