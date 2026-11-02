using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace IdeaStatiCa.Public
{
	/// <summary>
	/// Implementation of <see cref="IBlobStorage"/> where a <see cref="ZipArchive"/> is the storage of data
	/// </summary>
	public class BlobStorageInArchive : IBlobStorage, IDisposable
	{
		private bool disposedValue;

		Stream Storage { get; set; }
		ZipArchive Archive { get; set; }

		bool IsMyStorage { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		public BlobStorageInArchive()
		{
			IsMyStorage = true;
			Storage = new MemoryStream();
			Archive = new ZipArchive(Storage, ZipArchiveMode.Update, true);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">The stream which includes zip archive which will be used to initioalize BlobStorage</param>
		public BlobStorageInArchive(Stream source)
		{
			IsMyStorage = false;
			source.Seek(0, SeekOrigin.Begin);
			Storage = source;
			Archive = new ZipArchive(Storage, ZipArchiveMode.Update, true);
		}

		/// <inheritdoc cref="IBlobStorage.Delete(string)"/>
		public void Delete(string contentId)
		{
			var entryToDelet = Archive.GetEntry(contentId);
			entryToDelet.Delete();
		}

		/// <inheritdoc cref="IBlobStorage.Exist(string)"/>
		public bool Exist(string contentId)
		{
			var entries = Archive.Entries;
			var entry = entries.FirstOrDefault(e => contentId.Equals(e.FullName, StringComparison.InvariantCultureIgnoreCase));
			return entry != null;
		}


		/// <summary>
		/// Not needed for zip archive storage
		/// </summary>
		/// <param name="basePath"></param>
		public void Init(string basePath)
		{
		}

		/// <inheritdoc cref="IBlobStorage.Read(string)"/>
		public Stream Read(string contentId)
		{
			var entry = Archive.GetEntry(contentId);
			using (var source = entry.Open())
			{
				var res = new MemoryStream();
				source.CopyTo(res);
				res.Seek(0, SeekOrigin.Begin);
				return res;
			}
		}

		/// <inheritdoc cref="IBlobStorage.Write(Stream, string)"/>
		public void Write(Stream content, string contentId)
		{
			content.Seek(0, SeekOrigin.Begin);
			ZipArchiveEntry entry = Archive.CreateEntry(contentId);
			using (var destination = entry.Open())
			{
				content.CopyTo(destination);
			}
		}

		/// <inheritdoc cref="IBlobStorage.GetEntries"/>
		public IReadOnlyCollection<string> GetEntries()
		{
			var pathsList = Archive.Entries.Select(entry => entry.FullName).ToList();
			return pathsList;
		}

		/// <summary>
		/// Copy all the entries in the storage to a directory on the file system.
		/// </summary>
		/// <param name="destinationDirectory">Destination directory</param>
		public void CopyToDirectory(string destinationDirectory)
		{
			Archive.ExtractToDirectory(destinationDirectory);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Archive.Dispose();
					if (IsMyStorage)
					{
						Storage.Dispose();
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~BlobStorageInMemory()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		void IDisposable.Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
