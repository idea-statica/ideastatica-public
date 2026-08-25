using FluentAssertions;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests
{
	// The sequences handed to Import are deferred: BimImporter.ImportGroup builds them out of
	// IProject.GetBimObject, which re-reads the entity from the BIM application on every enumeration.
	// Walking such a sequence a second time imports the whole model a second time.
	[TestFixture]
	public class BimObjectImporterTest
	{
		private IPluginLogger _logger;
		private IImporter<IIdeaObject> _importer;
		private IResultImporter _resultImporter;
		private IBimResultsProvider _resultsProvider;
		private IProgressMessaging _remoteApp;
		private IProject _project;
		private int _nextIomId;

		[SetUp]
		public void SetUp()
		{
			_logger = Substitute.For<IPluginLogger>();
			_importer = Substitute.For<IImporter<IIdeaObject>>();
			_resultImporter = Substitute.For<IResultImporter>();
			_resultsProvider = Substitute.For<IBimResultsProvider>();
			_remoteApp = Substitute.For<IProgressMessaging>();
			_project = Substitute.For<IProject>();
			_nextIomId = 0;

			_importer.Import(Arg.Any<IImportContext>(), Arg.Any<IIdeaObject>()).Returns(_ => new Member1D());
			_project.GetIomId(Arg.Any<IIdeaObject>()).Returns(_ => ++_nextIomId);
		}

		private BimObjectImporter CreateImporter()
			=> new BimObjectImporter(
				_logger,
				_importer,
				_resultImporter,
				new BimImporterConfiguration(),
				_remoteApp,
				_resultsProvider);

		private static IIdeaObject BimObject(string id)
		{
			IIdeaObject obj = Substitute.For<IIdeaObject>();
			obj.Id.Returns(id);

			return obj;
		}

		private static IBimItem BimItem(string id)
		{
			// Assign first: NSubstitute tracks one "last call" globally, so configuring the referenced
			// object inside the Returns argument would discard the item's own configuration.
			IIdeaObject referencedObject = BimObject(id);
			IBimItem bimItem = Substitute.For<IBimItem>();
			bimItem.ReferencedObject.Returns(referencedObject);
			bimItem.Type.Returns(BIMItemType.Member);

			return bimItem;
		}

		/// <summary>
		/// A sequence that reports every time its enumeration is started, standing in for the deferred
		/// GetBimObject chains the production callers pass.
		/// </summary>
		private static IEnumerable<T> Deferred<T>(IEnumerable<T> items, Action onEnumerationStarted)
		{
			onEnumerationStarted();

			foreach (T item in items)
			{
				yield return item;
			}
		}

		[Test]
		public void The_bim_items_are_enumerated_once()
		{
			int enumerations = 0;
			IBimItem[] bimItems = { BimItem("member-1"), BimItem("member-2") };

			CreateImporter().Import(null, Deferred(bimItems, () => enumerations++), _project, CountryCode.ECEN);

			enumerations.Should().Be(1);
		}

		[Test]
		public void The_objects_are_enumerated_once()
		{
			int enumerations = 0;
			IIdeaObject[] objects = { BimObject("node-1"), BimObject("node-2") };

			CreateImporter().Import(Deferred(objects, () => enumerations++), null, _project, CountryCode.ECEN);

			enumerations.Should().Be(1);
		}

		// The count reported to the progress stage is the whole reason Import materializes at all.
		// Without these, moving the count back behind a null check on the progress sink would restore
		// the double import while both enumeration tests stayed green.
		[Test]
		public void Every_bim_item_is_reported_against_the_total()
		{
			IBimItem[] bimItems = { BimItem("member-1"), BimItem("member-2") };

			CreateImporter().Import(null, Deferred(bimItems, () => { }), _project, CountryCode.ECEN);

			Assert.Multiple(() =>
			{
				_remoteApp.Received(1).SetStageLocalised(1, 2, Arg.Any<LocalisedMessage>());
				_remoteApp.Received(1).SetStageLocalised(2, 2, Arg.Any<LocalisedMessage>());
			});
		}

		[Test]
		public void Every_object_is_reported_against_the_total()
		{
			IIdeaObject[] objects = { BimObject("node-1"), BimObject("node-2") };

			CreateImporter().Import(Deferred(objects, () => { }), null, _project, CountryCode.ECEN);

			Assert.Multiple(() =>
			{
				_remoteApp.Received(1).SetStageLocalised(1, 2, LocalisedMessage.ImportingIOMObject);
				_remoteApp.Received(1).SetStageLocalised(2, 2, LocalisedMessage.ImportingIOMObject);
			});
		}
	}
}
