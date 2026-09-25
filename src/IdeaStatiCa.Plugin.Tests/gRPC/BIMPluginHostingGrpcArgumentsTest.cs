using FluentAssertions;
using NUnit.Framework;

namespace IdeaStatiCa.Plugin.Tests.gRPC
{
	/// <summary>
	/// The command line the host starts the IDEA StatiCa application with. A link may add to it — one that already
	/// collected a project's design codes needs Checkbot to create the project rather than ask for them — through the
	/// opt-in <see cref="IBIMPluginFactoryWithArguments"/>.
	/// </summary>
	[TestFixture]
	public class BIMPluginHostingGrpcArgumentsTest
	{
		[Test]
		public void WithoutExtraArguments_TheCommandLineIsTheOneItAlwaysWas()
		{
			string arguments = BIMPluginHostingGrpc.BuildIdeaStatiCaArguments("1234", @"D:\projects\Bridge", 50000, null);

			arguments.Should().Be(@"-automation:1234 -project:""D:\projects\Bridge"" -grpcPort:50000");
		}

		[TestCase("")]
		[TestCase("   ")]
		public void AnEmptyExtraArgument_ChangesNothing(string extraArguments)
		{
			string arguments = BIMPluginHostingGrpc.BuildIdeaStatiCaArguments("1234", @"D:\projects\Bridge", 50000, extraArguments);

			arguments.Should().Be(@"-automation:1234 -project:""D:\projects\Bridge"" -grpcPort:50000");
		}

		[Test]
		public void ExtraArguments_AreAppendedAfterTheHostsOwn()
		{
			string arguments = BIMPluginHostingGrpc.BuildIdeaStatiCaArguments(
				"1234",
				@"D:\projects\Bridge",
				50000,
				"-new -designCode:ECEN -steelCodeEdition:EN_1993_1_8_2024");

			arguments.Should().Be(
				@"-automation:1234 -project:""D:\projects\Bridge"" -grpcPort:50000 -new -designCode:ECEN -steelCodeEdition:EN_1993_1_8_2024");
		}

		// The project directory stays the host's to decide, so a quoted path with spaces must survive intact.
		[Test]
		public void AProjectPathWithSpaces_StaysQuoted()
		{
			string arguments = BIMPluginHostingGrpc.BuildIdeaStatiCaArguments("1234", @"D:\my projects\Office block", 50000, "-new");

			arguments.Should().Contain(@"-project:""D:\my projects\Office block""");
		}
	}
}
