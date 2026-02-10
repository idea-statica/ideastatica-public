using System.Threading.Tasks;

namespace IdeaStatiCa.Public
{
	/// <summary>
	/// Responsible for handling notification of changed in currently opened item
	/// </summary>
	public interface IParentAppNotificationHandler
	{
		/// <summary>
		/// Notifies parent application, that currently opened item changed
		/// </summary>
		/// <param name="itemKey">identified of currently opened item</param>
		Task NotifyItemChangedAsync(int itemKey);
	}
}
