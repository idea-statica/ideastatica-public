using FluentAssertions;
using IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors;
using IdeaStatiCa.CheckbotPlugin.PluginList.Storage;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Tests.Unit
{
	[TestFixture]
	public class PluginListTest
	{
		private PluginList _pluginList;

		[SetUp]
		public void SetUp()
		{
			_pluginList = new PluginList(Substitute.For<IStorage>());
		}

		[Test]
		public void Ctor_WhenStorageIsNull_ThrowsArgumentNullException()
		{
			FluentActions.Invoking(() => new PluginList(null))
				.Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void Get_WhenArgIsNull_ThrowsArgumentNullException()
		{
			_pluginList
				.Invoking(x => x.Get(null))
				.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public void Get_WhenArgIsEmpty_ThrowsArgumentException()
		{
			_pluginList
				.Invoking(x => x.Get(""))
				.Should()
				.ThrowAsync<ArgumentException>();
		}

		[Test]
		public void Add_WhenArgIsNull_ThrowsArgumentNullException()
		{
			_pluginList
				.Invoking(x => x.Add(null))
				.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public void Remove_WhenArgIsNull_ThrowsArgumentNullException()
		{
			_pluginList
				.Invoking(x => x.Remove((PluginDescriptor)null))
				.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public void RemoveByName_WhenArgIsNull_ThrowsArgumentNullException()
		{
			_pluginList
				.Invoking(x => x.Remove((string)null))
				.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public void RemoveByName_WhenArgIsEmpty_ThrowsArgumentException()
		{
			_pluginList
				.Invoking(x => x.Remove(""))
				.Should()
				.ThrowAsync<ArgumentNullException>();
		}
	}
}