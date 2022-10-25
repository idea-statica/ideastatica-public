using FluentAssertions;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimImporter.Results;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests.Results
{
	[TestFixture]
	public class DefaultResultsProviderTest
	{
		private DefaultResultsProvider _sut;

		[SetUp]
		public void SetUp()
		{
			_sut = new DefaultResultsProvider();
		}

		[Test]
		public void GetResults_SingleObjectAndSingleResult()
		{
			var member = Substitute.For<IIdeaMember1D>();
			var result = Substitute.For<IIdeaResult>();
			member.GetResults().Returns(new IIdeaResult[] { result });

			_sut.GetResults(new IIdeaObjectWithResults[] { member })
				.Should().AllBeEquivalentTo(new ResultsData(member, MemberType.Member1D, new IIdeaResult[] { result }));
		}

		[Test]
		public void GetResults_TwoObjectsAndSingleResult()
		{
			var member = Substitute.For<IIdeaMember1D>();
			var result1 = Substitute.For<IIdeaResult>();
			member.GetResults().Returns(new IIdeaResult[] { result1 });

			var element = Substitute.For<IIdeaElement1D>();
			var result2 = Substitute.For<IIdeaResult>();
			element.GetResults().Returns(new IIdeaResult[] { result2 });

			_sut.GetResults(new IIdeaObjectWithResults[] { member, element })
				.Should().BeEquivalentTo(new ResultsData[] 
				{
					new ResultsData(member, MemberType.Member1D, new IIdeaResult[] { result1 }),
					new ResultsData(element, MemberType.Element1D, new IIdeaResult[] { result2 }),
				});
		}

		[Test]
		public void GetResults_ObjectIsNotMemberNorElement()
		{
			var objectWithResult = Substitute.For<IIdeaObjectWithResults>();
			var result = Substitute.For<IIdeaResult>();
			objectWithResult.GetResults().Returns(new IIdeaResult[] { result });

			_sut.GetResults(new IIdeaObjectWithResults[] { objectWithResult }).Should().BeEmpty();
		}
	}
}