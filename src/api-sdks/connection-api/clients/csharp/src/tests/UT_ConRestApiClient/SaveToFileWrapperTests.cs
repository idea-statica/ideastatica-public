using System.Net;
using IdeaStatiCa.ConnectionApi.Api;
using IdeaStatiCa.ConnectionApi.Client;

namespace UT_ConRestApiClient
{
	/// <summary>
	/// Every file-producing operation has a wrapper that saves the produced file: it asks the service
	/// for the raw bytes and writes them, unchanged, to the path the caller names.
	/// </summary>
	[TestFixture]
	public class SaveToFileWrapperTests
	{
		private static readonly Guid ProjectId = Guid.Parse("6f9619ff-8b86-d011-b42d-00c04fc964ff");
		private static readonly byte[] ZipContent = { 0x50, 0x4B, 0x03, 0x04, 0x0A, 0x00, 0x00, 0x00 };
		private static readonly byte[] DwgContent = { 0x41, 0x43, 0x31, 0x30, 0x32, 0x37, 0x00, 0x00 };

		[Test]
		public async Task SaveReportHtmlZipAsync_WritesTheZipTheServiceReturns()
		{
			var service = new RecordingAsynchronousClient(ZipContent);
			var report = new ReportApiExt(new ApiClient(), service, new Configuration());
			string filePath = TemporaryFilePath(".zip");
			try
			{
				await report.SaveReportHtmlZipAsync(ProjectId, 7, filePath);

				Assert.That(service.Options, Is.Not.Null);
				Assert.Multiple(() =>
				{
					Assert.That(service.RequestedPath, Does.EndWith("/projects/{projectId}/connections/{connectionId}/reports/htmlZip"));
					Assert.That(service.Options!.PathParameters["projectId"], Is.EqualTo(ProjectId.ToString()));
					Assert.That(service.Options.PathParameters["connectionId"], Is.EqualTo("7"));
					Assert.That(service.Options.HeaderParameters["Accept"], Is.EqualTo(new[] { "application/octet-stream" }),
						"the raw bytes are asked for, not a JSON rendering of them");
					Assert.That(File.ReadAllBytes(filePath), Is.EqualTo(ZipContent));
				});
			}
			finally
			{
				File.Delete(filePath);
			}
		}

		[Test]
		public async Task ExportDwgFileAsync_WritesTheDrawingTheServiceReturns()
		{
			var service = new RecordingAsynchronousClient(DwgContent);
			var export = new ExportApiExt(new ApiClient(), service, new Configuration());
			string filePath = TemporaryFilePath(".dwg");
			try
			{
				await export.ExportDwgFileAsync(ProjectId, 7, filePath);

				Assert.That(service.Options, Is.Not.Null);
				Assert.Multiple(() =>
				{
					Assert.That(service.RequestedPath, Does.EndWith("/projects/{projectId}/connections/{connectionId}/export-dwg"));
					Assert.That(service.Options!.PathParameters["projectId"], Is.EqualTo(ProjectId.ToString()));
					Assert.That(service.Options.PathParameters["connectionId"], Is.EqualTo("7"));
					Assert.That(service.Options.HeaderParameters["Accept"], Is.EqualTo(new[] { "application/octet-stream" }),
						"the raw bytes are asked for, not a JSON rendering of them");
					Assert.That(File.ReadAllBytes(filePath), Is.EqualTo(DwgContent));
				});
			}
			finally
			{
				File.Delete(filePath);
			}
		}

		private static string TemporaryFilePath(string extension)
		{
			return Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");
		}

		/// <summary>
		/// Stands in for the service: answers every GET with the same produced file and remembers what
		/// was asked for.
		/// </summary>
		private sealed class RecordingAsynchronousClient : IAsynchronousClient
		{
			private readonly byte[] content;

			public RecordingAsynchronousClient(byte[] content)
			{
				this.content = content;
			}

			public string? RequestedPath { get; private set; }

			public RequestOptions? Options { get; private set; }

			public Task<ApiResponse<T>> GetAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
			{
				RequestedPath = path;
				Options = options;
				return Task.FromResult(new ApiResponse<T>(HttpStatusCode.OK, (T)(object)content));
			}

			public Task<ApiResponse<T>> PostAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
			{
				throw new NotSupportedException();
			}

			public Task<ApiResponse<T>> PutAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
			{
				throw new NotSupportedException();
			}

			public Task<ApiResponse<T>> DeleteAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
			{
				throw new NotSupportedException();
			}

			public Task<ApiResponse<T>> HeadAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
			{
				throw new NotSupportedException();
			}

			public Task<ApiResponse<T>> OptionsAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
			{
				throw new NotSupportedException();
			}

			public Task<ApiResponse<T>> PatchAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
			{
				throw new NotSupportedException();
			}
		}
	}
}
