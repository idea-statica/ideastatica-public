using FluentAssertions;
using IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors;
using IdeaStatiCa.CheckbotPlugin.PluginList.Storage;
using NUnit.Framework;
using System.Text;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Tests.Integration
{
	[TestFixture]
	public class PluginListTest
	{
		private PluginList _pluginList;
		private StorageStub _storageStub;

		private static readonly PluginDescriptor pluginDescriptor = new(
			 "testplugin",
			 PluginType.Check,
			 new DotNetRunnerDriverDescriptor(@"c:\plugin1.exe", "TestPlugin.Main"));

		private static readonly PluginDescriptor pluginDescriptor2 = new(
				"testplugin2",
				PluginType.Check,
			 new DotNetRunnerDriverDescriptor(@"c:\plugin2.exe", "TestPlugin.Main"));

		[SetUp]
		public void SetUp()
		{
			_storageStub = new StorageStub();
			_pluginList = new PluginList(_storageStub);
		}

		[Test]
		public async Task GetAll_NoPlugins()
		{
			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();
			_ = plugins.Should().BeEmpty();
		}

		[Test]
		public async Task Add_One()
		{
			await _pluginList.Add(pluginDescriptor);

			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();
			_ = plugins.Count.Should().Be(1);
			_ = plugins.Should().ContainEquivalentOf(pluginDescriptor);
		}

		[Test]
		public async Task Add_Two()
		{
			await _pluginList.Add(pluginDescriptor);
			await _pluginList.Add(pluginDescriptor2);

			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();

			_ = plugins.Count.Should().Be(2);
			_ = plugins.Should().ContainEquivalentOf(pluginDescriptor);
			_ = plugins.Should().ContainEquivalentOf(pluginDescriptor2);
		}

		[Test]
		public async Task Add_Duplicate()
		{
			await _pluginList.Add(pluginDescriptor);

			_ = await _pluginList
				.Awaiting(x => x.Add(pluginDescriptor))
				.Should()
				.ThrowAsync<ArgumentException>();
		}

		[Test]
		public async Task AddOne_RemoveOne()
		{
			await _pluginList.Add(pluginDescriptor);
			_ = await _pluginList.Remove(pluginDescriptor);

			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();
			_ = plugins.Should().BeEmpty();
		}

		[Test]
		public async Task AddTwo_RemoveOne()
		{
			await _pluginList.Add(pluginDescriptor);
			await _pluginList.Add(pluginDescriptor2);

			_ = await _pluginList.Remove(pluginDescriptor);

			IReadOnlyList<PluginDescriptor> plugins = await _pluginList.GetAll();
			_ = plugins.Count.Should().Be(1);
			_ = plugins.Should().ContainEquivalentOf(pluginDescriptor2);
		}

		[Test]
		public async Task Get()
		{
			await _pluginList.Add(pluginDescriptor);
			await _pluginList.Add(pluginDescriptor2);

			PluginDescriptor result = await _pluginList.Get("testplugin");

			_ = result.Should().BeEquivalentTo(pluginDescriptor);
		}
	}

	internal class StorageStub : IStorage
	{
		private byte[] _data;

		public string GetData() => Encoding.UTF8.GetString(_data);

		public Stream GetReadStream()
		{
			if (_data is null)
			{
				return null;
			}

			return new StubStream(_data, x => _data = x);
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
				_ = Read(data, 0, data.Length);

				_flush(data);
			}
		}
	}
}