using FluentAssertions;
using IdeaStatiCa.CheckbotPlugin.Services;
using IdeaStatiCa.PluginRunner.Services;
using NUnit.Framework;

namespace IdeaStatiCa.PluginRunner.Tests.Unit
{
	[TestFixture]
	public class KeyValueStorageTest
	{
		private IKeyValueStorage subject;

		[SetUp]
		public void SetUp()
		{
			subject = new KeyValueStorageGrpc(null);
		}

		[Test]
		public async Task Delete_WhenKeyIsEmpty_ThrowsException()
		{
			await subject
				.Invoking(x => x.Delete(string.Empty))
				.Should()
				.ThrowAsync<ArgumentException>();
		}

		[Test]
		public async Task Delete_WhenKeyIsNull_ThrowsException()
		{
			await subject
				.Invoking(x => x.Delete(null))
				.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public async Task Exists_WhenKeyIsEmpty_ThrowsException()
		{
			await subject
				.Invoking(x => x.Exists(string.Empty))
				.Should()
				.ThrowAsync<ArgumentException>();
		}

		[Test]
		public async Task Exists_WhenKeyIsNull_ThrowsException()
		{
			await subject
				.Invoking(x => x.Exists(null))
				.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public async Task Get_WhenKeyIsEmpty_ThrowsException()
		{
			await subject
				.Invoking(x => x.Get(string.Empty))
				.Should()
				.ThrowAsync<ArgumentException>();
		}

		[Test]
		public async Task Get_WhenKeyIsNull_ThrowsException()
		{
			await subject
				.Invoking(x => x.Delete(null))
				.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public async Task Set_WhenKeyIsEmpty_ThrowsException()
		{
			await subject
				.Invoking(x => x.Set(string.Empty, new byte[1]))
				.Should()
				.ThrowAsync<ArgumentException>();
		}

		[Test]
		public async Task Set_WhenValueIsEmpty_ThrowsException()
		{
			await subject
				.Invoking(x => x.Set("key", null))
				.Should()
				.ThrowAsync<ArgumentException>();
		}

		[Test]
		public async Task Set_WhenKeyIsNull_ThrowsException()
		{
			await subject
				.Invoking(x => x.Set(null, new byte[1]))
				.Should()
				.ThrowAsync<ArgumentNullException>();
		}
	}
}