using AutoMapper;
using FluentAssertions;
using IdeaStatiCa.CheckbotPlugin.PluginList.Serialization;
using IdeaStatiCa.PluginSystem.PluginList.Descriptors;
using IdeaStatiCa.PluginSystem.PluginList.Json;
using NUnit.Framework;

namespace IdeaStatiCa.PluginSystem.PluginList.Tests.Unit
{
	[TestFixture]
	public class JsonMappingProfileTest
	{
		private Mapper _mapper;

		[SetUp]
		public void SetUp()
		{
			MapperConfiguration cfg = new(cfg =>
				cfg.AddProfile(typeof(JsonMappingProfile)));

			_mapper = new Mapper(cfg);
		}

		[Test]
		public void Test_ConfigurationValid()
		{
			_mapper.ConfigurationProvider.AssertConfigurationIsValid();
		}

		[Test]
		public void Test_Mapping_JsonDotNetRunnerDriver_Polymorhic()
		{
			Driver driver = new DotNetRunnerDriver()
			{
				ClassName = "TestClass",
				Path = @"c:\plugin.exe"
			};

			PluginDriverDescriptor descriptor = _mapper.Map<PluginDriverDescriptor>(driver);
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

			PluginDescriptor descriptor = _mapper.Map<PluginDescriptor>(plugin);

			descriptor.Name.Should().Be("testplugin");
			descriptor.Type.Should().Be(PluginType.Check);

			descriptor.DriverDescriptor.Should().BeOfType<DotNetRunnerDriverDescriptor>();
			DotNetRunnerDriverDescriptor dotNetRunnerDriverDescriptor = (DotNetRunnerDriverDescriptor)descriptor.DriverDescriptor;
			dotNetRunnerDriverDescriptor.ClassName.Should().Be("TestClass");
			dotNetRunnerDriverDescriptor.Path.Should().Be(@"c:\plugin.exe");
		}
	}
}