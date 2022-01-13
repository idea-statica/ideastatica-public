using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Geometry;
using IdeaStatiCa.RamToIdea.Providers;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class ObjectFactory : IObjectFactory
	{
		private readonly IModel _model;
		private readonly ISectionFactory _sectionFactory;
		private readonly IGeometry _geometry;
		private readonly ISegmentFactory _segmentFactory;
		private readonly ILoadsProvider _loadsProvider;

		private readonly Dictionary<int, IIdeaLoadCase> _loadCases = new Dictionary<int, IIdeaLoadCase>();
		private readonly Dictionary<ILoadCombination, IIdeaCombiInput> _combinations = new Dictionary<ILoadCombination, IIdeaCombiInput>();
		private readonly Dictionary<string, IIdeaLoadGroup> _loadGroups = new Dictionary<string, IIdeaLoadGroup>();
		private readonly IResultsFactory _resultsFactory;

		public ObjectFactory(IModel model, ISectionFactory sectionFactory, IResultsFactory resultsFactory, ILoadsProvider loadsProvider
			, IGeometry geometry, ISegmentFactory segmentFactory)
		{
			_model = model;
			_sectionFactory = sectionFactory;
			_loadsProvider = loadsProvider;
			_resultsFactory = resultsFactory;
			_geometry = geometry;
			_segmentFactory = segmentFactory;
		}

		public IIdeaMember1D GetBeam(IBeam beam)
		{
			return new RamMemberBeam(this, _sectionFactory, _resultsFactory, _geometry, _segmentFactory, beam);
		}

		public IIdeaMember1D GetColumn(IColumn column)
		{
			return new RamMemberColumn(this, _sectionFactory, _resultsFactory, _geometry, _segmentFactory, column);
		}

		public IIdeaMember1D GetHorizontalBrace(IHorizBrace horizBrace)
		{
			return new RamMemberHorizontalBrace(this, _sectionFactory, _resultsFactory, _geometry, _segmentFactory, horizBrace);
		}

		public IIdeaMember1D GetVerticalBrace(IVerticalBrace verticalBrace)
		{
			return new RamMemberVerticalBrace(this, _sectionFactory, _resultsFactory, _geometry, _segmentFactory, verticalBrace);
		}

		public IStory GetStory(int uid)
		{
			return _model.GetStory(uid);
		}

		public IIdeaCombiInput GetLoadCombiInput(ILoadCombination combination)
		{
			return GetOrCreate(_combinations, combination, () => new RamCombiInput(this, combination));
		}

		public IIdeaLoadCase GetLoadCase(int uid)
		{
			return GetOrCreate(_loadCases, uid, () => new RamLoadCase(this, _loadsProvider, uid));
		}

		public IIdeaLoadGroup GetLoadGroup(string loadGroupName, LoadGroupType type)
		{
			return GetOrCreate(_loadGroups, loadGroupName, () => new RamLoadGroup(loadGroupName, type));
		}

		private static TValue GetOrCreate<TKey, TValue>(Dictionary<TKey, TValue> store, TKey key, Func<TValue> creator)
		{
			if (!store.TryGetValue(key, out var value))
			{
				value = creator();
				store.Add(key, value);
			}

			return value;
		}
	}
}