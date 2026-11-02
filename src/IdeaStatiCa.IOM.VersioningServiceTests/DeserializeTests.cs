
using FluentAssertions;
using IdeaStatiCa.Plugin;
using NUnit.Framework;

namespace IdeaStatiCa.OpenModel.VersioningServiceTests
{
	[TestFixture]
	public class DeserializeTests
	{

		private IPluginLogger _logger;

		private readonly string TestData = "TestData";

		[SetUp]
		public void Setup()
		{
			_logger = new NullLogger();
		}

		[TestCase("CHKUpgraded.xml")]
		public void Deserialize_OpenModelContainer(string fileName)
		{
			string xmlContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, fileName));
			try
			{
				var container = IdeaRS.OpenModel.Tools.OpenModelContainerFromXml(xmlContent);

				container.Should().NotBeNull();
				container.Should().BeAssignableTo(typeof(IdeaRS.OpenModel.OpenModelContainer));

			}
			catch
			{
				IdeaStatiCa.IOM.VersioningServiceTests.UtHelper.Attach(xmlContent, $"{fileName}.xml");
				throw;
			}
		}

		[TestCase("OpenModel-SimpleUpgraded.xml")]
		[TestCase("OpenModel-LargeUpgraded.xml")]
		public void Deserialize_OpenMode(string fileName)
		{
			string xmlContent = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, TestData, fileName));
			try
			{
				var openModel = IdeaRS.OpenModel.OpenModel.LoadFromString(xmlContent);

				openModel.Should().NotBeNull();
				openModel.Should().BeAssignableTo(typeof(IdeaRS.OpenModel.OpenModel));

			}
			catch
			{
				IdeaStatiCa.IOM.VersioningServiceTests.UtHelper.Attach(xmlContent, $"{fileName}.xml");
				throw;
			}
		}
	}
}
