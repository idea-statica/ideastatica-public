using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi.Api
{
	internal static class ProducedFile
	{
		internal static async Task SaveAsync(byte[] content, string filePath)
		{
			using (var fileStream = File.Create(filePath))
			{
				await fileStream.WriteAsync(content, 0, content.Length);
			}
		}
	}
}
