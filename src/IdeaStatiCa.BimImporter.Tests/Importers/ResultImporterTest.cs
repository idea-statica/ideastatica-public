using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Tests.Importers
{
	[TestFixture]
	internal class ResultImporterTest
	{
		private ResultImporter resultImporter;
		private IImportContext ctx;

		[SetUp]
		public void SetUp()
		{
			ctx = Substitute.For<IImportContext>();
			resultImporter = new ResultImporter(new NullLogger());
		}

		[Test]
		public void Import_IfCtxArgumentIsNull_ThrowsNullArgumentException()
		{
			Assert.That(() => resultImporter.Import(null, new ReferenceElement(), Substitute.For<IIdeaObjectWithResults>()),
				Throws.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void Import_IfReferenceElementArgumentIsNull_ThrowsNullArgumentException()
		{
			Assert.That(() => resultImporter.Import(ctx, null, Substitute.For<IIdeaObjectWithResults>()),
				Throws.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void Import_IfObjArgumentIsNull_ThrowsNullArgumentException()
		{
			Assert.That(() => resultImporter.Import(ctx, new ReferenceElement(), null),
				Throws.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void Import_IfObjArgumentIsNotMemberOrElement_ThrowsConstraintException()
		{
			Assert.That(() => resultImporter.Import(ctx, new ReferenceElement(), Substitute.For<IIdeaObjectWithResults>()),
				Throws.InstanceOf<ConstraintException>());
		}

		[Test]
		public void Import_IfGetResultsReturnsNull_ReturnsEmptyEnumerable()
		{
			// Setup
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.GetResults().Returns((IEnumerable<IIdeaResult>)null);

			// Tested method
			IEnumerable<ResultOnMember> result = resultImporter.Import(ctx, new ReferenceElement(), member);

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void Import_InternalForces()
		{
			// Setup
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();

			IIdeaResult result = Substitute.For<IIdeaResult>();
			List<IIdeaResult> results = new List<IIdeaResult>() { result };
			member.GetResults().Returns(results);

			result.CoordinateSystemType.Returns(ResultLocalSystemType.Principle);

			IIdeaSection section = Substitute.For<IIdeaSection>();
			List<IIdeaSection> sections = new List<IIdeaSection>() { section };
			result.Sections.Returns(sections);

			section.Position.Returns(0.5);

			IIdeaLoadCase loadCase = Substitute.For<IIdeaLoadCase>();
			ReferenceElement loadCaseRef = new ReferenceElement() { Id = 2 };
			ctx.Import(loadCase).Returns(loadCaseRef);

			IIdeaSectionResult sectionResult = Substitute.For<IIdeaSectionResult>();
			List<IIdeaSectionResult> sectionResults = new List<IIdeaSectionResult>() { sectionResult };
			section.Results.Returns(sectionResults);
			sectionResult.Loading.Returns(loadCase);

			InternalForcesData internalForces = new InternalForcesData
			{
				Mx = 1,
				My = 2,
				Mz = 3,
				N = 4,
				Qy = 5,
				Qz = 6
			};
			sectionResult.Data.Returns(internalForces);

			ReferenceElement referenceElement = new ReferenceElement()
			{
				Id = 1
			};

			// Tested method
			List<ResultOnMember> resultsOnMember = resultImporter.Import(ctx, referenceElement, member).ToList();

			// Assert
			Assert.That(resultsOnMember.Count, Is.EqualTo(1));

			Assert.That(resultsOnMember[0].ResultType, Is.EqualTo(ResultType.InternalForces));
			Assert.That(resultsOnMember[0].LocalSystemType, Is.EqualTo(ResultLocalSystemType.Principle));
			Assert.That(resultsOnMember[0].Results.Count, Is.EqualTo(1));

			ResultOnSection resultOnSection = (ResultOnSection)resultsOnMember[0].Results[0];
			Assert.That(resultOnSection.AbsoluteRelative, Is.EqualTo(AbsoluteRelative.Absolute));
			Assert.That(resultOnSection.Position, Is.EqualTo(0.5));
			Assert.That(resultOnSection.Results.Count, Is.EqualTo(1));

			ResultOfInternalForces resultOfIF = (ResultOfInternalForces)resultOnSection.Results[0];
			Assert.That(resultOfIF.Mx, Is.EqualTo(1.0));
			Assert.That(resultOfIF.My, Is.EqualTo(2.0));
			Assert.That(resultOfIF.Mz, Is.EqualTo(3.0));
			Assert.That(resultOfIF.N, Is.EqualTo(4.0));
			Assert.That(resultOfIF.Qy, Is.EqualTo(5.0));
			Assert.That(resultOfIF.Qz, Is.EqualTo(6.0));

			ResultOfLoading resultOfLoading = resultOfIF.Loading;
			Assert.That(resultOfLoading.LoadingType, Is.EqualTo(LoadingType.LoadCase));
			Assert.That(resultOfLoading.Id, Is.EqualTo(2));
			Assert.That(resultOfLoading.Items.Count, Is.EqualTo(1));
			Assert.That(resultOfLoading.Items[0].Coefficient, Is.EqualTo(1.0));
		}

		[Test]
		public void Import_IfSectionPositionIsGreaterThanOne_ThrowsConstraintException()
		{
			// Setup: result with position 2
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();

			IIdeaResult result = Substitute.For<IIdeaResult>();
			List<IIdeaResult> results = new List<IIdeaResult>() { result };
			member.GetResults().Returns(results);

			result.CoordinateSystemType.Returns(ResultLocalSystemType.Principle);

			IIdeaSection section = Substitute.For<IIdeaSection>();
			List<IIdeaSection> sections = new List<IIdeaSection>() { section };
			result.Sections.Returns(sections);

			section.Position.Returns(2);

			ReferenceElement referenceElement = new ReferenceElement()
			{
				Id = 1
			};

			// Tested method
			Assert.That(() => resultImporter.Import(ctx, referenceElement, member).ToList(), Throws.InstanceOf<ConstraintException>());
		}

		[Test]
		public void Import_IfSectionPositionIsLessThanZero_ThrowsConstraintException()
		{
			// Setup: result with position 2
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();

			IIdeaResult result = Substitute.For<IIdeaResult>();
			List<IIdeaResult> results = new List<IIdeaResult>() { result };
			member.GetResults().Returns(results);

			result.CoordinateSystemType.Returns(ResultLocalSystemType.Principle);

			IIdeaSection section = Substitute.For<IIdeaSection>();
			List<IIdeaSection> sections = new List<IIdeaSection>() { section };
			result.Sections.Returns(sections);

			section.Position.Returns(-0.2);

			ReferenceElement referenceElement = new ReferenceElement()
			{
				Id = 1
			};

			// Tested method
			Assert.That(() => resultImporter.Import(ctx, referenceElement, member).ToList(), Throws.InstanceOf<ConstraintException>());
		}

		[Test]
		public void Import_IfTwoSectionHaveTheSamePosition_ThrowsConstraintException()
		{
			// Setup
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();

			IIdeaResult result = Substitute.For<IIdeaResult>();
			List<IIdeaResult> results = new List<IIdeaResult>() { result };
			member.GetResults().Returns(results);

			result.CoordinateSystemType.Returns(ResultLocalSystemType.Principle);

			IIdeaSection section = Substitute.For<IIdeaSection>();
			List<IIdeaSection> sections = new List<IIdeaSection>() { section, section };
			result.Sections.Returns(sections);

			section.Position.Returns(0.5);

			IIdeaLoadCase loadCase = Substitute.For<IIdeaLoadCase>();
			ReferenceElement loadCaseRef = new ReferenceElement() { Id = 2 };
			ctx.Import(loadCase).Returns(loadCaseRef);

			IIdeaSectionResult sectionResult = Substitute.For<IIdeaSectionResult>();
			List<IIdeaSectionResult> sectionResults = new List<IIdeaSectionResult>() { sectionResult };
			section.Results.Returns(sectionResults);
			sectionResult.Loading.Returns(loadCase);

			InternalForcesData internalForces = new InternalForcesData
			{
				Mx = 1,
				My = 2,
				Mz = 3,
				N = 4,
				Qy = 5,
				Qz = 6
			};
			sectionResult.Data.Returns(internalForces);

			ReferenceElement referenceElement = new ReferenceElement()
			{
				Id = 1
			};

			// Tested method
			Assert.That(() => resultImporter.Import(ctx, referenceElement, member).ToList(), Throws.InstanceOf<ConstraintException>());
		}

		[Test]
		public void Import_IfResultDataIsNotInstanceOfIdeaInternalForces_ThrowConstrainException()
		{
			// Setup
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();

			IIdeaResult result = Substitute.For<IIdeaResult>();
			List<IIdeaResult> results = new List<IIdeaResult>() { result };
			member.GetResults().Returns(results);

			result.CoordinateSystemType.Returns(ResultLocalSystemType.Principle);

			IIdeaSection section = Substitute.For<IIdeaSection>();
			List<IIdeaSection> sections = new List<IIdeaSection>() { section };
			result.Sections.Returns(sections);

			section.Position.Returns(0.5);

			IIdeaLoadCase loadCase = Substitute.For<IIdeaLoadCase>();
			ReferenceElement loadCaseRef = new ReferenceElement() { Id = 2 };
			ctx.Import(loadCase).Returns(loadCaseRef);

			IIdeaSectionResult sectionResult = Substitute.For<IIdeaSectionResult>();
			List<IIdeaSectionResult> sectionResults = new List<IIdeaSectionResult>() { sectionResult };
			section.Results.Returns(sectionResults);
			sectionResult.Loading.Returns(loadCase);

			IIdeaResultData resultData = Substitute.For<IIdeaResultData>();
			sectionResult.Data.Returns(resultData);

			ReferenceElement referenceElement = new ReferenceElement()
			{
				Id = 1
			};

			// Tested method
			Assert.That(() => resultImporter.Import(ctx, referenceElement, member).ToList(), Throws.InstanceOf<ConstraintException>());
		}
	}
}