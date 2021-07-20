﻿using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Persistence;
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
		private static readonly BímItemEqualityComparer _connectionEqualityComparer = new BímItemEqualityComparer();

		private IBimObjectImporter bimObjectImporter;

		private BimImporter CreateBimImporter(IIdeaModel model)
		{
			NullLogger logger = new NullLogger();

			IPersistence persistence = Substitute.For<IPersistence>();
			IObjectRestorer objectRestorer = Substitute.For<IObjectRestorer>();
			IProject project = new Project(logger, persistence, objectRestorer);
			IGeometryProvider geometryProvider = new DefaultGeometryProvider(logger, model);

			return new BimImporter(model, project, logger, geometryProvider, bimObjectImporter);
		}

		[SetUp]
		public void SetUp()
		{
			bimObjectImporter = Substitute.For<IBimObjectImporter>();
			bimObjectImporter.Import(Arg.Any<IEnumerable<IIdeaObject>>(), Arg.Any<IEnumerable<IBimItem>>(), Arg.Any<IProject>())
				.Returns(new ModelBIM()
				{
					Model = new IdeaRS.OpenModel.OpenModel(),
					Items = new List<BIMItemId>(),
					Results = new IdeaRS.OpenModel.Result.OpenModelResult()
				});
		}

		[Test]
		public void ImportConnections_TwoMembersBothSelected()
		{
			// Setup: model with one connection and two members, both members are selected
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

			IdeaRS.OpenModel.Connection.ConnectionPoint iomConnectionPoint = new IdeaRS.OpenModel.Connection.ConnectionPoint();

			BimImporter bimImporter = CreateBimImporter(model);

			// Tested methods
			bimImporter.ImportConnections();

			// Assert: expecting one connection in node 1 and members 1,2
			Connection expectedConnection = Connection.FromNodeAndMembers(builder.Nodes[1], new List<IIdeaMember1D>()
			{
				builder.Members[1], builder.Members[2]
			});

			bimObjectImporter.Received()
				.Import(
					Arg.Any<IEnumerable<IIdeaObject>>(),
					Arg.Is<IEnumerable<IBimItem>>(x =>
						Enumerable.SequenceEqual(x, new List<IBimItem>() { expectedConnection }, _connectionEqualityComparer)),
					Arg.Any<IProject>());
		}

		[Test]
		public void ImportConnections_ThreeMembersTwoSelected()
		{
			// Setup: model with two connections and three members, two members selected
			GeometryBuilder builder = new GeometryBuilder();
			builder
				.Member(1, "line(0,1)")
				.Member(2, "line(1,2)")
				.Member(3, "line(2,3)");

			IIdeaModel model = builder.GetModel();
			model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
				.Do(x =>
				{
					x[0] = new HashSet<IIdeaNode>();
					x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2] };
				});

			IdeaRS.OpenModel.Connection.ConnectionPoint iomConnectionPoint = new IdeaRS.OpenModel.Connection.ConnectionPoint();

			BimImporter bimImporter = CreateBimImporter(model);

			// Tested methods
			ModelBIM modelBIM = bimImporter.ImportConnections();

			// Assert: expecting one connection in node 1 and members 1,2
			Connection expectedConnection = Connection.FromNodeAndMembers(builder.Nodes[1], new List<IIdeaMember1D>()
			{
				builder.Members[1], builder.Members[2]
			});

			bimObjectImporter.Received()
				.Import(
					Arg.Any<IEnumerable<IIdeaObject>>(),
					Arg.Is<IEnumerable<IBimItem>>(x =>
						Enumerable.SequenceEqual(x, new List<IBimItem>() { expectedConnection }, _connectionEqualityComparer)),
					Arg.Any<IProject>());
		}

		[Test]
		public void ImportConnections_FourMembersAllSelected()
		{
			// Setup: model with 3 connections and 4 members, connected into one continuous line
			GeometryBuilder builder = new GeometryBuilder();
			builder
				.Member(1, "line(0,1)")
				.Member(2, "line(1,2)")
				.Member(3, "line(3,4)")
				.Member(4, "line(4,5)");

			IIdeaModel model = builder.GetModel();
			model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
				.Do(x =>
				{
					x[0] = new HashSet<IIdeaNode>();
					x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2], builder.Members[3], builder.Members[4] };
				});

			BimImporter bimImporter = CreateBimImporter(model);

			// Tested methods
			ModelBIM modelBIM = bimImporter.ImportConnections();

			// Assert: expecting 2 connections:
			//	1. node 1, members 1,2
			//	2. node 4, members 3,4
			Connection expectedConnection1 = Connection.FromNodeAndMembers(builder.Nodes[1], new List<IIdeaMember1D>()
			{
				builder.Members[1], builder.Members[2]
			});
			Connection expectedConnection2 = Connection.FromNodeAndMembers(builder.Nodes[4], new List<IIdeaMember1D>()
			{
				builder.Members[3], builder.Members[4]
			});

			bimObjectImporter.Received()
				.Import(
					Arg.Any<IEnumerable<IIdeaObject>>(),
					Arg.Is<IEnumerable<IBimItem>>(x =>
						Enumerable.SequenceEqual(x, new List<IBimItem>() { expectedConnection1, expectedConnection2 }, _connectionEqualityComparer)),
					Arg.Any<IProject>());
		}

		[Test]
		public void ImportConnections_ThreeMembersAllSelected()
		{
			// Setup: model with 2 connections and 3 members, all members selected
			GeometryBuilder builder = new GeometryBuilder();
			builder
				.Member(1, "line(0,1)")
				.Member(2, "line(1,2)")
				.Member(3, "line(2,3)");

			IIdeaModel model = builder.GetModel();
			model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
				.Do(x =>
				{
					x[0] = new HashSet<IIdeaNode>();
					x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2], builder.Members[3] };
				});

			BimImporter bimImporter = CreateBimImporter(model);

			// Tested methods
			ModelBIM modelBIM = bimImporter.ImportConnections();

			// Assert: expecting 2 connections:
			//	1. node 1, members 1,2
			//	2. node 2, members 2,3
			Connection expectedConnection1 = Connection.FromNodeAndMembers(builder.Nodes[1], new List<IIdeaMember1D>()
			{
				builder.Members[1], builder.Members[2]
			});
			Connection expectedConnection2 = Connection.FromNodeAndMembers(builder.Nodes[2], new List<IIdeaMember1D>()
			{
				builder.Members[2], builder.Members[3]
			});

			bimObjectImporter.Received()
				.Import(
					Arg.Any<IEnumerable<IIdeaObject>>(),
					Arg.Is<IEnumerable<IBimItem>>(x =>
						Enumerable.SequenceEqual(x, new List<IBimItem>() { expectedConnection1, expectedConnection2 }, _connectionEqualityComparer)),
					Arg.Any<IProject>());
		}

		[Test]
		public void ImportConnections_OneMemberAndNodeSelected()
		{
			// Setup: model with one member and one node, the node and the member selected
			GeometryBuilder builder = new GeometryBuilder();
			builder
				.Member(1, "line(0,1)");

			builder.Nodes[1].Name.Returns("connection node1");

			// select both the node and the member
			IIdeaModel model = builder.GetModel();
			model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
				.Do(x =>
				{
					x[0] = new HashSet<IIdeaNode>() { builder.Nodes[1] };
					x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1] };
				});

			BimImporter bimImporter = CreateBimImporter(model);

			// Tested methods
			ModelBIM modelBIM = bimImporter.ImportConnections();

			// Assert: expecting one connection in node 1, member 1
			Connection expectedConnection = Connection.FromNodeAndMembers(builder.Nodes[1], new List<IIdeaMember1D>()
			{
				builder.Members[1]
			});

			bimObjectImporter.Received()
				.Import(
					Arg.Any<IEnumerable<IIdeaObject>>(),
					Arg.Is<IEnumerable<IBimItem>>(x =>
						Enumerable.SequenceEqual(x, new List<IBimItem>() { expectedConnection }, _connectionEqualityComparer)),
					Arg.Any<IProject>());
		}

		[Test]
		public void ImportConnections_TwoIndependedConnectionsWithOneMemberEach()
		{
			// Setup: model with two connections, each with a single member
			GeometryBuilder builder = new GeometryBuilder();
			builder
				.Member(1, "line(0,1)")
				.Member(2, "line(2,3)");

			IIdeaModel model = builder.GetModel();
			model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
				.Do(x =>
				{
					x[0] = new HashSet<IIdeaNode>() { builder.Nodes[1], builder.Nodes[2] };
					x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[2] };
				});

			BimImporter bimImporter = CreateBimImporter(model);

			// Tested methods
			ModelBIM modelBIM = bimImporter.ImportConnections();

			// Assert: expecting 2 connections:
			//	1. node 1, members 1
			//	2. node 2, members 2
			Connection expectedConnection1 = Connection.FromNodeAndMembers(builder.Nodes[1], new List<IIdeaMember1D>()
			{
				builder.Members[1]
			});
			Connection expectedConnection2 = Connection.FromNodeAndMembers(builder.Nodes[2], new List<IIdeaMember1D>()
			{
				builder.Members[2]
			});

			bimObjectImporter.Received()
				.Import(
					Arg.Any<IEnumerable<IIdeaObject>>(),
					Arg.Is<IEnumerable<IBimItem>>(x =>
						Enumerable.SequenceEqual(x, new List<IBimItem>() { expectedConnection1, expectedConnection2 }, _connectionEqualityComparer)),
					Arg.Any<IProject>());
		}

		[Test]
		public void ImportMembers_OneMemberOneSelected()
		{
			// Setup: model with one member
			GeometryBuilder builder = new GeometryBuilder();
			builder
				.Member(1, "line(0,1)");

			IIdeaModel model = builder.GetModel();
			model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
				.Do(x =>
				{
					x[0] = new HashSet<IIdeaNode>();
					x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1] };
				});

			BimImporter bimImporter = CreateBimImporter(model);

			// Tested methods
			ModelBIM modelBIM = bimImporter.ImportMembers();

			// Assert: expecting 1 member and 2 connections on both ends of the member
			Member expectedMember = new Member(builder.Members[1]);
			Connection expectedConnection1 = Connection.FromNodeAndMembers(builder.Nodes[0], new List<IIdeaMember1D>()
			{
				builder.Members[1]
			});
			Connection expectedConnection2 = Connection.FromNodeAndMembers(builder.Nodes[1], new List<IIdeaMember1D>()
			{
				builder.Members[1]
			});

			bimObjectImporter.Received()
				.Import(
					Arg.Any<IEnumerable<IIdeaObject>>(),
					Arg.Is<IEnumerable<IBimItem>>(x =>
						Enumerable.SequenceEqual(x, new List<IBimItem>() { expectedMember, expectedConnection1, expectedConnection2 }, _connectionEqualityComparer)),
					Arg.Any<IProject>());
		}

		[Test]
		public void ImportMembers_ThreeMembersOneSelected()
		{
			// Setup: model with three members connected in a continuous line
			GeometryBuilder builder = new GeometryBuilder();
			builder
				.Member(1, "line(0,1)")
				.Member(2, "line(1,2)")
				.Member(3, "line(1,3)");

			IIdeaModel model = builder.GetModel();
			model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
				.Do(x =>
				{
					x[0] = new HashSet<IIdeaNode>();
					x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1] };
				});

			BimImporter bimImporter = CreateBimImporter(model);

			// Tested methods
			ModelBIM modelBIM = bimImporter.ImportMembers();

			// Assert: expecting 1 member and 2 connections with all member connected to the node
			Member expectedMember = new Member(builder.Members[1]);
			Connection expectedConnection1 = Connection.FromNodeAndMembers(builder.Nodes[0], new List<IIdeaMember1D>()
			{
				builder.Members[1]
			});
			Connection expectedConnection2 = Connection.FromNodeAndMembers(builder.Nodes[1], new List<IIdeaMember1D>()
			{
				builder.Members[1],
				builder.Members[2],
				builder.Members[3],
			});

			bimObjectImporter.Received()
				.Import(
					Arg.Any<IEnumerable<IIdeaObject>>(),
					Arg.Is<IEnumerable<IBimItem>>(x =>
						Enumerable.SequenceEqual(x, new List<IBimItem>() { expectedMember, expectedConnection1, expectedConnection2 }, _connectionEqualityComparer)),
					Arg.Any<IProject>());
		}

		[Test]
		public void ImportMembers_ThreeMembersSharingOneNodeTwoMembersSelected()
		{
			// Setup: model with three members connected into a one node, two members selected
			GeometryBuilder builder = new GeometryBuilder();
			builder
				.Member(1, "line(0,1)")
				.Member(2, "line(1,2)")
				.Member(3, "line(1,3)");

			IIdeaModel model = builder.GetModel();
			model.When(x => x.GetSelection(out Arg.Any<ISet<IIdeaNode>>(), out Arg.Any<ISet<IIdeaMember1D>>()))
				.Do(x =>
				{
					x[0] = new HashSet<IIdeaNode>();
					x[1] = new HashSet<IIdeaMember1D>() { builder.Members[1], builder.Members[3] };
				});

			BimImporter bimImporter = CreateBimImporter(model);

			// Tested methods
			ModelBIM modelBIM = bimImporter.ImportMembers();

			// Assert: expecting 2 members (the selected ones), 3 connections
			Member expectedMember1 = new Member(builder.Members[1]);
			Member expectedMember2 = new Member(builder.Members[3]);
			Connection expectedConnection1 = Connection.FromNodeAndMembers(builder.Nodes[0], new List<IIdeaMember1D>()
			{
				builder.Members[1]
			});
			Connection expectedConnection2 = Connection.FromNodeAndMembers(builder.Nodes[1], new List<IIdeaMember1D>()
			{
				builder.Members[1],
				builder.Members[2],
				builder.Members[3],
			});
			Connection expectedConnection3 = Connection.FromNodeAndMembers(builder.Nodes[3], new List<IIdeaMember1D>()
			{
				builder.Members[3],
			});

			bimObjectImporter.Received()
				.Import(
					Arg.Any<IEnumerable<IIdeaObject>>(),
					Arg.Is<IEnumerable<IBimItem>>(x =>
						Enumerable.SequenceEqual(x, new List<IBimItem>() {
							expectedMember1,
							expectedMember2,
							expectedConnection1,
							expectedConnection2,
							expectedConnection3
						}, _connectionEqualityComparer)),
					Arg.Any<IProject>());
		}
	}
}