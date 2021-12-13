using RAMDATAACCESSLib;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Geometry;
using IdeaStatiCa.RamToIdea.Providers;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	/// <inheritdoc cref="IIdeaModel"/>
	internal class RamModel : IIdeaModel
	{
		//App Objects
		private readonly IModel _model;
		private readonly INodes _modelNodes;
		//private readonly IModelData _modelData;
		
		//Idea Objects
		private readonly ILinesAndNodes _linesAndNodes;
		private readonly IObjectFactory _objectFactory;
		private readonly ILoadsProvider _loadsProvider;
		private readonly IImportSession _importSession;

		/// <summary>
		/// Creates instance of RSTABModel with given RSTAB model.
		/// </summary>
		/// <param name="model">RSTAB model</param>
		public RamModel(IModel model, ILinesAndNodes linesAndNodes, IObjectFactory objectFactory, ILoadsProvider loadsProvider,
			IImportSession importSession)
		{
			_model = model;
			_modelNodes = _model.GetFrameAnalysisNodes();
			_linesAndNodes = linesAndNodes;
			_objectFactory = objectFactory;
			//_modelData = model.GetModelData();
			_loadsProvider = loadsProvider;
			_importSession = importSession;
		}

		/// <summary>
		/// Returns set of all member in the project.
		/// </summary>
		/// <returns>Set of members</returns>
		public ISet<IIdeaMember1D> GetMembers()
		{
			//using (new LicenceLock(_model))
			//{
				return GetAllMembers()
				   .Select(x => _objectFactory.GetMember(x.IUID))
				   .ToHashSet();
			//}
		}

		/// <summary>
		/// Invokes selection of members in RSTAB and returns set of selected members.
		/// </summary>
		/// <param name="nodes">Always empty</param>
		/// <param name="members">Set of selected members</param>
		//public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members)
		//{
		//	nodes = new HashSet<IIdeaNode>();
		//	members = new HashSet<IIdeaMember1D>();

		//	using (new LicenceLock(_model))
		//	{
		//		List<int> selectedMembers = new List<int>();
		//		List<int> selectedNodalSupports = new List<int>();

		//		_modelData.EnableSelections(true);
		//		try
		//		{
		//			selectedMembers = _modelData.GetMembers().Select(x => x.No).ToList();
		//			selectedNodalSupports = _modelData.GetNodalSupports()
		//				.SelectMany(x => Utils.ParseObjectList(x.NodeList))
		//				.ToList();
		//		}
		//		finally
		//		{
		//			_modelData.EnableSelections(false);
		//		}

		//		if (selectedMembers.Count == 0)
		//		{
		//			string selectedMembersList = null;
		//			_model.GetActiveView().PickObjects(ToolType.SelectMembers, ref selectedMembersList);

		//			if (!string.IsNullOrEmpty(selectedMembersList))
		//			{
		//				selectedMembers = Utils.ParseObjectList(selectedMembersList).ToList();
		//			}
		//		}

		//		if (_importSession.RequestedItemsType != RequestedItemsType.Substructure &&
		//			selectedMembers.Count == 1 && selectedNodalSupports.Count == 0)
		//		{
		//			string selectedNodesList = null;
		//			_model.GetActiveView().PickObjects(ToolType.SelectNodes, ref selectedNodesList);

		//			if (!string.IsNullOrEmpty(selectedNodesList))
		//			{
		//				selectedNodalSupports.AddRange(Utils.ParseObjectList(selectedNodesList));
		//			}
		//		}

		//		if (_importSession.RequestedItemsType == RequestedItemsType.Substructure)
		//		{
		//			_linesAndNodes.UpdateAll();
		//		}
		//		else
		//		{
		//			_linesAndNodes.UpdateMembers(selectedMembers);
		//		}

		//		members = selectedMembers
		//			.Select(x => _objectFactory.GetMember(x))
		//			.ToHashSet();
		//		nodes = selectedNodalSupports
		//			.Select(x => _objectFactory.GetNode(x))
		//			.ToHashSet();
		//	}
		//}

		//public void SelectObject(IEnumerable<IIdeaObject> objects)
		//{
		//	string objectList = string.Join(",", objects
		//		.Select(x => x as RamMember)
		//		.Select(x => x?.No ?? -1)
		//		.Where(x => x != -1));

		//	_model.SelectObjects(ModelObjectType.MemberObject, objectList);
		//}

		//public void DeselectObjects()
		//{
		//	_modelData.SelectObjects(ModelObjectType.MemberObject, null);
		//}

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
				//TODO
				//ProjectName = _model.GetName(),
				//ProjectDescription = _model.GetDescription(),
				///CountryCode = CountryCode.ECEN // RSTAB api won't give me the country code/standard
			};
		}

		public IEnumerable<IMember> GetAllMembers()
		{
			//TODO Improve to allow for story input selection

			List<IMember> members = new List<IMember>();

			IStories stories = _model.GetStories();
			int numberStories = stories.GetCount();

			for (int i = 0; i < numberStories; i++)
			{
				IStory story = stories.GetAt(i);
				members.AddRange(getAllStoryMembers(story));
			}
			return members;
		}

		private IEnumerable<IMember> getAllStoryMembers(IStory story)
		{
			List<IMember> storyMembers = new List<IMember>();

			IColumns columns = story.GetColumns();
			int colCount = columns.GetCount();
			for (int i = 0; i < colCount; i++)
			{
				IColumn column = columns.GetAt(i);
				storyMembers.Add(new IMember(column, _modelNodes));
			}

			IBeams beams = story.GetBeams();
			int beamCount = columns.GetCount();
			for (int i = 0; i < beamCount; i++)
			{
				IBeam beam = beams.GetAt(i);
				storyMembers.Add(new IMember(beam, _modelNodes));
			}

			IVerticalBraces vertBraces = story.GetVerticalBraces();
			int vertCount = vertBraces.GetCount();
			for (int i = 0; i < vertCount; i++)
			{
				IVerticalBrace vertBrace = vertBraces.GetAt(i);
				storyMembers.Add(new IMember(vertBrace, _modelNodes));
			}

			IHorizBraces horizBraces = story.GetHorizBraces();
			int horzCount = horizBraces.GetCount();
			for (int i = 0; i < horzCount; i++)
			{
				IHorizBrace horzBrace = horizBraces.GetAt(i);
				storyMembers.Add(new IMember(horzBrace, _modelNodes));
			}

			return storyMembers;
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members)
		{
			throw new System.NotImplementedException();
		}
	}
}
