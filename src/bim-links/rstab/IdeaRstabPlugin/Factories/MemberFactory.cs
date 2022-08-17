using Dlubal.RSTAB8;
using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;

namespace IdeaRstabPlugin.Factories
{
	internal class MemberFactory : IFactory<int, IIdeaMember1D>
	{
		private static readonly IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.factories.memberfactory");

		private readonly IResultsFactory _resultsFactory;
		private readonly IModelDataProvider _modelDataProvider;
		private readonly IElementFactory _elementFactory;
		private readonly IResultsProvider _resultsProvider;

		public MemberFactory(IResultsFactory resultsFactory,
					   IModelDataProvider modelDataProvider,
					   IElementFactory elementFactory,
					   IResultsProvider resultsProvider)
		{
			_resultsFactory = resultsFactory;
			_modelDataProvider = modelDataProvider;
			_elementFactory = elementFactory;
			_resultsProvider = resultsProvider;
		}

		public IIdeaMember1D Create(IObjectFactory objectFactory, IImportSession importSession, int memberNo)
		{
			Member member = _modelDataProvider.GetMember(memberNo);

			if (!CanImport(member))
			{
				return null;
			}

			_resultsProvider.Prefetch(member.No);
			return new RstabMember(objectFactory,
						  _modelDataProvider,
						  _resultsFactory,
						  _elementFactory,
						  memberNo);
		}

		private static bool CanImport(Member member)
		{
			if (member.StartCrossSectionNo == 0)
			{
				_logger.LogDebug($"Member has no start cross-section, skipping.");
				return false;
			}

			return CanImportType(member.Type);
		}

		private static bool CanImportType(MemberType type)
		{
			switch (type)
			{
				case MemberType.UnknownMemberType:
				case MemberType.Rigid:
				case MemberType.CouplingRigidRigid:
				case MemberType.CouplingRigidHinge:
				case MemberType.CouplingHingeHinge:
				case MemberType.CouplingHingeRigid:
				case MemberType.NullMember:
					_logger.LogDebug($"Member is of unsupported type '{type}', skipping.");
					return false;
			}

			return true;
		}
	}
}