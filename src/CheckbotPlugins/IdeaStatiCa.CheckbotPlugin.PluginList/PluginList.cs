using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.PluginSystem.PluginList.Descriptors;
using IdeaStatiCa.PluginSystem.PluginList.Json;
using IdeaStatiCa.PluginSystem.PluginList.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdeaStatiCa.PluginSystem.PluginList
{
	public class PluginList
	{
		private readonly JsonPluginList _list;

		public static PluginList Create() => new PluginList(new AppDataStorage());

		public PluginList(IStorage storage)
		{
			Ensure.NotNull(storage, nameof(storage));

			_list = new JsonPluginList(storage);
		}

		public async Task<PluginDescriptor> Get(string name)
		{
			Ensure.NotEmpty(name, nameof(name));

			List<PluginDescriptor> pluginList = await Load();
			return pluginList
				.Where(x => !(x is null))
				.FirstOrDefault(x => x.Name == name);
		}

		public async Task<IReadOnlyList<PluginDescriptor>> GetAll() => await Load();

		public async Task Add(PluginDescriptor pluginDescriptor)
		{
			Ensure.NotNull(pluginDescriptor, nameof(pluginDescriptor));

			List<PluginDescriptor> pluginList = await Load();

			if (pluginList.Any(x => x.Name == pluginDescriptor.Name))
			{
				throw new ArgumentException();
			}

			pluginList.Add(pluginDescriptor);

			await Store(pluginList);
		}

		public async Task<bool> Remove(PluginDescriptor pluginDescriptor)
		{
			Ensure.NotNull(pluginDescriptor, nameof(pluginDescriptor));

			List<PluginDescriptor> pluginList = await Load();
			bool result = pluginList.Remove(pluginDescriptor);
			await Store(pluginList);

			return result;
		}

		public async Task<bool> Remove(string name)
		{
			Ensure.NotEmpty(name, nameof(name));

			List<PluginDescriptor> pluginList = await Load();

			int removed = pluginList.RemoveAll(x => x.Name == name);
			if (removed > 1)
			{
				throw new ArgumentException();
			}

			await Store(pluginList);

			return removed == 1;
		}

		private async Task<List<PluginDescriptor>> Load()
			=> await _list.Read().ConfigureAwait(false);

		private async Task Store(IReadOnlyCollection<PluginDescriptor> pluginDescriptors)
			=> await _list.Write(pluginDescriptors).ConfigureAwait(false);
	}
}