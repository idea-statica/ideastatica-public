using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.ImportedObjects;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Tests.Helpers;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Tests
{
    [TestFixture]
    public class BimImporterTest
    {
        [Test]
        public void ImportConnections_OnlyMembersSelected()
        {
            // Setup: model with one connection and two members
            GeometryBuilder builder = new GeometryBuilder();
            builder
                .Member(1, "line(0,1)")
                .Member(2, "line(1,2)");

            IIdeaModel model = builder.GetModel();
            model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
                .Do(x =>
                {
                    x[0] = new HashSet<IIdeaNode>();
                    x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2] };
                });

            ConnectionPoint iomConnectionPoint = new ConnectionPoint();

            IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
            importer.Import(Arg.Any<IImportContext>(), Arg.Any<Connection>()).Returns(iomConnectionPoint);

            BimImporter bimImporter = new BimImporter(model, new Project(), importer, new Geometry());

            // Tested methods
            ModelBIM modelBIM = bimImporter.ImportConnections();

            // Asserts

            Assert.That(modelBIM.Items.Count, Is.EqualTo(1));
            Assert.That(modelBIM.Items[0].Id, Is.EqualTo(1));
            Assert.That(modelBIM.Items[0].Type, Is.EqualTo(BIMItemType.Node));
        }

        [Test]
        public void ImportConnections_NodeSelected()
        {
            // Setup: model with one connection and two members
            GeometryBuilder builder = new GeometryBuilder();
            builder
                .Member(1, "line(0,1)")
                .Member(2, "line(1,2)");

            IIdeaModel model = builder.GetModel();
            model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
                .Do(x =>
                {
                    x[0] = new HashSet<IIdeaNode>() { builder.Nodes[1] };
                    x[1] = new HashSet<IIdeaMember1D>();
                });

            ConnectionPoint iomConnectionPoint = new ConnectionPoint();

            IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
            importer.Import(Arg.Any<IImportContext>(), Arg.Any<Connection>()).Returns(iomConnectionPoint);

            BimImporter bimImporter = new BimImporter(model, new Project(), importer, new Geometry());

            // Tested methods
            ModelBIM modelBIM = bimImporter.ImportConnections();

            // Asserts

            Assert.That(modelBIM.Items.Count, Is.EqualTo(1));
            Assert.That(modelBIM.Items[0].Id, Is.EqualTo(1));
            Assert.That(modelBIM.Items[0].Type, Is.EqualTo(BIMItemType.Node));
        }

        [Test]
        public void ImportConnections_OnlyMembersSelected2()
        {
            // Setup: model with one connection and two members
            GeometryBuilder builder = new GeometryBuilder();
            builder
                .Member(1, "line(0,1)")
                .Member(2, "line(1,2)");

            builder.Nodes[1].Name.Returns("connection node");

            IIdeaModel model = builder.GetModel();
            model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
                .Do(x =>
                {
                    x[0] = new HashSet<IIdeaNode>();
                    x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2] };
                });

            ConnectionPoint iomConnectionPoint = new ConnectionPoint();

            IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
            importer.Import(Arg.Any<IImportContext>(), Arg.Any<Connection>()).Returns(iomConnectionPoint);

            BimImporter bimImporter = new BimImporter(model, new Project(), importer, new Geometry());

            // Tested methods
            ModelBIM modelBIM = bimImporter.ImportConnections();

            // Asserts
            Assert.That(modelBIM.Model.ConnectionPoint, Contains.Item(iomConnectionPoint));

            importer.Received().Import(Arg.Any<IImportContext>(), Arg.Is<Connection>(x =>
                x.Id != null &&
                x.Node == builder.Nodes[1] &&
                x.Name == "connection node" &&
                Enumerable.SequenceEqual(x.Members, new List<IIdeaMember1D>() { builder.Members[1], builder.Members[2] })
            ));
        }

        [Test]
        public void ImportConnections_TwoMembersSelected()
        {
            // Setup: model with one connection and two members
            GeometryBuilder builder = new GeometryBuilder();
            builder
                .Member(1, "line(0,1)")
                .Member(2, "line(1,2)")
                .Member(3, "line(2,3)");

            builder.Nodes[1].Name.Returns("connection node");

            IIdeaModel model = builder.GetModel();
            model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
                .Do(x =>
                {
                    x[0] = new HashSet<IIdeaNode>();
                    x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2] };
                });

            ConnectionPoint iomConnectionPoint = new ConnectionPoint();

            IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
            importer.Import(Arg.Any<IImportContext>(), Arg.Any<Connection>()).Returns(iomConnectionPoint);

            BimImporter bimImporter = new BimImporter(model, new Project(), importer, new Geometry());

            // Tested methods
            ModelBIM modelBIM = bimImporter.ImportConnections();

            // Asserts
            Assert.That(modelBIM.Model.ConnectionPoint, Contains.Item(iomConnectionPoint));

            importer.Received().Import(Arg.Any<IImportContext>(), Arg.Is<Connection>(x =>
                x.Id != null &&
                x.Node == builder.Nodes[1] &&
                x.Name == "connection node" &&
                Enumerable.SequenceEqual(x.Members, new List<IIdeaMember1D>() { builder.Members[1], builder.Members[2] })
            ));
        }

        [Test]
        public void ImportConnections_TwoMembersSelected2()
        {
            // Setup: model with one connection and two members
            GeometryBuilder builder = new GeometryBuilder();
            builder
                .Member(1, "line(0,1)")
                .Member(2, "line(1,2)")
                .Member(3, "line(1,3)");

            builder.Nodes[1].Name.Returns("connection node");

            IIdeaModel model = builder.GetModel();
            model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
                .Do(x =>
                {
                    x[0] = new HashSet<IIdeaNode>();
                    x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2] };
                });

            ConnectionPoint iomConnectionPoint = new ConnectionPoint();

            IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
            importer.Import(Arg.Any<IImportContext>(), Arg.Any<Connection>()).Returns(iomConnectionPoint);

            BimImporter bimImporter = new BimImporter(model, new Project(), importer, new Geometry());

            // Tested methods
            ModelBIM modelBIM = bimImporter.ImportConnections();

            // Asserts
            Assert.That(modelBIM.Model.ConnectionPoint, Contains.Item(iomConnectionPoint));

            importer.Received().Import(Arg.Any<IImportContext>(), Arg.Is<Connection>(x =>
                x.Id != null &&
                x.Node == builder.Nodes[1] &&
                x.Name == "connection node" &&
                Enumerable.SequenceEqual(x.Members, new List<IIdeaMember1D>() { builder.Members[1], builder.Members[2] })
            ));
        }

        [Test]
        public void ImportConnections_TwoConnections()
        {
            // Setup: model with one connection and two members
            GeometryBuilder builder = new GeometryBuilder();
            builder
                .Member(1, "line(0,1)")
                .Member(2, "line(1,2)")
                .Member(3, "line(3,4)")
                .Member(4, "line(4,5)");

            builder.Nodes[1].Name.Returns("connection node1");
            builder.Nodes[4].Name.Returns("connection node2");

            IIdeaModel model = builder.GetModel();
            model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
                .Do(x =>
                {
                    x[0] = new HashSet<IIdeaNode>();
                    x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2], builder.Members[3], builder.Members[4] };
                });

            IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
            importer.Import(Arg.Any<IImportContext>(), Arg.Any<Connection>()).Returns(_ => new ConnectionPoint());

            BimImporter bimImporter = new BimImporter(model, new Project(), importer, new Geometry());

            // Tested methods
            ModelBIM modelBIM = bimImporter.ImportConnections();

            // Asserts
            Assert.That(modelBIM.Model.ConnectionPoint.Count, Is.EqualTo(2));

            importer.Received().Import(Arg.Any<IImportContext>(), Arg.Is<Connection>(x =>
                x.Id != null &&
                x.Node == builder.Nodes[1] &&
                x.Name == builder.Nodes[1].Name &&
                Enumerable.SequenceEqual(x.Members, new List<IIdeaMember1D>() { builder.Members[1], builder.Members[2] })
            ));
            importer.Received().Import(Arg.Any<IImportContext>(), Arg.Is<Connection>(x =>
                 x.Id != null &&
                 x.Node == builder.Nodes[4] &&
                 x.Name == builder.Nodes[4].Name &&
                 Enumerable.SequenceEqual(x.Members, new List<IIdeaMember1D>() { builder.Members[3], builder.Members[4] })
             ));
        }

        [Test]
        public void ImportConnections_TwoIndependedConnections()
        {
            // Setup: model with one connection and two members
            GeometryBuilder builder = new GeometryBuilder();
            builder
                .Member(1, "line(0,1)")
                .Member(2, "line(1,2)")
                .Member(3, "line(2,3)");

            builder.Nodes[1].Name.Returns("connection node1");
            builder.Nodes[2].Name.Returns("connection node2");

            IIdeaModel model = builder.GetModel();
            model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
                .Do(x =>
                {
                    x[0] = new HashSet<IIdeaNode>();
                    x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2], builder.Members[3] };
                });

            IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
            importer.Import(Arg.Any<IImportContext>(), Arg.Any<Connection>()).Returns(_ => new ConnectionPoint());

            BimImporter bimImporter = new BimImporter(model, new Project(), importer, new Geometry());

            // Tested methods
            ModelBIM modelBIM = bimImporter.ImportConnections();

            // Asserts
            Assert.That(modelBIM.Model.ConnectionPoint.Count, Is.EqualTo(2));

            importer.Received().Import(Arg.Any<IImportContext>(), Arg.Is<Connection>(x =>
                x.Id != null &&
                x.Node == builder.Nodes[1] &&
                x.Name == builder.Nodes[1].Name &&
                Enumerable.SequenceEqual(x.Members, new List<IIdeaMember1D>() { builder.Members[1], builder.Members[2] })
            ));
            importer.Received().Import(Arg.Any<IImportContext>(), Arg.Is<Connection>(x =>
                 x.Id != null &&
                 x.Node == builder.Nodes[2] &&
                 x.Name == builder.Nodes[2].Name &&
                 Enumerable.SequenceEqual(x.Members, new List<IIdeaMember1D>() { builder.Members[2], builder.Members[3] })
             ));
        }



        [Test]
        public void ImportConnections_TwoIndependedConnectionsnodeslected()
        {
            // Setup: model with one connection and two members
            GeometryBuilder builder = new GeometryBuilder();
            builder
                .Member(1, "line(0,1)")
                .Member(2, "line(1,2)")
                .Member(3, "line(2,3)");

            builder.Nodes[1].Name.Returns("connection node1");
            builder.Nodes[2].Name.Returns("connection node2");

            IIdeaModel model = builder.GetModel();
            model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
                .Do(x =>
                {
                    x[0] = new HashSet<IIdeaNode>() { builder.Nodes[1], builder.Nodes[2] };
                    x[1] = new HashSet<IIdeaMember1D>();
                });

            IImporter<IIdeaObject> importer = Substitute.For<IImporter<IIdeaObject>>();
            importer.Import(Arg.Any<IImportContext>(), Arg.Any<Connection>()).Returns(_ => new ConnectionPoint());

            BimImporter bimImporter = new BimImporter(model, new Project(), importer, new Geometry());

            // Tested methods
            ModelBIM modelBIM = bimImporter.ImportConnections();

            // Asserts
            Assert.That(modelBIM.Model.ConnectionPoint.Count, Is.EqualTo(2));

            importer.Received().Import(Arg.Any<IImportContext>(), Arg.Is<Connection>(x =>
                x.Id != null &&
                x.Node == builder.Nodes[1] &&
                x.Name == builder.Nodes[1].Name &&
                Enumerable.SequenceEqual(x.Members, new List<IIdeaMember1D>() { builder.Members[1], builder.Members[2] })
            ));
            importer.Received().Import(Arg.Any<IImportContext>(), Arg.Is<Connection>(x =>
                 x.Id != null &&
                 x.Node == builder.Nodes[2] &&
                 x.Name == builder.Nodes[2].Name &&
                 Enumerable.SequenceEqual(x.Members, new List<IIdeaMember1D>() { builder.Members[2], builder.Members[3] })
             ));
        }
    }
}