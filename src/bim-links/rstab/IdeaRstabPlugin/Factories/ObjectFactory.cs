using Dlubal.RSTAB8;
using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Geometry;
using IdeaRstabPlugin.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Factories
{
	/// <summary>
	/// Factory class mainly for creating BimApi objects from RSTAB objects.
	/// </summary>
	internal class ObjectFactory : IObjectFactory, IDataCache
	{
		private static readonly IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.factories");

		private static readonly IIdeaObjectComparer _comparer = new IIdeaObjectComparer();
		private static readonly IdeaMaterialEqualityComparer _materialEqualityComparer = new IdeaMaterialEqualityComparer();

		private readonly ObjectStorage<IIdeaMember1D> _members = new ObjectStorage<IIdeaMember1D>(_comparer);
		private readonly ObjectStorage<IIdeaNode> _nodes = new ObjectStorage<IIdeaNode>(_comparer);
		private readonly ObjectStorage<IIdeaCrossSection> _crossSections = new ObjectStorage<IIdeaCrossSection>(_comparer);
		private readonly ObjectStorage<IIdeaMaterial> _materials = new ObjectStorage<IIdeaMaterial>(_materialEqualityComparer);

		private readonly ObjectStorage<IIdeaLoadCase> _loadcases = new ObjectStorage<IIdeaLoadCase>(_comparer);
		private readonly ObjectStorage<IIdeaLoadCase> _loadcasesNl = new ObjectStorage<IIdeaLoadCase>(_comparer);
		private readonly ObjectStorage<IIdeaCombiInput> _combiinput = new ObjectStorage<IIdeaCombiInput>(_comparer);

		private readonly IModel _model;
		private readonly IModelData _modelData;
		private readonly ILoads _rfLoads;
		private readonly IFactory<ICrossSection, IIdeaCrossSection> _crossSectionFactory;
		private readonly IFactory<IMaterial, IIdeaMaterial> _materialFactory;

		private readonly IElementFactory _elementFactory;
		private readonly ResultsFactory _resultsFactory;
		private readonly IModelDataProvider _modelDataProvider;
		private readonly IImportSession _importSession;
		private readonly ILoadsProvider _loadsProvider;
		private readonly IResultsProvider _resultsProvider;
		private readonly MemberFactory _memberFactory;

		public IList<int> ids = new List<int>();
		public IList<IIdeaLoadCase> notExclusiveIds = new List<IIdeaLoadCase>();
		public IList<IIdeaLoadCase> permanentIds = new List<IIdeaLoadCase>();
		public IList<IIdeaLoadCase> variableIds = new List<IIdeaLoadCase>();
		public IList<IIdeaLoadCase> cyclicIds = new List<IIdeaLoadCase>();
		public IList<IIdeaLoadCase> differentRelationLC = new List<IIdeaLoadCase>();

		public ObjectStorage<IIdeaLoadGroup> LoadGroup { get; } = new ObjectStorage<IIdeaLoadGroup>(_comparer);

		/// <summary>
		/// ObjectFactory
		/// </summary>
		/// <param name="model"></param>
		public ObjectFactory(IModel model, IModelDataProvider modelDataProvider, ILinesAndNodes linesAndNodes,
			ILoadsProvider loadsProvider, IResultsProvider resultsProvider, IImportSession importSession)
		{
			_model = model;
			_importSession = importSession;

			_modelDataProvider = modelDataProvider;
			_loadsProvider = loadsProvider;
			_resultsProvider = resultsProvider;

			using (new LicenceLock(_model))
			{
				_modelData = model.GetModelData();
				_rfLoads = model.GetLoads();
			}

			_crossSectionFactory = new CrossSectionFactory();
			_materialFactory = new MaterialFactory();

			_resultsFactory = new ResultsFactory(this, resultsProvider, importSession);
			_elementFactory = new ElementFactory(modelDataProvider, linesAndNodes, this, _importSession);
			_memberFactory = new MemberFactory(_resultsFactory, modelDataProvider, _elementFactory, resultsProvider);
		}

		public void Clear()
		{
			_logger.LogDebug("Flushing all object caches.");

			_members.Clear();
			_nodes.Clear();
			_crossSections.Clear();
			_loadcases.Clear();
			LoadGroup.Clear();
			_combiinput.Clear();
		}

		public IIdeaMember1D GetMember(int memberNo)
		{
			_logger.LogTrace($"ObjectFactory.GetMember({memberNo})");

			return _members.GetOrCreate(memberNo,
				() => _memberFactory.Create(this, _importSession, memberNo));
		}

		public IIdeaCrossSection GetCrossSection(int cssNo)
		{
			_logger.LogTrace($"ObjectFactory.GetCrossSection({cssNo})");

			return _crossSections.GetOrCreate(cssNo,
				() => _crossSectionFactory.Create(this, _importSession, _modelData.GetCrossSection(cssNo, ItemAt.AtNo)));
		}

		public IIdeaNode GetNode(int nodeNo)
		{
			_logger.LogTrace($"ObjectFactory.GetNode({nodeNo})");

			return _nodes.GetOrCreate(nodeNo,
				() => new RstabNode(_importSession, _modelDataProvider, this, nodeNo));
		}

		public IIdeaLoadCase GetLoadCase(int loadCaseNo)
		{
			_logger.LogTrace($"ObjectFactory.GetLoadCase({loadCaseNo})");

			return _loadcases.GetOrCreate(loadCaseNo,
				() => new RstabLoadCase(this, _loadsProvider, loadCaseNo));
		}

		public IIdeaLoadGroup GetLoadGroup(int lgNo)
		{
			_logger.LogTrace($"ObjectFactory.GetLoadGroup({lgNo})");

			return LoadGroup.GetOrCreate(lgNo,
				() => new RstabLoadGroup(_model, this, lgNo));
		}

		public IIdeaCombiInput GetResultCombiInput(int no)
		{
			_logger.LogTrace($"ObjectFactory.GetCombiInput({no})");

			return _combiinput.GetOrCreate(no,
				() => new RstabCombiInput(_model, this, _rfLoads.GetResultCombination(no, ItemAt.AtNo)));
		}

		public IIdeaLoadCase GetLoadCaseNonLin(int lgNo)
		{
			_logger.LogTrace($"ObjectFactory.GetLoadCaseNonLin({lgNo})");

			return _loadcasesNl.GetOrCreate(lgNo,
				() => new RstabLoadCaseNonLinear(this, _loadsProvider, lgNo));
		}

		public IIdeaMaterial GetMaterial(int materialNo)
		{
			_logger.LogTrace($"ObjectFactory.GetMaterial({materialNo})");

			return _materials.GetOrCreate(materialNo,
				() => _materialFactory.Create(this, _importSession, _modelData.GetMaterial(materialNo, ItemAt.AtNo)));
		}

		public IIdeaLoading GetLoading(Loading loading)
		{
			switch (loading.Type)
			{
				case LoadingType.LoadCaseType:
					return GetLoadCase(loading.No);

				case LoadingType.LoadCombinationType:
					return GetLoadCaseNonLin(loading.No);

				case LoadingType.ResultCombinationType:
					return GetResultCombiInput(loading.No);

				default:
					throw new ArgumentException();
			}
		}
	}
}