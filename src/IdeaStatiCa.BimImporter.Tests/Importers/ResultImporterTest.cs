using FluentAssertions;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Results;
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
		private int _nextId;

		private IIdeaLoadCase CreateLoadCase(out int id)
		{
			id = _nextId++;

			IIdeaLoadCase loadCase = Substitute.For<IIdeaLoadCase>();
			loadCase.Id.Returns($"loadcase-{id}");

			ReferenceElement loadCaseRef = new ReferenceElement()
			{
				Id = id
			};
			ctx.Import(loadCase).Returns(loadCaseRef);

			return loadCase;
		}

		private List<IIdeaSectionResult> CreateOneSectionResult(IIdeaLoadCase loadCase)
		{
			IIdeaSectionResult sectionResult = Substitute.For<IIdeaSectionResult>();
			List<IIdeaSectionResult> sectionResults = new List<IIdeaSectionResult>() { sectionResult };
			sectionResult.Loading.Returns(loadCase);

			IIdeaResultData resultData = new InternalForcesData();
			sectionResult.Data.Returns(resultData);

			return sectionResults;
		}

		[SetUp]
		public void SetUp()
		{
			ctx = Substitute.For<IImportContext>();

			BimImporterConfiguration configuration = new BimImporterConfiguration();
			ctx.Configuration.Returns(configuration);

			resultImporter = new ResultImporter(new NullLogger());

			_nextId = 1;
		}

		[Test]
		public void Import_IfCtxArgumentIsNull_ThrowsNullArgumentException()
		{
			ResultsData resultsData = new ResultsData(
				Substitute.For<IIdeaObjectWithResults>(),
				MemberType.Member1D,
				new List<IIdeaResult>());

			Assert.That(() => resultImporter.Import(null, new ReferenceElement(), resultsData),
				Throws.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void Import_IfReferenceElementArgumentIsNull_ThrowsNullArgumentException()
		{
			ResultsData resultsData = new ResultsData(
				Substitute.For<IIdeaObjectWithResults>(),
				MemberType.Member1D,
				new List<IIdeaResult>());

			Assert.That(() => resultImporter.Import(ctx, null, resultsData),
				Throws.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void Import_IfObjArgumentIsNull_ThrowsNullArgumentException() => Assert.That(() => resultImporter.Import(ctx, new ReferenceElement(), null),
				Throws.InstanceOf<ArgumentNullException>());

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

			ResultsData resultsData = new ResultsData(member, MemberType.Member1D, results);

			// Tested method
			List<ResultOnMember> resultsOnMember = resultImporter.Import(ctx, referenceElement, resultsData).ToList();

			// Assert
			Assert.That(resultsOnMember.Count, Is.EqualTo(1));

			Assert.That(resultsOnMember[0].ResultType, Is.EqualTo(ResultType.InternalForces));
			Assert.That(resultsOnMember[0].LocalSystemType, Is.EqualTo(ResultLocalSystemType.Principle));
			Assert.That(resultsOnMember[0].Results.Count, Is.EqualTo(1));

			ResultOnSection resultOnSection = (ResultOnSection)resultsOnMember[0].Results[0];
			Assert.That(resultOnSection.AbsoluteRelative, Is.EqualTo(AbsoluteRelative.Relative));
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

			ResultsData resultsData = new ResultsData(member, MemberType.Member1D, results);

			// Tested method
			Assert.That(() => resultImporter.Import(ctx, referenceElement, resultsData).ToList(), Throws.InstanceOf<ConstraintException>());
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

			ResultsData resultsData = new ResultsData(member, MemberType.Member1D, results);

			// Tested method
			Assert.That(() => resultImporter.Import(ctx, referenceElement, resultsData).ToList(), Throws.InstanceOf<ConstraintException>());
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

			ResultsData resultsData = new ResultsData(member, MemberType.Member1D, results);

			// Tested method
			Assert.That(() => resultImporter.Import(ctx, referenceElement, resultsData).ToList(), Throws.InstanceOf<ConstraintException>());
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

			ResultsData resultsData = new ResultsData(member, MemberType.Member1D, results);

			// Tested method
			Assert.That(() => resultImporter.Import(ctx, referenceElement, resultsData).ToList(), Throws.InstanceOf<ConstraintException>());
		}

		[Test]
		public void Import_SectionPositionNormalizationOfZeroAndOne()
		{
			// Setup
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();

			IIdeaResult result = Substitute.For<IIdeaResult>();
			List<IIdeaResult> results = new List<IIdeaResult>() { result };
			member.GetResults().Returns(results);

			result.CoordinateSystemType.Returns(ResultLocalSystemType.Principle);

			IIdeaSection section1 = Substitute.For<IIdeaSection>();
			section1.Position.Returns(0.0 - 1e-7);

			IIdeaSection section2 = Substitute.For<IIdeaSection>();
			section2.Position.Returns(1.0 + 1e-7);

			List<IIdeaSection> sections = new List<IIdeaSection>() { section1, section2 };
			result.Sections.Returns(sections);

			IIdeaLoadCase loadCase = Substitute.For<IIdeaLoadCase>();
			ReferenceElement loadCaseRef = new ReferenceElement() { Id = 2 };
			ctx.Import(loadCase).Returns(loadCaseRef);

			IIdeaSectionResult sectionResult = Substitute.For<IIdeaSectionResult>();
			List<IIdeaSectionResult> sectionResults = new List<IIdeaSectionResult>() { sectionResult };
			sectionResult.Loading.Returns(loadCase);

			section1.Results.Returns(sectionResults);
			section2.Results.Returns(sectionResults);

			IIdeaResultData resultData = new InternalForcesData();
			sectionResult.Data.Returns(resultData);

			ReferenceElement referenceElement = new ReferenceElement()
			{
				Id = 1
			};

			ResultsData resultsData = new ResultsData(member, MemberType.Member1D, results);

			// Tested method
			ResultOnMember resultOnMember = resultImporter.Import(ctx, referenceElement, resultsData).ToList()[0];

			// Assert
			List<ResultOnSection> resultOnSections = resultOnMember.Results.Cast<ResultOnSection>().ToList();
			Assert.That(resultOnSections[0].Position, Is.EqualTo(0.0).Within(double.Epsilon));
			Assert.That(resultOnSections[1].Position, Is.EqualTo(1.0).Within(double.Epsilon));
		}

		[Test]
		public void Import_ShouldOrderSectionPositionFrom0To1()
		{
			// Setup
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();

			IIdeaResult result = Substitute.For<IIdeaResult>();
			List<IIdeaResult> results = new List<IIdeaResult>() { result };
			member.GetResults().Returns(results);

			result.CoordinateSystemType.Returns(ResultLocalSystemType.Principle);

			// Create sections in a random order
			IIdeaSection section1 = Substitute.For<IIdeaSection>();
			List<IIdeaSectionResult> sectionResults1 = CreateOneSectionResult(CreateLoadCase(out int lcId1));
			section1.Position.Returns(0.3);
			section1.Results.Returns(sectionResults1);

			IIdeaSection section2 = Substitute.For<IIdeaSection>();
			List<IIdeaSectionResult> sectionResults2 = CreateOneSectionResult(CreateLoadCase(out int lcId2));
			section2.Position.Returns(1.0);
			section2.Results.Returns(sectionResults2);

			IIdeaSection section3 = Substitute.For<IIdeaSection>();
			List<IIdeaSectionResult> sectionResults3 = CreateOneSectionResult(CreateLoadCase(out int lcId3));
			section3.Position.Returns(0.7);
			section3.Results.Returns(sectionResults3);

			IIdeaSection section4 = Substitute.For<IIdeaSection>();
			List<IIdeaSectionResult> sectionResults4 = CreateOneSectionResult(CreateLoadCase(out int lcId4));
			section4.Position.Returns(0.0);
			section4.Results.Returns(sectionResults4);

			List<IIdeaSection> sections = new List<IIdeaSection>() { section1, section2, section3, section4 };
			result.Sections.Returns(sections);

			ReferenceElement referenceElement = new ReferenceElement()
			{
				Id = 1
			};

			ResultsData resultsData = new ResultsData(member, MemberType.Member1D, results);

			// Tested method
			ResultOnMember resultOnMember = resultImporter.Import(ctx, referenceElement, resultsData).ToList()[0];

			// Assert
			List<ResultOnSection> resultOnSections = resultOnMember.Results.Cast<ResultOnSection>().ToList();

			Assert.That(resultOnSections[0].Position, Is.EqualTo(0.0));
			Assert.That(resultOnSections[0].Results[0].Loading.Id, Is.EqualTo(lcId4));

			Assert.That(resultOnSections[1].Position, Is.EqualTo(0.3));
			Assert.That(resultOnSections[1].Results[0].Loading.Id, Is.EqualTo(lcId1));

			Assert.That(resultOnSections[2].Position, Is.EqualTo(0.7));
			Assert.That(resultOnSections[2].Results[0].Loading.Id, Is.EqualTo(lcId3));

			Assert.That(resultOnSections[3].Position, Is.EqualTo(1.0));
			Assert.That(resultOnSections[3].Results[0].Loading.Id, Is.EqualTo(lcId2));
		}

		[Test]
		public void Import_SectionPositionDoesNotNormalizeDifferenceEqualToPrecision()
		{
			// Setup
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();

			IIdeaResult result = Substitute.For<IIdeaResult>();
			List<IIdeaResult> results = new List<IIdeaResult>() { result };
			member.GetResults().Returns(results);

			result.CoordinateSystemType.Returns(ResultLocalSystemType.Principle);

			IIdeaSection section1 = Substitute.For<IIdeaSection>();
			section1.Position.Returns(0.0);

			IIdeaSection section2 = Substitute.For<IIdeaSection>();
			double epsilon = ctx.Configuration.ResultSectionPositionPrecision;
			section2.Position.Returns(epsilon);

			List<IIdeaSection> sections = new List<IIdeaSection>() { section1, section2 };
			result.Sections.Returns(sections);

			IIdeaLoadCase loadCase = Substitute.For<IIdeaLoadCase>();
			ReferenceElement loadCaseRef = new ReferenceElement() { Id = 2 };
			ctx.Import(loadCase).Returns(loadCaseRef);

			IIdeaSectionResult sectionResult = Substitute.For<IIdeaSectionResult>();
			List<IIdeaSectionResult> sectionResults = new List<IIdeaSectionResult>() { sectionResult };
			sectionResult.Loading.Returns(loadCase);

			section1.Results.Returns(sectionResults);
			section2.Results.Returns(sectionResults);

			IIdeaResultData resultData = new InternalForcesData();
			sectionResult.Data.Returns(resultData);

			ReferenceElement referenceElement = new ReferenceElement()
			{
				Id = 1
			};

			ResultsData resultsData = new ResultsData(member, MemberType.Member1D, results);

			// Tested method
			ResultOnMember resultOnMember = resultImporter.Import(ctx, referenceElement, resultsData).ToList()[0];

			// Assert
			List<ResultOnSection> resultOnSections = resultOnMember.Results.Cast<ResultOnSection>().ToList();
			Assert.That(resultOnSections[0].Position, Is.EqualTo(0.0).Within(double.Epsilon));
			Assert.That(resultOnSections[1].Position, Is.EqualTo(epsilon).Within(double.Epsilon));
		}
	}
}