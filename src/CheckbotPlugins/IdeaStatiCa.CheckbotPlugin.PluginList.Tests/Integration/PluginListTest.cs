#nullable disable

using FluentAssertions;
using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.PluginSystem.PluginList.Descriptors;
using IdeaStatiCa.PluginSystem.PluginList.Storage;
using NUnit.Framework;

namespace IdeaStatiCa.PluginSystem.PluginList.Tests.Integration
{
	[TestFixture]
	public class PluginListTest
	{
		private PluginList _pluginList;

		private static readonly PluginDescriptor pluginDescriptor = new(
			 "testplugin",
			 PluginType.Check,
			 new DotNetRunnerDriverDescriptor(@"c:\plugin1.exe", "TestPlugin.Main"));

		private static readonly PluginDescriptor pluginDescriptor2 = new(
				"testplugin2",
				PluginType.Check,
			 new DotNetRunnerDriverDescriptor(@"c:\plugin2.exe", "TestPlugin.Main"));

		[SetUp]
		public void SetUp() => _pluginList = new PluginList(new StorageStub());

		[Test]
		public async Task GetAll_NoPlugins()
		{
			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();
			plugins.Should().BeEmpty();
		}

		[Test]
		public async Task Add_One()
		{
			await _pluginList.Add(pluginDescriptor);

			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();
			plugins.Count.Should().Be(1);
			plugins.Should().ContainEquivalentOf(pluginDescriptor);
		}

		[Test]
		public async Task Add_Two()
		{
			await _pluginList.Add(pluginDescriptor);
			await _pluginList.Add(pluginDescriptor2);

			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();

			plugins.Count.Should().Be(2);
			plugins.Should().ContainEquivalentOf(pluginDescriptor);
			plugins.Should().ContainEquivalentOf(pluginDescriptor2);
		}

		[Test]
		public async Task Add_Duplicate()
		{
			await _pluginList.Add(pluginDescriptor);

			await _pluginList
				.Awaiting(x => x.Add(pluginDescriptor))
				.Should()
				.ThrowAsync<ArgumentException>();
		}

		[Test]
		public async Task AddOne_RemoveOne()
		{
			await _pluginList.Add(pluginDescriptor);
			await _pluginList.Remove(pluginDescriptor);

			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();
			plugins.Should().BeEmpty();
		}

		[Test]
		public async Task AddTwo_RemoveOne()
		{
			await _pluginList.Add(pluginDescriptor);
			await _pluginList.Add(pluginDescriptor2);

			await _pluginList.Remove(pluginDescriptor);

			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();
			plugins.Count.Should().Be(1);
			plugins.Should().ContainEquivalentOf(pluginDescriptor2);
		}

		[Test]
		public async Task Get()
		{
			await _pluginList.Add(pluginDescriptor);
			await _pluginList.Add(pluginDescriptor2);

			PluginDescriptor result = await _pluginList.Get("testplugin");

			result.Should().BeEquivalentTo(pluginDescriptor);
		}
	}

	internal class StorageStub : IStorage
	{
		private byte[] _data;

		public Maybe<Stream> GetReadStream()
		{
			if (_data is null)
			{
				return new Maybe<Stream>();
			}

			return new Maybe<Stream>(new StubStream(_data, x => _data = x));
		}

		public Stream GetWriteStream()
		{
			if (_data is null)
			{
				return new StubStream(x => _data = x);
			}

			return new StubStream(_data, x => _data = x);
		}

		private sealed class StubStream : MemoryStream
		{
			private readonly Action<byte[]> _flush;

			public StubStream(Action<byte[]> flush)
				: base()
			{
				_flush = flush;
			}

			public StubStream(byte[] buffer, Action<byte[]> flush)
				: base()
			{
				Write(buffer, 0, buffer.Length);
				Position = 0;

				_flush = flush;
			}

			protected override void Dispose(bool disposing)
			{
				byte[] data = new byte[Position];
				Position = 0;
				Read(data, 0, data.Length);

				_flush(data);
			}
		}
	}
}