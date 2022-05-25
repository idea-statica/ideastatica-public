using IdeaStatiCa.Public;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.Plugin.Tests.ProjectContentOnDisk
{
	[TestFixture]
	public class IOMethodsTest
	{
		private const string TestFileName = "testfile.txt";
		private const string TestString = "ABCDEFGHI";
		private static readonly string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

		[OneTimeSetUp]
		public static void Initialize()
		{
			Directory.CreateDirectory(tempDirectory);
		}

		[OneTimeTearDown]
		public static void ClassCleanUp()
		{
			Directory.Delete(tempDirectory, true);
		}

		[TearDown]
		public void TestCleanUp()
		{
			File.Delete(Path.Combine(tempDirectory, TestFileName));
		}

		[Test]
		public void Test_CreateMethod()
		{
			var filePath = Path.Combine(tempDirectory, TestFileName);
			string content;
			var contentOnDisc = new ProjectContentOnDisc(tempDirectory);
			contentOnDisc.Create(TestFileName).Close();
			Assert.IsTrue(File.Exists(filePath), "File doesn't exists.");

			using (StreamWriter sw = File.AppendText(filePath))
			{
				sw.Write(TestString);
			}
			contentOnDisc.Create(TestFileName).Close();

			using (FileStream fs = new FileStream(filePath, FileMode.Open))
			{
				using (StreamReader reader = new StreamReader(fs))
				{
					content = reader.ReadToEnd();
				}
			}
			Assert.IsTrue(content.Equals(""), "Content of the stream is not as expected.");
		}

		[Test]
		public void Test_DeleteMethod()
		{
			File.Create(Path.Combine(tempDirectory, TestFileName)).Close();
			var contentOnDisc = new ProjectContentOnDisc(Path.Combine(tempDirectory));

			// Tests if implementation deletes file.
			contentOnDisc.Delete(TestFileName);
			Assert.IsFalse(File.Exists(TestFileName), "File wasn't deleted.");
			Assert.That(() => contentOnDisc.Delete(TestFileName), Throws.TypeOf<FileNotFoundException>());
		}

		[Test]
		public void TestExistsMethod()
		{
			var contentOnDisc = new ProjectContentOnDisc(tempDirectory);
			Assert.IsFalse(contentOnDisc.Exist(TestFileName), "File exists.");

			File.Create(Path.Combine(tempDirectory, TestFileName)).Close();
			Assert.IsTrue(contentOnDisc.Exist(TestFileName), "File doesn't exists.");
		}

		[Test]
		public void TestGetMethod()
		{
			var filePath = Path.Combine(tempDirectory, TestFileName);
			string content;
			using (StreamWriter sw = File.AppendText(filePath))
			{
				sw.Write(TestString);
			}
			var contentOnDisc = new ProjectContentOnDisc(tempDirectory);

			using (Stream fs = contentOnDisc.Get(TestFileName))
			{
				Assert.IsNotNull(fs, "Filestream is null.");
				using (StreamReader reader = new StreamReader(fs))
				{
					content = reader.ReadToEnd();
				}
			}

			Assert.IsTrue(content.Equals(TestString), "Content of the stream is not as expected.");
		}

		[Test]
		public void TestGetContentMethod()
		{
			var contentOnDisc = new ProjectContentOnDisc(tempDirectory);
			CreateTestDirectories();

			var actual = contentOnDisc.GetContent();
			var expected = new List<ProjectDataItem>
			{
				new ProjectDataItem("A", ItemType.Group),
				new ProjectDataItem("C", ItemType.Group),
				new ProjectDataItem("A\\B", ItemType.Group),
				new ProjectDataItem(Path.Combine("A", TestFileName), ItemType.File)
			};

			CollectionAssert.AreEquivalent(expected, actual);
		}

		[Test]
		public void TestCopyContentMethod()
		{
			var testDirPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.Delete(tempDirectory, true); //previous test cleanup

			var srcContentOnDisc = new ProjectContentOnDisc(tempDirectory);
			var currContentOnDisc = new ProjectContentOnDisc(testDirPath);
			CreateTestDirectories();
			currContentOnDisc.CopyContent(srcContentOnDisc);

			var actual = srcContentOnDisc.GetContent();
			var expected = currContentOnDisc.GetContent();

			CollectionAssert.AreEquivalent(expected, actual);
		}

		private void CreateTestDirectories()
		{
			var aDir = Path.Combine(tempDirectory, "A");
			Directory.CreateDirectory(aDir);
			File.Create(Path.Combine(aDir, TestFileName)).Close();
			Directory.CreateDirectory(Path.Combine(aDir, "B"));
			Directory.CreateDirectory(Path.Combine(tempDirectory, "C"));
		}
	}
}
