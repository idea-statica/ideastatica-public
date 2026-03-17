using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	public interface ISynchronization
	{
		/// <summary>
		/// Synchronization ID for element tracking during OpenModel to Detail updates.
		/// </summary>
		System.Int32 SyncId { get; set; }
	}
}
