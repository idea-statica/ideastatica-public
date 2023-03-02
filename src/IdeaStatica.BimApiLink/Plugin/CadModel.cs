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
			ProcessSelection(out nodes, out members, out connectionPoints, selections);
		}

		public void GetWholeModel(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out ISet<IIdeaConnectionPoint> connectionPoints)
		{
			IEnumerable<CadUserSelection> selections = _cadModel.GetSelectionOfWholeModel();
			ProcessSelection(out nodes, out members, out connectionPoints, selections);
		}

		private void ProcessSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out ISet<IIdeaConnectionPoint> connectionPoints, IEnumerable<CadUserSelection> selections)
		{
			nodes = new HashSet<IIdeaNode>();
			members = new HashSet<IIdeaMember1D>();
			connectionPoints = new HashSet<IIdeaConnectionPoint>();
			if (selections == null)
			{
				return;
			}

			foreach (var selection in selections)
			{
				_lastSelection = selection;
				var connectionPoint = _bimApiImporter.Get(selection.ConnectionPoint);

				nodes.Add(connectionPoint.Node);


				var connectedMembers = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();
				var connectionMembers = connectedMembers
				.Select(x => x.IdeaMember)
				.WhereNotNull()
				.ToHashSet();

				members.UnionWith(connectionMembers);

				ProcessConnectionMembers(nodes, connectedMembers, connectionPoint);
				ProcessConnectionObjects(connectionPoint, selection);

				connectionPoints.Add(connectionPoint);
			}
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out IIdeaConnectionPoint connectionPoint)
		{
			CadUserSelection selection = _cadModel.GetUserSelection();
			_lastSelection = selection;
			nodes = new HashSet<IIdeaNode>();

			if (selection == null)
			{
				members = new HashSet<IIdeaMember1D>();
				connectionPoint = null;
				return;
			}
			connectionPoint = _bimApiImporter.Get(selection.ConnectionPoint);


			nodes.Add(connectionPoint.Node);

			var connectedMembers = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

			members = connectedMembers
			.Select(x => x.IdeaMember)
			.WhereNotNull()
			.ToHashSet();

			ProcessConnectionMembers(nodes, connectedMembers, connectionPoint);
			ProcessConnectionObjects(connectionPoint, selection);

		}

		private void ProcessConnectionMembers(ISet<IIdeaNode> nodes, ISet<IIdeaConnectedMember> members, IIdeaConnectionPoint connectionPoint)
		{
			foreach (var ideaMember in members)
			{
				nodes.Add(ideaMember.IdeaMember.Elements.First().Segment.StartNode);
				nodes.Add(ideaMember.IdeaMember.Elements.Last().Segment.EndNode);
				ideaMember.ConnectedMemberType = IdeaConnectedMemberType.Structural;
				(connectionPoint.ConnectedMembers as List<IIdeaConnectedMember>).Add(ideaMember);
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
							if (plate != null)
							{
								(connectionPoint.Plates as List<IIdeaPlate>).Add(plate);
							}
							break;
						}
					case nameof(IIdeaFoldedPlate):
						{
							var foldedPlate = _bimApiImporter.Get(item) as IIdeaFoldedPlate;
							if (foldedPlate != null)
							{
								(connectionPoint.FoldedPlates as List<IIdeaFoldedPlate>).Add(foldedPlate);
							}
							break;
						}
					case nameof(IIdeaCut):
						{
							var cut = _bimApiImporter.Get(item) as IIdeaCut;
							if (cut != null)
							{
								(connectionPoint.Cuts as List<IIdeaCut>).Add(cut);
							}
							break;
						}

					case nameof(IIdeaWeld):
						{
							var weld = _bimApiImporter.Get(item) as IIdeaWeld;
							if (weld != null)
							{
								(connectionPoint.Welds as List<IIdeaWeld>).Add(weld);
							}
							break;
						}
					case nameof(IIdeaBoltGrid):
						{
							var boltGrid = _bimApiImporter.Get(item) as IIdeaBoltGrid;
							if (boltGrid != null)
							{
								(connectionPoint.BoltGrids as List<IIdeaBoltGrid>).Add(boltGrid);
							}
							break;
						}
					case nameof(IIdeaAnchorGrid):
						{
							var anchorGrid = _bimApiImporter.Get(item) as IIdeaAnchorGrid;
							if (anchorGrid != null)
							{
								(connectionPoint.AnchorGrids as List<IIdeaAnchorGrid>).Add(anchorGrid);
							}
							break;
						}
					case nameof(IIdeaConnectedMember):
						{
							var stiffeningMember = _bimApiImporter.Get(item) as IIdeaConnectedMember;
							if (stiffeningMember != null)
							{
								stiffeningMember.ConnectedMemberType = IdeaConnectedMemberType.Stiffening;
								(connectionPoint.ConnectedMembers as List<IIdeaConnectedMember>).Add(stiffeningMember);
							}
							break;
						}
				}
			}
		}


	}
}
