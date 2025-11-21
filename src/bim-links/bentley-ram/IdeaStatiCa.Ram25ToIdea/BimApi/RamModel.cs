using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Geometry;
using IdeaStatiCa.RamToIdea.Providers;
using RAMDATAACCESSLib;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamModel : IIdeaModel
	{
		private readonly IObjectFactory _objectFactory;
		private readonly IModel _model;
		private readonly IGeometry _geometry;
		private readonly ILoadsProvider _loadsProvider;

		private readonly HashSet<IIdeaMember1D> _members;

		private IdeaRS.OpenModel.CountryCode _countryCode;

		public RamModel(IObjectFactory objectFactory, IModel model, ILoadsProvider loadsProvider, IGeometry geometry, IdeaRS.OpenModel.CountryCode countryCode)
		{
			_objectFactory = objectFactory;
			_model = model;
			_loadsProvider = loadsProvider;
			_geometry = geometry;
			_countryCode = countryCode;

			_members = GetAllMembers().ToHashSet();
		}

		public ISet<IIdeaLoading> GetLoads()
		{
			return _loadsProvider.GetLoadCombinations()
				.Select(x => (IIdeaLoading)_objectFactory.GetLoadCombiInput(x))
				.ToHashSet();
		}

		public ISet<IIdeaMember1D> GetMembers()
		{
			return _members;
		}

		public OriginSettings GetOriginSettings()
		{
			return new OriginSettings()
			{
				CheckEquilibrium = true,
				CountryCode = _countryCode,
				ProjectName = _model.strProjectName
			};
		}

		public BulkSelection GetBulkSelection()
		{
			var nodes = new HashSet<IIdeaNode>();
			var members = _members.ToHashSet();

			return new BulkSelection(nodes, members);
		}

		private IEnumerable<IIdeaMember1D> GetAllMembers()
		{
			//TODO Improve to allow for story input selection

			_geometry.Initialize(GetNodes());
			return CreateMembers();
		}

		private List<IIdeaMember1D> CreateMembers()
		{
			List<IIdeaMember1D> members = new List<IIdeaMember1D>();

			IStories stories = _model.GetStories();
			int numberStories = stories.GetCount();

			for (int i = 0; i < numberStories; i++)
			{
				members.AddRange(GetAllStoryMembers(stories.GetAt(i)));
			}

			return members;
		}

		private IEnumerable<INode> GetNodes()
		{
			INodes nodes = _model.GetFrameAnalysisNodes();
			int count = nodes.GetCount();
			for (int i = 0; i < count; i++)
			{
				yield return nodes.GetAt(i);
			}
		}

		private IEnumerable<IIdeaMember1D> GetAllStoryMembers(IStory story)
		{
			IEnumerable<IIdeaMember1D> a = GetColumns(story).Select(x => _objectFactory.GetColumn(x));
			IEnumerable<IIdeaMember1D> b = GetBeams(story).Select(x => _objectFactory.GetBeam(x));
			IEnumerable<IIdeaMember1D> c = GetVerticalBraces(story).Select(x => _objectFactory.GetVerticalBrace(x));
			IEnumerable<IIdeaMember1D> d = GetHorizontalBraces(story).Select(x => _objectFactory.GetHorizontalBrace(x));

			return a.Concat(b).Concat(c).Concat(d);
		}

		private IEnumerable<IColumn> GetColumns(IStory story)
		{
			IColumns columns = story.GetColumns();
			int count = columns.GetCount();

			for (int i = 0; i < count; i++)
			{
				yield return columns.GetAt(i);
			}
		}

		private IEnumerable<IBeam> GetBeams(IStory story)
		{
			IBeams beams = story.GetBeams();
			int count = beams.GetCount();

			for (int i = 0; i < count; i++)
			{
				yield return beams.GetAt(i);
			}
		}

		private IEnumerable<IVerticalBrace> GetVerticalBraces(IStory story)
		{
			IVerticalBraces braces = story.GetVerticalBraces();
			int count = braces.GetCount();

			for (int i = 0; i < count; i++)
			{
				yield return braces.GetAt(i);
			}
		}

		private IEnumerable<IHorizBrace> GetHorizontalBraces(IStory story)
		{
			IHorizBraces braces = story.GetHorizBraces();
			int count = braces.GetCount();

			for (int i = 0; i < count; i++)
			{
				yield return braces.GetAt(i);
			}
		}

		public SingleSelection GetSingleSelection()
		{
			throw new System.NotImplementedException();
		}

		public BulkSelection GetWholeModel()
		{
			throw new System.NotImplementedException();
		}
	}
}