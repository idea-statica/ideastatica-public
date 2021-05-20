using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
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
		[Test]
		public void Import_IfReferenceElementArgumentIsNull_ThrowsNullArgumentException()
		{
			ResultImporter resultImporter = new ResultImporter(new NullLogger());
			Assert.That(() => resultImporter.Import(null, Substitute.For<IIdeaObjectWithResults>()),
				Throws.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void Import_IfObjArgumentIsNull_ThrowsNullArgumentException()
		{
			ResultImporter resultImporter = new ResultImporter(new NullLogger());
			Assert.That(() => resultImporter.Import(new ReferenceElement(), null),
				Throws.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void Import_IfObjArgumentIsNotMemberOrElement_ThrowsArgumentException()
		{
			ResultImporter resultImporter = new ResultImporter(new NullLogger());
			Assert.That(() => resultImporter.Import(new ReferenceElement(), Substitute.For<IIdeaObjectWithResults>()),
				Throws.InstanceOf<ArgumentException>());
		}

		[Test]
		public void Import_IfGetResultsReturnsNull_ReturnsEmptyEnumerable()
		{
			// Setup
			IIdeaMember1D member = Substitute.For<IIdeaMember1D>();
			member.GetResults().Returns((IEnumerable<IIdeaResult>)null);

			ResultImporter resultImporter = new ResultImporter(new NullLogger());

			// Tested method
			IEnumerable<ResultOnMember> result = resultImporter.Import(new ReferenceElement(), member);

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

			result.Type.Returns(ResultType.InternalForces);
			result.CoordinateSystemType.Returns(ResultLocalSystemType.Principle);

			IIdeaResultSection section = Substitute.For<IIdeaResultSection>();
			List<IIdeaResultSection> sections = new List<IIdeaResultSection>() { section };
			result.Sections.Returns(sections);

			section.AbsoluteOrRelative.Returns(true);
			section.Position.Returns(0.5);

			ResultOfInternalForces internalForces = new ResultOfInternalForces
			{
				Mx = 1,
				My = 2,
				Mz = 3,
				N = 4,
				Qy = 5,
				Qz = 6
			};
			List<SectionResultBase> sectionResults = new List<SectionResultBase>() { internalForces };
			section.Results.Returns(sectionResults);

			ResultImporter resultImporter = new ResultImporter(new NullLogger());

			ReferenceElement referenceElement = new ReferenceElement()
			{
				Id = 1
			};

			// Tested method
			List<ResultOnMember> resultsOnMember = resultImporter.Import(referenceElement, member).ToList();

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
		}
	}
}