#nullable disable

using FluentAssertions;
using IdeaStatiCa.CheckbotPlugin.Common.Mappers;
using NUnit.Framework;

namespace IdeaStatiCa.CheckbotPlugin.Common.Tests.Unit
{
	[TestFixture]
	public class CheckbotPluginProfileTest
	{
		[Test]
		public void Test_Mapping_EventOpenCheckApplication()
		{
			Protos.Event src = new()
			{
				OpenCheckApplication = new()
				{
					Object = new()
					{
						Id = 1,
						Type = Protos.ModelObjectType.Node
					}
				}
			};

			Models.Event dst = Mapper.Map(src);
			dst.Should().BeOfType<Models.EventOpenCheckApplication>();

			Models.EventOpenCheckApplication evt = (Models.EventOpenCheckApplication)dst;
			evt.ModelObject.Id.Should().Be(1);
			evt.ModelObject.Type.Should().Be(Models.ModelObjectType.Node);
		}

		[Test]
		public void Test_Mapping_EventOpenCheckApplication_Back()
		{
			Models.EventOpenCheckApplication src = new(
				new(Models.ModelObjectType.Node, 1));

			Protos.Event dst = Mapper.Map((Models.Event)src);

			dst.EventCase.Should().Be(Protos.Event.EventOneofCase.OpenCheckApplication);
			dst.OpenCheckApplication.Object.Type.Should().Be(Protos.ModelObjectType.Node);
			dst.OpenCheckApplication.Object.Id.Should().Be(1);
		}

		[Test]
		public void Test_Mapping_EventCustomButtonClicked()
		{
			Protos.Event src = new()
			{
				CustomButtonClicked = new()
				{
					ButtonName = "my-button"
				}
			};

			Models.Event dst = Mapper.Map(src);
			dst.Should().BeOfType<Models.EventCustomButtonClicked>();

			Models.EventCustomButtonClicked evt = (Models.EventCustomButtonClicked)dst;
			evt.ButtonName.Should().Be("my-button");
		}

		[Test]
		public void Test_Mapping_EventCustomButtonClicked_Back()
		{
			Models.EventCustomButtonClicked src = new("my-button");

			Protos.Event dst = Mapper.Map((Models.Event)src);

			dst.EventCase.Should().Be(Protos.Event.EventOneofCase.CustomButtonClicked);
			dst.CustomButtonClicked.ButtonName.Should().Be("my-button");
		}
	}
}