using AutoFixture.NUnit3;
using FluentAssertions;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin;
using IdeaStatiCa.TeklaStructuresPlugin.Importers;
using NSubstitute;
using NUnit.Framework;


namespace IdeaStatiCa.TeklaStructuresTest
{
	public class NodeImporterTest
	{
		[Test, AutoData]
		public void ShloudImportTeklaNode(double x, double y, double z)
		{

			IPluginLogger pluginLogger = Substitute.For<IPluginLogger>();
			ModelClient modelClient = new ModelClient(null, pluginLogger);

			Tekla.Structures.Geometry3d.Point point = new Tekla.Structures.Geometry3d.Point(x, y, z);


			NodeImporter importer = new NodeImporter(modelClient, pluginLogger);
			var result = importer.Create(modelClient.GetPointId(point));
			result.Should().NotBeNull();
			result.Should().BeAssignableTo<IIdeaNode>();
			(result).Vector.X.Should().Be(x.MilimetersToMeters());
			(result).Vector.Y.Should().Be(y.MilimetersToMeters());
			(result).Vector.Z.Should().Be(z.MilimetersToMeters());
		}
	}
}