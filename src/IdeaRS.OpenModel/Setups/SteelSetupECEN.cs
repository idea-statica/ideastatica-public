namespace IdeaRS.OpenModel
{

	/// <summary>
	/// Steel setup ECEN class
	/// </summary>
	public class SteelSetupECEN : SteelSetup
	{
		/// <summary>
		/// GammaM0
		/// </summary>
		public double GammaM0 { get; set; }

		/// <summary>
		/// GammaM1
		/// </summary>
		public double GammaM1 { get; set; }

		/// <summary>
		/// GammaM2
		/// </summary>
		public double GammaM2 { get; set; }

		/// <summary>
		/// GammaMfi in fire design
		/// </summary>
		public double GammaMfi { get; set; }

		/// <summary>
		/// GammaMu
		/// </summary>
		public double GammaMu { get; set; }
	}
}
