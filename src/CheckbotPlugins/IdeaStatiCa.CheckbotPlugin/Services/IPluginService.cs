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
		/// Ends current procedure and unblocks Checkbot for the user.
		/// </summary>
		/// <returns></returns>
		Task ProcedureComplete();
	}
}