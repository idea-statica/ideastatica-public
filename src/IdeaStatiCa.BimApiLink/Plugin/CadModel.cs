﻿using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	internal class CadModelAdapter : BimLinkObject, IIdeaModel
	{
		private CadUserSelection _lastSelection;

		private readonly ICadModel _cadModel;
		private readonly IProgressMessaging _remoteApp;
		private readonly string _applicationName;
		private IComparer<IIdentifier> _itemsComparer;

		public CadModelAdapter(ICadModel cadModel, IProgressMessaging remoteApp, string applicationName, IComparer<IIdentifier> itemsComparer)
		{			
			_cadModel = cadModel;
			_remoteApp = remoteApp;
			_applicationName = applicationName;
			_itemsComparer = itemsComparer;
		}

		public ISet<IIdeaLoading> GetLoads()
		{
			return new HashSet<IIdeaLoading>();
		}

		public ISet<IIdeaMember1D> GetMembers()
		{
			return _cadModel.GetAllMembers()
				.Select(x => GetMaybe(x))
				.Where(x => x != null)
				.ToHashSet();
		}

		public OriginSettings GetOriginSettings()
			=> _cadModel.GetOriginSettings();

		public BulkSelection GetBulkSelection()
		{
			_remoteApp?.SetStageLocalised(1, 0, LocalisedMessage.AwaitingUserSelection, _applicationName);
			IEnumerable<CadUserSelection> selections = _cadModel.GetUserSelections();
			return ProcessSelection(selections);
		}

		public BulkSelection GetWholeModel()
		{
			IEnumerable<CadUserSelection> selections = _cadModel.GetSelectionOfWholeModel();
			return ProcessSelection(selections);
		}

		private BulkSelection ProcessSelection(IEnumerable<CadUserSelection> selections)
		{
			var nodes = new HashSet<IIdeaNode>();
			var members = new HashSet<IIdeaMember1D>();
			var connectionPoints = new HashSet<IIdeaConnectionPoint>();

			if (selections == null)
			{
				return new BulkSelection(nodes, members, connectionPoints);
			}

			foreach (var selection in selections)
			{
				_lastSelection = selection;
				var connectionPoint = GetMaybe(selection.ConnectionPoint);

				nodes.Add(connectionPoint.Node);


				var connectedMembers = selection.Members
				.Select(x => GetMaybe(x))
				.Where(x => x != null)
				.ToHashSet();

				var connectionMembers = connectedMembers
				.Select(x => x.IdeaMember)
				.Where(x => x != null)
				.ToHashSet();

				members.UnionWith(connectionMembers);

				ProcessConnectionMembers(nodes, connectedMembers, connectionPoint);
				ProcessConnectionObjects(connectionPoint, selection);

				connectionPoints.Add(connectionPoint);
			}

			return new BulkSelection(nodes, members, connectionPoints);
		}

		public SingleSelection GetSingleSelection()
		{
			_remoteApp?.SetStageLocalised(1, 0, LocalisedMessage.AwaitingUserSelection, _applicationName);
			CadUserSelection selection = _cadModel.GetUserSelection();
			_lastSelection = selection;
			var nodes = new HashSet<IIdeaNode>();

			if (selection == null)
			{
				return new SingleSelection(nodes, new HashSet<IIdeaMember1D>());
			}

			var connectionPoint = GetMaybe(selection.ConnectionPoint);

			nodes.Add(connectionPoint.Node);

			var connectedMembers = selection.Members
				.Select(x => GetMaybe(x))
				.Where(x => x != null)
				.ToHashSet();

			var members = connectedMembers
			.Select(x => x.IdeaMember)
			.Where(x => x != null)
			.ToHashSet();

			ProcessConnectionMembers(nodes, connectedMembers, connectionPoint);
			ProcessConnectionObjects(connectionPoint, selection);

			return new SingleSelection(nodes, members, connectionPoint);
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
			//make sure first is process plates, members and than bolts weld (part witch should connect is known). 
			ICollection<IIdentifier> sortedIdentifiers;
			if (_itemsComparer == null)
			{
				sortedIdentifiers = selection.Objects.ToList();
			}
			else
			{
				sortedIdentifiers = selection.Objects.OrderBy(i => i, _itemsComparer).ToList();
			}

			foreach (var item in sortedIdentifiers)
			{
				switch (item.ObjectType.Name)
				{
					case nameof(IIdeaPlate):
						{
							var plate = BimApiImporter.Get(item) as IIdeaPlate;
							if (plate != null)
							{
								(connectionPoint.Plates as List<IIdeaPlate>).Add(plate);
							}
							break;
						}
					case nameof(IIdeaFoldedPlate):
						{
							var foldedPlate = BimApiImporter.Get(item) as IIdeaFoldedPlate;
							if (foldedPlate != null)
							{
								(connectionPoint.FoldedPlates as List<IIdeaFoldedPlate>).Add(foldedPlate);
							}
							break;
						}
					case nameof(IIdeaCut):
						{
							var cut = BimApiImporter.Get(item) as IIdeaCut;
							if (cut != null)
							{
								(connectionPoint.Cuts as List<IIdeaCut>).Add(cut);
							}
							break;
						}

					case nameof(IIdeaWeld):
						{
							var weld = BimApiImporter.Get(item) as IIdeaWeld;
							if (weld != null)
							{
								(connectionPoint.Welds as List<IIdeaWeld>).Add(weld);
							}
							break;
						}
					case nameof(IIdeaBoltGrid):
						{
							var boltGrid = BimApiImporter.Get(item) as IIdeaBoltGrid;
							if (boltGrid != null)
							{
								(connectionPoint.BoltGrids as List<IIdeaBoltGrid>).Add(boltGrid);
							}
							break;
						}
					case nameof(IIdeaAnchorGrid):
						{
							var anchorGrid = BimApiImporter.Get(item) as IIdeaAnchorGrid;
							if (anchorGrid != null)
							{
								(connectionPoint.AnchorGrids as List<IIdeaAnchorGrid>).Add(anchorGrid);
							}
							break;
						}
					case nameof(IIdeaConnectedMember):
						{
							var stiffeningMember = BimApiImporter.Get(item) as IIdeaConnectedMember;
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
