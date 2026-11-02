using FluentAssertions;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimImporter.Tests
{
	[TestFixture]
	public class FeaApplicationTest
	{
		private IProject _project;
		private IBimImporter _bimImporter;
		private IPluginHook _pluginHook;

		[SetUp]
		public void SetUp()
		{
			_project = Substitute.For<IProject>();
			_bimImporter = Substitute.For<IBimImporter>();
			_pluginHook = Substitute.For<IPluginHook, ISynchronizationHook>();
		}

		private FeaApplication CreateApplication()
			=> new FeaApplication(
				"test",
				Substitute.For<IPluginLogger>(),
				_project,
				Substitute.For<IProjectStorage>(),
				_bimImporter,
				Substitute.For<IBimApiImporter>(),
				_pluginHook,
				Substitute.For<IScopeHook>(),
				Substitute.For<IBimUserDataSource>(),
				TaskScheduler.Default);

		private static BIMItemsGroup Group(params BIMItemId[] items)
			=> new BIMItemsGroup { Type = RequestedItemsType.Connections, Items = new List<BIMItemId>(items) };

		private static BIMItemId Item(BIMItemType type, int id)
			=> new BIMItemId { Type = type, Id = id };

		[Test]
		public void SynchronizationHookGetsTheMembersOfAllGroupsBeforeAnyIsImported()
		{
			var member1 = new IntIdentifier<IIdeaMember1D>(1);
			var member2 = new IntIdentifier<IIdeaMember1D>(2);
			_project.GetPersistenceToken(10).Returns(member1);
			_project.GetPersistenceToken(20).Returns(member2);
			_project.GetPersistenceToken(30).Returns(_ => throw new KeyNotFoundException());
			_project.GetPersistenceToken(40).Returns(new IntIdentifier<IIdeaNode>(4));

			var groups = new List<BIMItemsGroup>
			{
				Group(Item(BIMItemType.Node, 40), Item(BIMItemType.Member, 10), Item(BIMItemType.Member, 20)),
				Group(Item(BIMItemType.Member, 20), Item(BIMItemType.Member, 30)),
			};

			IReadOnlyCollection<Identifier<IIdeaMember1D>> members = null;
			((ISynchronizationHook)_pluginHook)
				.When(x => x.EnterSynchronization(Arg.Any<IReadOnlyCollection<Identifier<IIdeaMember1D>>>()))
				.Do(call => members = call.Arg<IReadOnlyCollection<Identifier<IIdeaMember1D>>>());

			CreateApplication().GetModelForSelection(CountryCode.ECEN, groups);

			members.Should().BeEquivalentTo(new[] { member1, member2 });
			Received.InOrder(() =>
			{
				_pluginHook.EnterImport(CountryCode.ECEN);
				((ISynchronizationHook)_pluginHook).EnterSynchronization(Arg.Any<IReadOnlyCollection<Identifier<IIdeaMember1D>>>());
				_bimImporter.ImportSelected(groups, CountryCode.ECEN);
				_pluginHook.ExitImport(CountryCode.ECEN);
			});
		}
	}
}
