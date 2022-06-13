using FluentAssertions;
using IdeaStatiCa.CheckbotPlugin.Models;
using IdeaStatiCa.PluginRunner.Services;
using NUnit.Framework;

namespace IdeaStatiCa.PluginRunner.Tests.Unit.Services
{
	[TestFixture]
	public class ProjectServiceTest
	{
		private ProjectService sut;

		[SetUp]
		public void SetUp() => sut = new(null);

		[Test]
		public async Task GetObjects_WhenListIsEmpty_ThrowsException()
		{
			await sut
				.Invoking(x => x.GetObjects(new(), ModelExportOptions.Default))
				.Should()
				.ThrowAsync<ArgumentException>();
		}
	}
}