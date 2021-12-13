using RAMDATAACCESSLib;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	/// <inheritdoc cref="IIdeaMember1D"/>
	internal class RamMember : IIdeaMember1D
	{
		//TODO
		//private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

		public Member1DType Type => GetMemberType();

		public List<IIdeaElement1D> Elements => _elementFactory.GetElements(_memberNo);

		public string Id => "member-" + Name;

		public string Name => No.ToString();

		public int No => GetData().No;

		public IIdeaPersistenceToken Token => new PersistenceToken(TokenObjectType.Member, No);

		private readonly IObjectFactory _objectFactory;
		private readonly IResultsFactory _resultsFactory;
		private readonly IModelDataProvider _modelDataProvider;
		private readonly IElementFactory _elementFactory;
		private readonly int _memberNo;

		public RamMember(IObjectFactory objectFactory,
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

			//TODO
			//_logger.LogDebug($"Created {nameof(RamMember)} with id {Id}");
		}

		public IEnumerable<IIdeaResult> GetResults()
		{
			return _resultsFactory.GetResultsForMember(GetData());
		}

		private Member1DType GetMemberType()
		{
			switch (GetData().Type)
			{
				case IMember.MemberType.Column:
					return Member1DType.Column;

				//case MemberType.RibType:
				//	return Member1DType.Rib;

				case IMember.MemberType.HorizontalBrace:
				case IMember.MemberType.VerticalBrace:
					return Member1DType.Truss;

				default:
					return Member1DType.Beam;
			}
		}

		private IMember GetData() => _modelDataProvider.GetMember(_memberNo);
	}
}
