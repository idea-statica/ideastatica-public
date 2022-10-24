using FluentAssertions;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimImporter.Results;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests.Results
{
	[TestFixture]
	public class ResultsDataTest
	{
		[Test]
		public void Ctor_ObjIsNull_ThrowArgumentNullException()
		{
			Assert.That(() => new ResultsData(
					null,
					MemberType.Element1D,
					new List<IIdeaResult>()),
				Throws.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void Ctor_ShouldAssignObjArgumentIntoObjectProperty()
		{
			var obj = Substitute.For<IIdeaObjectWithResults>();

			var resultData = new ResultsData(
				obj,
				MemberType.Element1D,
				null);

			resultData.Object.Should().Be(obj);
		}

		[Test]
		public void Ctor_WhenResultsIsNull_ShouldUseEmptyCollection()
		{
			var resultData = new ResultsData(
				Substitute.For<IIdeaObjectWithResults>(),
				MemberType.Element1D,
				null);

			resultData.Results.Should().BeEmpty();
		}
	}
}