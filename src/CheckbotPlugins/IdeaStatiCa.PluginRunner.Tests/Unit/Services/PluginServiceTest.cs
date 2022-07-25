using FluentAssertions;
using IdeaStatiCa.PluginRunner.Services;
using NUnit.Framework;

namespace IdeaStatiCa.PluginRunner.Tests.Unit.Services
{
	[TestFixture]
	public class PluginServiceTest
	{
		private PluginService sut;

		[SetUp]
		public void SetUp() => sut = new(null);

		[Test]
		public async Task NewVersion_WhenNewVersionIsEmpty_ThrowsException()
		{
			await sut
				.Invoking(x => x.NewVersion(string.Empty))
				.Should()
				.ThrowAsync<ArgumentException>();
		}
	}
}