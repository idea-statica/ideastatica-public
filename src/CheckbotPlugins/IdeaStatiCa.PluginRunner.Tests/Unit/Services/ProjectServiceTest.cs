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

	}
}