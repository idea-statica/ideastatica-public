using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Tests.Helpers;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests.Importers
{
	[TestFixture]
	public class LoadCaseImporterTest
	{
		[Test]
		public void LoadCaseImport()
		{
			// Setup
			var ctxBuilder = new ImportContextBuilder();

			(var loadGroup, var refLoadGroup) = ctxBuilder.Add<IIdeaLoadGroup>();

			(var loadOnLine1, var refLoadOnLine1) = ctxBuilder.Add<IIdeaLoadOnLine>();
			(var loadOnLine2, var refLoadOnLine2) = ctxBuilder.Add<IIdeaLoadOnLine>();
			(var loadOnLine3, var refLoadOnLine3) = ctxBuilder.Add<IIdeaLoadOnLine>();

			(var pointLoadOnLine1, var refPointLoadOnLine1) = ctxBuilder.Add<IIdeaPointLoadOnLine>();
			(var pointLoadOnLine2, var refPointLoadOnLine2) = ctxBuilder.Add<IIdeaPointLoadOnLine>();

			var loadCase = Substitute.For<IIdeaLoadCase>();
			loadCase.Id.Returns("loadCase");
			loadCase.Name.Returns("loadCase");
			loadCase.Description.Returns("This is load case");
			loadCase.LoadType.Returns(LoadCaseType.Permanent);
			loadCase.Type.Returns(LoadCaseSubType.PermanentSelfweight);
			loadCase.Variable.Returns(VariableType.Seismicity);
			loadCase.LoadGroup.Returns(loadGroup);
			loadCase.LoadsOnLine.Returns(new List<IIdeaLoadOnLine>() { loadOnLine1, loadOnLine2, loadOnLine3 });
			loadCase.PointLoadsOnLine.Returns(new List<IIdeaPointLoadOnLine>() { pointLoadOnLine1, pointLoadOnLine2 });

			var loadCaseImporter = new LoadCaseImporter(new NullLogger());

			// Tested method
			var iomObject = loadCaseImporter.Import(ctxBuilder.Context, loadCase);

			// Asserts
			Assert.That(iomObject, Is.InstanceOf<LoadCase>());
			var iomLoadCase = (LoadCase)iomObject;

			Assert.That(iomLoadCase.Name, Is.EqualTo("loadCase"));
			Assert.That(iomLoadCase.Description, Is.EqualTo("This is load case"));
			Assert.That(iomLoadCase.LoadType, Is.EqualTo(LoadCaseType.Permanent));
			Assert.That(iomLoadCase.Type, Is.EqualTo(LoadCaseSubType.PermanentSelfweight));
			Assert.That(iomLoadCase.Variable, Is.EqualTo(VariableType.Seismicity));
			Assert.That(iomLoadCase.LoadGroup, Is.EqualTo(refLoadGroup));
			Assert.That(iomLoadCase.LoadsOnLine, Is.EquivalentTo(new[] { refLoadOnLine1, refLoadOnLine2, refLoadOnLine3 }));
			Assert.That(iomLoadCase.PointLoadsOnLine, Is.EquivalentTo(new[] { refPointLoadOnLine1, refPointLoadOnLine2}));
		}
	}
}
