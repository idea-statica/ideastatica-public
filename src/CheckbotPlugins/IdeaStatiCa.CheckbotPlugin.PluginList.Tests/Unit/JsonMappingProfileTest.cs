using FluentAssertions;
using IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors;
using IdeaStatiCa.CheckbotPlugin.PluginList.Json;
using IdeaStatiCa.CheckbotPlugin.PluginList.Mappers;
using NUnit.Framework;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Tests.Unit
{
	[TestFixture]
	public class JsonMappingProfileTest
	{
		[Test]
		public void Test_Mapping_JsonDotNetRunnerDriver_Polymorhic()
		{
			Driver driver = new DotNetRunnerDriver()
			{
				ClassName = "TestClass",
				Path = @"c:\plugin.exe"
			};

			PluginDriverDescriptor descriptor = Mapper.Map(driver);
			descriptor.Should().BeOfType<DotNetRunnerDriverDescriptor>();

			DotNetRunnerDriverDescriptor dotNetRunnerDriverDescriptor = (DotNetRunnerDriverDescriptor)descriptor;
			dotNetRunnerDriverDescriptor.ClassName.Should().Be("TestClass");
			dotNetRunnerDriverDescriptor.Path.Should().Be(@"c:\plugin.exe");
		}

		[Test]
		public void Test_Mapping_JsonPlugin()
		{
			Plugin plugin = new Plugin()
			{
				Name = "testplugin",
				Type = JsonPluginType.Check,
				Driver = new DotNetRunnerDriver()
				{
					ClassName = "TestClass",
					Path = @"c:\plugin.exe"
				}
			};

			PluginDescriptor descriptor = Mapper.Map(plugin);

			descriptor.Name.Should().Be("testplugin");
			descriptor.Type.Should().Be(PluginType.Check);

			descriptor.DriverDescriptor.Should().BeOfType<DotNetRunnerDriverDescriptor>();
			DotNetRunnerDriverDescriptor dotNetRunnerDriverDescriptor = (DotNetRunnerDriverDescriptor)descriptor.DriverDescriptor;
			dotNetRunnerDriverDescriptor.ClassName.Should().Be("TestClass");
			dotNetRunnerDriverDescriptor.Path.Should().Be(@"c:\plugin.exe");
		}

		[Test]
		public void Test_Mapping_CustomAction()
		{
			Plugin plugin = new Plugin()
			{
				Name = "testplugin",
				Type = JsonPluginType.Check,
				Driver = new DotNetRunnerDriver()
				{
					ClassName = "TestClass",
					Path = @"c:\plugin.exe"
				},
				CustomActions = new ActionButton[]
				{
					new ActionButton()
					{
						Name = "myaction",
						Image = "image",
						Text = "text",
						Tooltip = "tooltip"
					}
				}
			};

			PluginDescriptor descriptor = Mapper.Map(plugin);

			descriptor.CustomActionDescriptors.Should().NotBeEmpty();

			var customAction = descriptor.CustomActionDescriptors[0];
			customAction.Name.Should().Be("myaction");
			customAction.Image.Should().Be("image");
			customAction.Text.Should().Be("text");
			customAction.Tooltip.Should().Be("tooltip");
		}
	}
}