using System.Threading.Tasks;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public interface IPluginService
	{
		/// <summary>
		/// Announces a new version of the plugin to the user.
		/// </summary>
		/// <param name="newVersion"></param>
		/// <returns></returns>
		Task NewVersion(string newVersion);

		/// <summary>
		/// Ends current operation and discards any changes by the plugin.
		/// </summary>
		/// <returns></returns>
		Task OperationDiscard();

		/// <summary>
		/// Ends current operation and applies all changes from the plugin.
		/// </summary>
		/// <returns></returns>
		Task OperationCompleted();
	}
}