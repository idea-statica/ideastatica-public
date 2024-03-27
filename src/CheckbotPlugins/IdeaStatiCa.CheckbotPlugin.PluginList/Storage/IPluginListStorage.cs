namespace IdeaStatiCa.CheckbotPlugin.PluginList.Storage
{
	public interface IStorage
	{
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		Stream? GetReadStream();

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		Stream GetWriteStream();
	}
}