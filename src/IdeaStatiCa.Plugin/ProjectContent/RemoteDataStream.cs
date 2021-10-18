using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.ProjectContent
{
	/// <summary>
	/// MemoryStream which data are copied to <see cref="IProjectContentStorage"/> when method in Flush or Close is called and local data are changed
	/// </summary>
	public class RemoteDataStream : MemoryStream
	{
		IProjectContentStorage Storage { get; set; }
		public string ContentId { get; private set; }

		private bool IsDirty { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="contentId">Content ID</param>
		/// <param name="storage">Storage</param>
		public RemoteDataStream(string contentId, IProjectContentStorage storage)
		{
			Debug.Assert(!string.IsNullOrEmpty(contentId));
			Debug.Assert(storage != null);
			ContentId = contentId;
			Storage = storage;
			Storage.ReadData(ContentId, this);
			IsDirty = false;
		}

		/// <summary>
		/// Write data to storage if data are changed
		/// </summary>
		public override void Flush()
		{
			base.Flush();
			WriteToStorage();
		}

		/// <summary>
		/// Write data to stream and set <see cref="IsDirty" flag/>
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			Modify();
			base.Write(buffer, offset, count);
		}

		/// <summary>
		/// Write data to stream and set <see cref="IsDirty" flag/>
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			Modify();
			return base.WriteAsync(buffer, offset, count, cancellationToken);
		}

		/// <summary>
		/// Write data to stream and set <see cref="IsDirty" flag/>
		/// </summary>
		/// <param name="value"></param>
		public override void WriteByte(byte value)
		{
			Modify();
			base.WriteByte(value);
		}

		/// <summary>
		/// Write data to stream and set <see cref="IsDirty" flag/>
		/// </summary>
		/// <param name="asyncResult"></param>
		public override void EndWrite(IAsyncResult asyncResult)
		{
			Modify();
			base.EndWrite(asyncResult);
		}

		/// <summary>
		///  Write data to storage if data are changed
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override async Task FlushAsync(CancellationToken cancellationToken)
		{
			await Task.Run(() => { Storage.WriteData(ContentId, this); });
			await base.FlushAsync(cancellationToken);
		}
		
		/// <summary>
		/// Write data to storage
		/// </summary>
		private void WriteToStorage()
		{
			if (IsDirty)
			{
				this.Seek(0, SeekOrigin.Begin);
				Storage.WriteData(ContentId, this);
				IsDirty = false;
			}
		}

		/// <summary>
		/// Set Is dirty flag
		/// </summary>
		private void Modify()
		{
			IsDirty = true;
		}

		/// <summary>
		///  Write data to storage if data are changed
		/// </summary>
		public override void Close()
		{
			WriteToStorage();
			base.Close();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
