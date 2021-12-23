using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using RAMDATAACCESSLib;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	public class RamModel : IIdeaModel
	{
		private readonly IObjectFactory _objectFactory;
		private readonly HashSet<IIdeaMember1D> _members;
		private readonly IModel _model;

		internal RamModel(IObjectFactory objectFactory, IModel model)
		{
			_objectFactory = objectFactory;
			_model = model;
			_members = GetAllMembers().ToHashSet();
		}

		public ISet<IIdeaLoading> GetLoads()
		{
			return new HashSet<IIdeaLoading>();
		}

		public ISet<IIdeaMember1D> GetMembers()
		{
			return _members;
		}

		public OriginSettings GetOriginSettings()
		{
			return new OriginSettings()
			{
				CountryCode = CountryCode.ECEN,
				ProjectName = _model.strProjectName
			};
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members)
		{
			nodes = new HashSet<IIdeaNode>();
			members = _members.ToHashSet();
		}

		private IEnumerable<IIdeaMember1D> GetAllMembers()
		{
			//TODO Improve to allow for story input selection

			List<IIdeaMember1D> members = new List<IIdeaMember1D>();

			IStories stories = _model.GetStories();
			int numberStories = stories.GetCount();

			for (int i = 0; i < numberStories; i++)
			{
				members.AddRange(GetAllStoryMembers(stories.GetAt(i)));
			}

			return members;
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
	}
}