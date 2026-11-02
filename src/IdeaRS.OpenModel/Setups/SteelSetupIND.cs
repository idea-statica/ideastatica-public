namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Steel setup IND class
	/// </summary>
	public class SteelSetupIND : SteelSetup
	{
        /// <summary>
        /// Partial safety factor for resistance governed by yielding
        /// </summary>
        public double GammaM0 { get; set; }

        /// <summary>
        /// Partial safety factor for resistance governed by ultimate stress
        /// </summary>
        public double GammaM1 { get; set; }

        /// <summary>
        /// Partial safety factor for bolts - bearing type
        /// </summary>
        public double GammaMb { get; set; }

        /// <summary>
        /// Partial safety factor for bolts - friction type
        /// </summary>
        public double GammaMf { get; set; }

        /// <summary>
        /// Partial safety factor for welds
        /// </summary>
        public double GammaMw { get; set; }

		/// <summary>
		/// Friction Coefficient Pbolt Default
		/// </summary>
		public override double FrictionCoefficientPboltDefault()
		{
			return 0.35;
		}
	}
}