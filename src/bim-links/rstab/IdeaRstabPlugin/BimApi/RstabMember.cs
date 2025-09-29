using Dlubal.RSTAB8;
using IdeaRS.OpenModel.Model;
using IdeaRstabPlugin.Factories;
using IdeaRstabPlugin.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System.Collections.Generic;

namespace IdeaRstabPlugin.BimApi
{
	/// <inheritdoc cref="IIdeaMember1D"/>
	internal class RstabMember : IIdeaMember1D
	{
		private static readonly IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

		public Member1DType Type => GetMemberType();

		public List<IIdeaElement1D> Elements => _elementFactory.GetElements(_memberNo);

		public string Id => "member-" + Name;

		public string Name => No.ToString();

		public int No => GetData().No;

		public IIdeaPersistenceToken Token => new PersistenceToken(TokenObjectType.Member, No);

		public IIdeaTaper Taper => null;

		public IIdeaCrossSection CrossSection => null;

		public Alignment Alignment => Alignment.Center;

		public bool MirrorY => false;

		public bool MirrorZ => false;

		public IdeaVector3D EccentricityBegin { get; } = new IdeaVector3D(0, 0, 0);
		public IdeaVector3D EccentricityEnd { get; } = new IdeaVector3D(0, 0, 0);
		public InsertionPoints InsertionPoint { get; }
		public EccentricityReference EccentricityReference { get; }

		private readonly IObjectFactory _objectFactory;
		private readonly IResultsFactory _resultsFactory;
		private readonly IModelDataProvider _modelDataProvider;
		private readonly IElementFactory _elementFactory;
		private readonly int _memberNo;

		public RstabMember(IObjectFactory objectFactory,
			IModelDataProvider modelDataProvider,
			IResultsFactory resultsFactory,
			IElementFactory elementFactory,
			int memberNo)
		{
			_objectFactory = objectFactory;
			_modelDataProvider = modelDataProvider;
			_resultsFactory = resultsFactory;
			_elementFactory = elementFactory;
			_memberNo = memberNo;

			_logger.LogDebug($"Created {nameof(RstabMember)} with id {Id}");
		}

		public IEnumerable<IIdeaResult> GetResults()
		{
			return _resultsFactory.GetResultsForMember(GetData());
		}

		private Member1DType GetMemberType()
		{
			switch (GetData().Type)
			{
				case MemberType.Buckling:
				case MemberType.Compression:
					return Member1DType.Column;

				//case MemberType.RibType:
				//	return Member1DType.Rib;

				case MemberType.Truss:
					return Member1DType.Truss;

				default:
					return Member1DType.Beam;
			}
		}

		private Member GetData() => _modelDataProvider.GetMember(_memberNo);
	}
}