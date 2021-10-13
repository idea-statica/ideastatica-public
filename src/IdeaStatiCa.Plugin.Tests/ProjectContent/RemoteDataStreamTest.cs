using FluentAssertions;
using IdeaStatiCa.Plugin.ProjectContent;
using NSubstitute;
using System;
using System.IO;
using Xunit;

namespace IdeaStatiCa.Plugin.Tests.ProjectContent
{
	public class RemoteDataStreamTest
	{
		[Fact]
		public void InitStreamTest()
		{
			string contentId = "item1";
			var storage = Substitute.For<IProjectContentStorage>();
			storage.ReadData(default, default).ReturnsForAnyArgs(t =>
			{
				var arg1 = (t[0] as string);
				Stream st = (t[1] as Stream);

				if (arg1 == contentId)
				{
					st.WriteByte(1);
					st.WriteByte(2);
				}
				else
				{
					throw new Exception();
				}

				return 1;
			});

			using (MemoryStream writtenData = new MemoryStream())
			{

				storage.WriteData(default, default).ReturnsForAnyArgs(t =>
				{
					var arg1 = (t[0] as string);
					Stream st = (t[1] as Stream);

					if (arg1 == contentId)
					{
						st.CopyTo(writtenData);
					}
					else
					{
						throw new Exception();
					}

					return 1;
				});

				{
					// initialize stream - it should write 2 bytes
					var stream = new RemoteDataStream(contentId, storage);

					stream.Seek(0, SeekOrigin.Begin);
					byte val = 0;
					val = (byte)stream.ReadByte();
					val.Should().Be(1);
					val = (byte)stream.ReadByte();
					val.Should().Be(2);
					stream.Dispose();

					writtenData.Length.Should().Be(0, "No data should be written to storage if data are not changed");
				}

				{
					var stream = new RemoteDataStream(contentId, storage);

					stream.Seek(0, SeekOrigin.Begin);
					byte val = 0;
					val = (byte)stream.ReadByte();
					val.Should().Be(1);
					val = (byte)stream.ReadByte();
					val.Should().Be(2);

					// write one more byte to stream
					stream.WriteByte(3);
					stream.Dispose();

					// data should be writte
					writtenData.Length.Should().Be(3, "New data should be written to the storage");
				}
			}
		}
	}
}
