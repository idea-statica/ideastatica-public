using IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors;
using IdeaStatiCa.CheckbotPlugin.PluginList.Serialization;
using IdeaStatiCa.CheckbotPlugin.PluginList.Storage;
using IdeaStatiCa.CheckbotPlugin.PluginList.Utils;

namespace IdeaStatiCa.CheckbotPlugin.PluginList
{
	/// <summary>
	/// Represents list of all plugins integrated into Checkbot.
	/// </summary>
	public class PluginList
	{
		private readonly JsonPluginListSerializer _list;

		/// <summary>
		/// Creates a default instance of <see cref="PluginList"/>.
		/// </summary>
		/// <returns></returns>
		public static PluginList Create()
		{
			return new PluginList(new AppDataStorage());
		}

		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="storage"></param>
		public PluginList(IStorage storage)
		{
			Ensure.NotNull(storage);

			_list = new JsonPluginListSerializer(storage);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public async Task<PluginDescriptor?> Get(string name)
		{
			Ensure.NotEmpty(name);

			List<PluginDescriptor> pluginList = await Load();
			return pluginList
				.Where(x => x is not null)
				.FirstOrDefault(x => x.Name == name);
		}

		/// <summary>
		/// Returns list of all integrated plugins or an empty list
		/// if there isn't any.
		/// </summary>
		/// <returns>List of <see cref="PluginDescriptor"/></returns>
		public async Task<IReadOnlyList<PluginDescriptor>> GetAll()
		{
			return await Load();
		}

		/// <summary>
		/// Adds a new plugin.
		/// </summary>
		/// <param name="pluginDescriptor">Plugin descriptor</param>
		/// <exception cref="ArgumentException">A plugin with the same name already exists.</exception>
		/// <exception cref="ArgumentNullException">An argument is null.</exception>
		public async Task Add(PluginDescriptor pluginDescriptor)
		{
			Ensure.NotNull(pluginDescriptor);

			List<PluginDescriptor> pluginList = await Load();

			if (pluginList.Exists(x => x.Name == pluginDescriptor.Name))
			{
				throw new ArgumentException("A plugin with the same name already exists.");
			}

			pluginList.Add(pluginDescriptor);

			await Store(pluginList);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="pluginDescriptor"></param>
		/// <returns></returns>
		public async Task<bool> Remove(PluginDescriptor pluginDescriptor)
		{
			Ensure.NotNull(pluginDescriptor);

			List<PluginDescriptor> pluginList = await Load();
			bool result = pluginList.Remove(pluginDescriptor);
			await Store(pluginList);

			return result;
		}

		public async Task<bool> Remove(string name)
		{
			Ensure.NotEmpty(name);

			List<PluginDescriptor> pluginList = await Load();

			int removed = pluginList.RemoveAll(x => x.Name == name);
			if (removed > 1)
			{
				throw new ArgumentException("More than one plugin match the name", nameof(name));
			}

			await Store(pluginList);

			return removed == 1;
		}

		private async Task<List<PluginDescriptor>> Load()
		{
			return await _list.Read().ConfigureAwait(false);
		}

		private async Task Store(IReadOnlyCollection<PluginDescriptor> pluginDescriptors)
		{
			await _list.Write(pluginDescriptors).ConfigureAwait(false);
		}
	}
}