using System;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Steel setup RUS class
	/// </summary>
	public class SteelSetupRUS : SteelSetup
	{
		/// <summary>
		/// Safety factor Structural_Fi
		/// </summary>
		public double Structural_Fi { get; set; }

		/// <summary>
		/// Safety factor Bolt_Fib
		/// </summary>
		public double Bolt_Fib { get; set; }

		/// <summary>
		/// Safety factor preloaded bolt
		/// </summary>
		public double Bolt_Fip { get; set; }

		/// <summary>
		/// Safety factor Weld_Fiw
		/// </summary>
		public double Weld_Fiw { get; set; }

		/// <summary>
		/// Safety factor Anchor_Fiar
		/// </summary>
		public double Anchor_Fiar { get; set; }

		/// <summary>
		/// Concrete bearing factor
		/// </summary>
		public double CrtBearing { get; set; }

		/// <summary>
		/// WeldingType
		/// </summary>
		[Obsolete("Moved to the welding electrodes properties")]
		public WeldingTypeSNIP WeldingTypeSNIP { get; set; }

		/// <summary>
		/// WeldingType
		/// </summary>
		public PreloadedBoltsLoadType PreloadedBoltsLoadType { get; set; }

		/// <summary>
		/// Friction Coefficient Pbolt Default
		/// </summary>
		public override double FrictionCoefficientPboltDefault()
		{
			return 0.35;
		}
	}

	/// <summary>
	/// Welding Type
	/// </summary>
	public enum WeldingTypeSNIP
	{
		/// <summary>
		/// Manual welding
		/// </summary>
		Manual,

		/// <summary>
		/// Manual welding using rod solid cross-section with diameter less than 1.4mm
		/// </summary>
		ManualSmallRodDiam,

		/// <summary>
		/// Automatic and machine welding
		/// </summary>
		AutomaticAndMachine,

		/// <summary>
		/// Automatic welding
		/// </summary>
		Automatic
	}

	/// <summary>
	/// Preloaded Bolts Load Type SNIP
	/// </summary>
	public enum PreloadedBoltsLoadType
	{
		/// <summary>
		/// Static
		/// </summary>
		Static,
		/// <summary>
		/// Dynamic
		/// </summary>
		Dynamic,
	}
}
