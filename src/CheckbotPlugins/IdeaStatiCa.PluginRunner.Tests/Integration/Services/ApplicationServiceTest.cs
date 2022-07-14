using FluentAssertions;
using IdeaStatiCa.PluginRunner.Services;
using NSubstitute;
using NUnit.Framework;
using Models = IdeaStatiCa.CheckbotPlugin.Models;
using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Tests.Integration.Services
{
	[TestFixture]
	public class ApplicationServiceTest
	{
		private Protos.ApplicationService.ApplicationServiceClient mockClient;
		private ApplicationService sut;

		[SetUp]
		public void SetUp()
		{
			mockClient = Substitute.For<Protos.ApplicationService.ApplicationServiceClient>();
			sut = new(mockClient);
		}

		[Test]
		public async Task GetAllSettings()
		{
			Protos.GetAllSettingsResp mockResp = new();
			mockResp.Values.Add(new Protos.SettingsValue()
			{
				Name = "name1",
				Value = "value1"
			});
			mockResp.Values.Add(new Protos.SettingsValue()
			{
				Name = "name2",
				Value = "value2"
			});

			Models.SettingsValue[] expectedResp = new[] {
				new Models.SettingsValue("name1", "value1"),
				new Models.SettingsValue("name2", "value2")
			};

			mockClient.GetAllSettingsAsync(Arg.Any<Protos.GetAllSettingsReq>())
				.Returns(Utils.CreateUnaryCall(mockResp));

			IReadOnlyCollection<Models.SettingsValue> actualResp = await sut.GetAllSettings();

			actualResp.Should().BeEquivalentTo(expectedResp);
		}

		[Test]
		public async Task GetSettings()
		{
			Protos.GetSettingsResp mockResp = new()
			{
				Value = new()
				{
					Name = "name1",
					Value = "value1"
				}
			};

			mockClient.GetSettingsAsync(Arg.Any<Protos.GetSettingsReq>())
				.Returns(Utils.CreateUnaryCall(mockResp));

			string actualResp = await sut.GetSettings("name1");

			actualResp.Should().Be("value1");
		}
	}
}