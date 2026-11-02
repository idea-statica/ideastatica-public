namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Steel setup IND class
	/// </summary>
	public class SteelSetupHKG : SteelSetup
	{
		///// <summary>
		///// Safety factor Structural_Fi
		///// </summary>
		//public double Structural_Fi { get; set; }

		///// <summary>
		///// Safety factor Bolt_Fib
		///// </summary>
		//public double Bolt_Fib { get; set; }

		///// <summary>
		///// Safety factor preloaded bolt
		///// </summary>
		//public double Bolt_Fip { get; set; }

		///// <summary>
		///// Safety factor Weld_Fiw
		///// </summary>
		//public double Weld_Fiw { get; set; }

		///// <summary>
		///// Safety factor Anchor_Fiar
		///// </summary>
		//public double Anchor_Fiar { get; set; }

		///// <summary>
		///// Concrete bearing factor
		///// </summary>
		//public double CrtBearing { get; set; }

		///// <summary>
		///// WeldingType
		///// </summary>
		//public WeldingType WeldingType { get; set; }

		///// <summary>
		///// WeldingType
		///// </summary>
		//public PreloadedBoltsLoadType PreloadedBoltsLoadType { get; set; }

		/// <summary>
		/// Friction Coefficient Pbolt Default
		/// </summary>
		public override double FrictionCoefficientPboltDefault()
		{
			return 0.35;
		}
	}
}