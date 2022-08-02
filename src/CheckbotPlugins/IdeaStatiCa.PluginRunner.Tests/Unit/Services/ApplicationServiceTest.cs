using FluentAssertions;
using IdeaStatiCa.PluginRunner.Services;
using NUnit.Framework;

namespace IdeaStatiCa.PluginRunner.Tests.Unit.Services
{
	[TestFixture]
	public class ApplicationServiceTest
	{
		private ApplicationService sut;

		[SetUp]
		public void SetUp() => sut = new(null);

		[Test]
		public async Task GetSettings_WhenNameIsEmpty_ThrowsException()
		{
			await sut
				.Invoking(x => x.GetSettings(string.Empty))
				.Should()
				.ThrowAsync<ArgumentException>();
		}
	}
}