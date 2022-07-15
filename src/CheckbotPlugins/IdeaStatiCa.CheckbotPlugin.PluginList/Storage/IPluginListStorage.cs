using IdeaStatiCa.CheckbotPlugin.Common;
using System.IO;

namespace IdeaStatiCa.PluginSystem.PluginList.Storage
{
	public interface IStorage
	{
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		Maybe<Stream> GetReadStream();

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		Stream GetWriteStream();
	}
}