namespace IdeaRS.OpenModel.Concrete.Load
{
	/// <summary>
	/// Tendon component loading
	/// </summary>
	public class TendonComponentLoading
	{
		/// <summary>
		/// Tendon component Id
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Loading
		/// </summary>
		public TendonLoading Loading { get; set; }
	}
}