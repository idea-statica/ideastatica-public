namespace IdeaRS.OpenModel.Detail
{
	public interface ISynchronization
	{
		/// <summary>
		/// Synchronization ID for element tracking during OpenModel to Detail updates.
		/// </summary>
		int SyncId { get; set; }
	}
}
