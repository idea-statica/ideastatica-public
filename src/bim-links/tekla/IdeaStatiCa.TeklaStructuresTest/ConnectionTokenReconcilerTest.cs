using FluentAssertions;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.TeklaStructuresTest
{
	/// <summary>
	/// Pure tests for the CAD re-sync token reconcile (the reliability-critical part of the added-element re-discovery).
	/// The Tekla-touching connectivity walk (<c>IdentifierHelper.GetConnectedIdentifiers</c>) needs a live model and is
	/// verified manually; here we feed a synthetic "discovered" list and assert how the token is merged.
	/// </summary>
	public class ConnectionTokenReconcilerTest
	{
		private static ConnectionIdentifier<IIdeaConnectionPoint> MakeToken(
			IEnumerable<string> members = null,
			IEnumerable<string> plates = null,
			IEnumerable<string> welds = null)
		{
			var id = new ConnectionIdentifier<IIdeaConnectionPoint>(0, 0, 0)
			{
				ConnectedMembers = new List<ImmutableIdentifier<IIdeaConnectedMember>>(),
				Plates = new List<ImmutableIdentifier<IIdeaPlate>>(),
				BoltGrids = new List<ImmutableIdentifier<IIdeaBoltGrid>>(),
				AnchorGrids = new List<ImmutableIdentifier<IIdeaAnchorGrid>>(),
				Welds = new List<ImmutableIdentifier<IIdeaWeld>>(),
				Cuts = new List<ImmutableIdentifier<IIdeaCut>>(),
				FoldedPlates = new List<ImmutableIdentifier<IIdeaFoldedPlate>>(),
			};

			foreach (string m in members ?? Enumerable.Empty<string>())
			{
				id.ConnectedMembers.Add(new ConnectedMemberIdentifier<IIdeaConnectedMember>(m));
			}

			foreach (string p in plates ?? Enumerable.Empty<string>())
			{
				id.Plates.Add(new StringIdentifier<IIdeaPlate>(p));
			}

			foreach (string w in welds ?? Enumerable.Empty<string>())
			{
				id.Welds.Add(new StringIdentifier<IIdeaWeld>(w));
			}

			return id;
		}

		private static IReadOnlyList<string> Ids<T>(IEnumerable<ImmutableIdentifier<T>> collection)
			where T : IIdeaObject
			=> collection.Select(x => x.GetId().ToString()).ToList();

		// 1. The user's case: a stiffener plate added in Tekla and surfaced by discovery is merged into the token.
		[Test]
		public void Apply_addsNewlyDiscoveredPlate()
		{
			var id = MakeToken(members: new[] { "M1" });
			var discovered = new List<IIdentifier> { new StringIdentifier<IIdeaPlate>("P1") };

			ConnectionTokenReconciler.Apply(id, discovered, Substitute.For<IPluginLogger>());

			Ids(id.Plates).Should().ContainSingle().Which.Should().Be("P1");
		}

		// 2. A discovered component already in the token is not duplicated.
		[Test]
		public void Apply_doesNotDuplicateExistingComponent()
		{
			var id = MakeToken(members: new[] { "M1" }, plates: new[] { "P1" });
			var discovered = new List<IIdentifier> { new StringIdentifier<IIdeaPlate>("P1") };

			ConnectionTokenReconciler.Apply(id, discovered, Substitute.For<IPluginLogger>());

			Ids(id.Plates).Should().Equal("P1");
		}

		// 3. The same component discovered twice (e.g. reached from two members) yields a single entry.
		[Test]
		public void Apply_dedupsSameComponentDiscoveredTwice()
		{
			var id = MakeToken(members: new[] { "M1" });
			var discovered = new List<IIdentifier>
			{
				new StringIdentifier<IIdeaPlate>("P1"),
				new StringIdentifier<IIdeaPlate>("P1"),
			};

			ConnectionTokenReconciler.Apply(id, discovered, Substitute.For<IPluginLogger>());

			Ids(id.Plates).Should().Equal("P1");
		}

		// 4. Keep-if-live: an existing component that discovery does not re-surface is NOT dropped (miss-drop regression).
		[Test]
		public void Apply_keepsExistingComponentNotRediscovered()
		{
			var id = MakeToken(members: new[] { "M1" }, plates: new[] { "P1" });
			var discovered = new List<IIdentifier> { new StringIdentifier<IIdeaPlate>("P2") };

			ConnectionTokenReconciler.Apply(id, discovered, Substitute.For<IPluginLogger>());

			Ids(id.Plates).Should().BeEquivalentTo("P1", "P2");
		}

		// 5. Structural member-set is frozen: a newly connected member is NOT added, and it is logged as a topology change.
		[Test]
		public void Apply_freezesNewStructuralMemberAndLogsIt()
		{
			var id = MakeToken(members: new[] { "M1" });
			var logger = Substitute.For<IPluginLogger>();
			var discovered = new List<IIdentifier> { new ConnectedMemberIdentifier<IIdeaConnectedMember>("M2") };

			ConnectionTokenReconciler.Apply(id, discovered, logger);

			Ids(id.ConnectedMembers).Should().Equal("M1");
			logger.Received(1).LogInformation(Arg.Is<string>(s => s.Contains("M2")), Arg.Any<Exception>());
		}

		// 6. A frozen member re-surfaced by discovery is a no-op (no duplicate, no topology log).
		[Test]
		public void Apply_ignoresRediscoveredExistingMember()
		{
			var id = MakeToken(members: new[] { "M1" });
			var logger = Substitute.For<IPluginLogger>();
			var discovered = new List<IIdentifier> { new ConnectedMemberIdentifier<IIdeaConnectedMember>("M1") };

			ConnectionTokenReconciler.Apply(id, discovered, logger);

			Ids(id.ConnectedMembers).Should().Equal("M1");
			logger.DidNotReceive().LogInformation(Arg.Any<string>(), Arg.Any<Exception>());
		}

		// 7. Each non-member kind lands in its own token collection.
		[Test]
		public void Apply_routesEachComponentKindToItsCollection()
		{
			var id = MakeToken(members: new[] { "M1" });
			var discovered = new List<IIdentifier>
			{
				new StringIdentifier<IIdeaPlate>("P1"),
				new StringIdentifier<IIdeaBoltGrid>("B1"),
				new StringIdentifier<IIdeaWeld>("W1"),
				new StringIdentifier<IIdeaCut>("C1"),
				new StringIdentifier<IIdeaAnchorGrid>("A1"),
				new StringIdentifier<IIdeaFoldedPlate>("F1"),
			};

			ConnectionTokenReconciler.Apply(id, discovered, Substitute.For<IPluginLogger>());

			Ids(id.Plates).Should().Equal("P1");
			Ids(id.BoltGrids).Should().Equal("B1");
			Ids(id.Welds).Should().Equal("W1");
			Ids(id.Cuts).Should().Equal("C1");
			Ids(id.AnchorGrids).Should().Equal("A1");
			Ids(id.FoldedPlates).Should().Equal("F1");
		}

		// 8. Idempotent: re-applying the same discovery result does not churn or duplicate.
		[Test]
		public void Apply_isIdempotent()
		{
			var id = MakeToken(members: new[] { "M1" });
			var discovered = new List<IIdentifier> { new StringIdentifier<IIdeaPlate>("P1") };

			ConnectionTokenReconciler.Apply(id, discovered, Substitute.For<IPluginLogger>());
			ConnectionTokenReconciler.Apply(id, discovered, Substitute.For<IPluginLogger>());

			Ids(id.Plates).Should().Equal("P1");
		}

		// 9. Degenerate inputs: empty / null discovery is a no-op and never throws.
		[Test]
		public void Apply_withEmptyOrNullDiscovery_isNoOp()
		{
			var id = MakeToken(members: new[] { "M1" }, plates: new[] { "P1" });

			ConnectionTokenReconciler.Apply(id, new List<IIdentifier>(), Substitute.For<IPluginLogger>());
			ConnectionTokenReconciler.Apply(id, null, Substitute.For<IPluginLogger>());

			Ids(id.Plates).Should().Equal("P1");
			Ids(id.ConnectedMembers).Should().Equal("M1");
		}

		// 10. A null logger is tolerated even when a topology change would be logged.
		[Test]
		public void Apply_withNullLogger_doesNotThrowOnNewMember()
		{
			var id = MakeToken(members: new[] { "M1" });
			var discovered = new List<IIdentifier> { new ConnectedMemberIdentifier<IIdeaConnectedMember>("M2") };

			Action act = () => ConnectionTokenReconciler.Apply(id, discovered, null);

			act.Should().NotThrow();
			Ids(id.ConnectedMembers).Should().Equal("M1");
		}
	}
}
