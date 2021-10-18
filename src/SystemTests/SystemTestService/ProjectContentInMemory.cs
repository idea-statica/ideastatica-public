using IdeaStatiCa.Plugin.ProjectContent;
using IdeaStatiCa.Public;
using System;
using System.Collections.Generic;
using System.IO;

namespace SystemTestService
{
	public class ProjectContentInMemory : IProjectContent, IProjectContentStorage
	{
		private Dictionary<string, MemoryStream> MemStreamDict { get; set; }

		public ProjectContentInMemory()
		{
			MemStreamDict = new Dictionary<string, MemoryStream>();
		}

		public void CopyContent(IProjectContent sourceContent)
		{
			throw new NotImplementedException();
		}

		public Stream Create(string contentId)
		{
			var newMemStream = new MemoryStream();
			MemStreamDict.Add(contentId, newMemStream);

			var res = new RemoteDataStream(contentId, this);
			return res;
		}

		public void Delete(string contentId)
		{
			var removedMemStream = MemStreamDict[contentId];
			MemStreamDict.Remove(contentId);
			removedMemStream.Dispose();
		}

		public bool Exist(string contentId)
		{
			return MemStreamDict.ContainsKey(contentId);
		}

		public Stream Get(string contentId)
		{
			var res = new RemoteDataStream(contentId, this);
			return res;
		}

		public List<ProjectDataItem> GetContent()
		{
			throw new NotImplementedException();
		}

		public int ReadData(string contentId, Stream outputStream)
		{
			var localStream = MemStreamDict[contentId];
			localStream.Seek(0, SeekOrigin.Begin);
			localStream.CopyTo(outputStream);
			localStream.Seek(0, SeekOrigin.Begin);
			outputStream.Seek(0, SeekOrigin.Begin);
			return 1;
		}

		public int WriteData(string contentId, Stream inputStream)
		{
			var localStream = MemStreamDict[contentId];

			inputStream.Seek(0, SeekOrigin.Begin);
			localStream.Seek(0, SeekOrigin.Begin);
			inputStream.CopyTo(localStream);

			return 1;
		}
	}
}
