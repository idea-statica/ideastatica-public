using AutoFixture.NUnit3;
using FluentAssertions;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin;
using IdeaStatiCa.TeklaStructuresPlugin.Importers;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresTest
{
	public class WorkPlaneImporterTest
	{
		[Test, AutoData]
		public void ShloudImportTeklaWorkPlaneFitting()
		{

			IPluginLogger pluginLogger = Substitute.For<IPluginLogger>();
			IModelClient modelClient = Substitute.For<IModelClient>();


			Plane plane = new Plane();
			plane.AxisX = new Tekla.Structures.Geometry3d.Vector(1, 0, 0);
			plane.AxisY = new Tekla.Structures.Geometry3d.Vector(0, 1, 0);
			Fitting fitting = new Fitting()
			{
				Plane = plane,
			};

			modelClient.Configure().GetItemByHandler(Arg.Any<string>()).Returns(a =>
			{
				return fitting;
			});


			WorkPlaneImporter importer = new WorkPlaneImporter(modelClient, pluginLogger);
			var result = importer.Create(fitting.Identifier.GUID.ToString());
			result.Should().NotBeNull();
			result.Should().BeAssignableTo<IIdeaWorkPlane>();
			(result).Normal.X.Should().Be(0);
			(result).Normal.Y.Should().Be(0);
			(result).Normal.Z.Should().Be(1);
		}

		[Test, AutoData]
		public void ShloudImportTeklaWorkPlaneCutPlane()
		{

			IPluginLogger pluginLogger = Substitute.For<IPluginLogger>();
			IModelClient modelClient = Substitute.For<IModelClient>();


			Plane plane = new Plane();
			plane.AxisX = new Tekla.Structures.Geometry3d.Vector(1, 0, 0);
			plane.AxisY = new Tekla.Structures.Geometry3d.Vector(0, 1, 0);
			CutPlane cutPlane = new CutPlane()
			{
				Plane = plane,
			};

			modelClient.Configure().GetItemByHandler(Arg.Any<string>()).Returns(a =>
			{
				return cutPlane;
			});


			WorkPlaneImporter importer = new WorkPlaneImporter(modelClient, pluginLogger);
			var result = importer.Create(cutPlane.Identifier.GUID.ToString());
			result.Should().NotBeNull();
			result.Should().BeAssignableTo<IIdeaWorkPlane>();
			(result).Normal.X.Should().Be(0);
			(result).Normal.Y.Should().Be(0);
			(result).Normal.Z.Should().Be(1);
		}
	}
}