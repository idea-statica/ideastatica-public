using Dlubal.RSTAB8;
using IdeaRS.OpenModel;
using IdeaRstabPlugin.Factories;
using IdeaRstabPlugin.Geometry;
using IdeaRstabPlugin.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaRstabPlugin.BimApi
{
	/// <inheritdoc cref="IIdeaModel"/>
	internal class RstabModel : IIdeaModel
	{
		private readonly IModel _model;
		private readonly IModelData _modelData;
		private readonly ILinesAndNodes _linesAndNodes;
		private readonly IObjectFactory _objectFactory;
		private readonly ILoadsProvider _loadsProvider;
		private readonly IImportSession _importSession;

		/// <summary>
		/// Creates instance of RSTABModel with given RSTAB model.
		/// </summary>
		/// <param name="model">RSTAB model</param>
		public RstabModel(IModel model, ILinesAndNodes linesAndNodes, IObjectFactory objectFactory, ILoadsProvider loadsProvider,
			IImportSession importSession)
		{
			_model = model;
			_linesAndNodes = linesAndNodes;
			_objectFactory = objectFactory;
			_modelData = model.GetModelData();
			_loadsProvider = loadsProvider;
			_importSession = importSession;
		}

		/// <summary>
		/// Returns set of all member in the project.
		/// </summary>
		/// <returns>Set of members</returns>
		public ISet<IIdeaMember1D> GetMembers()
		{
			using (new LicenceLock(_model))
			{
				return _modelData.GetMembers()
					.Select(x => _objectFactory.GetMember(x.No))
					.Where(x => x != null)
					.ToHashSet();
			}
		}

		/// <summary>
		/// Invokes selection of members in RSTAB and returns set of selected members.
		/// </summary>
		/// <param name="nodes">Always empty</param>
		/// <param name="members">Set of selected members</param>
		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out ISet<IIdeaConnectionPoint> connectionPoints)
		{
			nodes = new HashSet<IIdeaNode>();
			members = new HashSet<IIdeaMember1D>();
			connectionPoints = null;
			using (new LicenceLock(_model))
			{
				List<int> selectedMembers = new List<int>();
				List<int> selectedNodalSupports = new List<int>();

				_modelData.EnableSelections(true);
				try
				{
					selectedMembers = _modelData.GetMembers().Select(x => x.No).ToList();
					selectedNodalSupports = _modelData.GetNodalSupports()
						.SelectMany(x => Utils.ParseObjectList(x.NodeList))
						.ToList();
				}
				finally
				{
					_modelData.EnableSelections(false);
				}

				if (selectedMembers.Count == 0)
				{
					string selectedMembersList = null;
					_model.GetActiveView().PickObjects(ToolType.SelectMembers, ref selectedMembersList);

					if (!string.IsNullOrEmpty(selectedMembersList))
					{
						selectedMembers = Utils.ParseObjectList(selectedMembersList).ToList();
					}
				}

				if (_importSession.RequestedItemsType != RequestedItemsType.Substructure &&
					selectedMembers.Count == 1 && selectedNodalSupports.Count == 0)
				{
					string selectedNodesList = null;
					_model.GetActiveView().PickObjects(ToolType.SelectNodes, ref selectedNodesList);

					if (!string.IsNullOrEmpty(selectedNodesList))
					{
						selectedNodalSupports.AddRange(Utils.ParseObjectList(selectedNodesList));
					}
				}

				if (_importSession.RequestedItemsType == RequestedItemsType.Substructure)
				{
					_linesAndNodes.UpdateAll();
				}
				else
				{
					_linesAndNodes.UpdateMembers(selectedMembers);
				}

				members = selectedMembers
					.Select(x => _objectFactory.GetMember(x))
					.Where(x => x != null)
					.ToHashSet();
				nodes = selectedNodalSupports
					.Select(x => _objectFactory.GetNode(x))
					.ToHashSet();
			}
		}

		public void SelectObject(IEnumerable<IIdeaObject> objects)
		{
			string objectList = string.Join(",", objects
				.Select(x => x as RstabMember)
				.Select(x => x?.No ?? -1)
				.Where(x => x != -1));

			_modelData.SelectObjects(ModelObjectType.MemberObject, objectList);
		}

		public void DeselectObjects()
		{
			_modelData.SelectObjects(ModelObjectType.MemberObject, null);
		}

		public ISet<IIdeaLoading> GetLoads()
		{
			return _loadsProvider.GetResultCombinations()
				.Select(x => _objectFactory.GetResultCombiInput(x.Loading.No))
				.Cast<IIdeaLoading>()
				.ToHashSet();
		}

		public OriginSettings GetOriginSettings()
		{
			return new OriginSettings()
			{
				ProjectName = _model.GetName(),
				ProjectDescription = _model.GetDescription(),
				///CountryCode = CountryCode.ECEN // RSTAB api won't give me the country code/standard
			};
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out IIdeaConnectionPoint connectionPoints)
		{
			throw new System.NotImplementedException();
		}
	}
}