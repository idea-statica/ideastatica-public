namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Nad strategy concrete
	/// </summary>
	public class NadStrategyConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected NadStrategyConcrete()
		{
			SetupValues = new System.Collections.Generic.List<NadSetupValue>();
		}

		/// <summary>
		/// Setup values
		/// </summary>
		public System.Collections.Generic.List<NadSetupValue> SetupValues { get; set; }
	}
}