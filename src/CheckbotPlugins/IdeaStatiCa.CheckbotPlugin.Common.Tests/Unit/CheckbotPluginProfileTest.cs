﻿#nullable disable

using AutoMapper;
using FluentAssertions;
using IdeaStatiCa.CheckbotPlugin.Common.MappingProfiles;
using NUnit.Framework;

namespace IdeaStatiCa.CheckbotPlugin.Common.Tests.Unit
{
	[TestFixture]
	public class CheckbotPluginProfileTest
	{
		private Mapper _mapper;

		[SetUp]
		public void SetUp()
		{
			MapperConfiguration cfg = new(cfg =>
				cfg.AddProfile(new PluginApiProfile()));

			_mapper = new Mapper(cfg);
		}

		[Test]
		public void Test_ConfigurationValid()
		{
			_mapper.ConfigurationProvider.AssertConfigurationIsValid();
		}

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

			Models.Event dst = _mapper.Map<Models.Event>(src);
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

			Protos.Event dst = _mapper.Map<Protos.Event>(src);

			dst.EventCase.Should().Be(Protos.Event.EventOneofCase.OpenCheckApplication);
			dst.OpenCheckApplication.Object.Type.Should().Be(Protos.ModelObjectType.Node);
			dst.OpenCheckApplication.Object.Id.Should().Be(1);
		}
	}
}