namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Type of exposure class
	/// </summary>
	public enum ExposureClassEc2
	{
		/// <summary>
		/// No risk of corrosion or attack - very dry
		/// </summary>
		X0 = 0,

		/// <summary>
		/// Corrosion induced by carbonation - dry or permanently wet
		/// </summary>
		XC1 = 1,

		/// <summary>
		/// Corrosion induced by carbonation - wet, rarely dry
		/// </summary>
		XC2,

		/// <summary>
		/// Corrosion induced by carbonation - moderate humidity
		/// </summary>
		XC3,

		/// <summary>
		/// Corrosion induced by carbonation - cyclic wet and dry
		/// </summary>
		XC4,

		/// <summary>
		/// Corrosion induced by chlorides - moderate humidity
		/// </summary>
		XD1,

		/// <summary>
		/// Corrosion induced by chlorides - wet, rarely dry
		/// </summary>
		XD2,

		/// <summary>
		/// Corrosion induced by chlorides - cyclic wet and dry
		/// </summary>
		XD3,

		/// <summary>
		/// Corrosion induced by chlorides from sea water - exposed to airborne salt but not in direct contact with sea water
		/// </summary>
		XS1,

		/// <summary>
		/// Corrosion induced by chlorides from sea water - permanently submerged
		/// </summary>
		XS2,

		/// <summary>
		/// Corrosion induced by chlorides from sea water - tidal, splash and spray zones
		/// </summary>
		XS3,

		/// <summary>
		/// Freeze/Thaw Attack - moderate water saturation, without de-icing agent
		/// </summary>
		XF1,

		/// <summary>
		/// Freeze/Thaw Attack - moderate water saturation, with de-icing agent
		/// </summary>
		XF2,

		/// <summary>
		/// Freeze/Thaw Attack - high water saturation, without de-icing agents
		/// </summary>
		XF3,

		/// <summary>
		/// Freeze/Thaw Attack - high water saturation with de-icing agents or sea water
		/// </summary>
		XF4,

		/// <summary>
		/// Chemical attack - slightly aggressive chemical environment according to EN 206-1, Table 2
		/// </summary>
		XA1,

		/// <summary>
		/// Chemical attack - moderately aggressive chemical environment according to EN 206-1, Table 2
		/// </summary>
		XA2,

		/// <summary>
		/// Chemical attack - highly aggressive chemical environment according to EN 206-1, Table 2
		/// </summary>
		XA3,
	}

	/// <summary>
	/// Exposure Classes Ec2
	/// </summary>
	public class ExposureClassesDataEc2
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ExposureClassesDataEc2()
		{
			NoCorrosionCheck = false;
			CarbonationCheck = true;
			Carbonation = ExposureClassEc2.XC3;
			ChloridesCheck = true;
			Chlorides = ExposureClassEc2.XD1;
			ChloridesFromSeaCheck = false;
			ChloridesFromSea = ExposureClassEc2.XS1;
			FreezeAttackCheck = false;
			FreezeAttack = ExposureClassEc2.XF1;
			ChemicalAttackCheck = false;
			ChemicalAttack = ExposureClassEc2.XA1;
		}

		/// <summary>
		/// Check box No corrosion
		/// </summary>
		public bool NoCorrosionCheck { get; set; }

		/// <summary>
		/// Check box Carbonation
		/// </summary>
		public bool CarbonationCheck { get; set; }

		/// <summary>
		/// Combo box Carbonation
		/// </summary>
		public ExposureClassEc2 Carbonation { get; set; }

		/// <summary>
		/// Check box Chlorides
		/// </summary>
		public bool ChloridesCheck { get; set; }

		/// <summary>
		/// Combo box Chlorides
		/// </summary>
		public ExposureClassEc2 Chlorides { get; set; }

		/// <summary>
		/// Check box Chlorides from sea
		/// </summary>
		public bool ChloridesFromSeaCheck { get; set; }

		/// <summary>
		/// Combo box Chlorides from sea
		/// </summary>
		public ExposureClassEc2 ChloridesFromSea { get; set; }

		/// <summary>
		/// Check box Freeze/Thaw Attack
		/// </summary>
		public bool FreezeAttackCheck { get; set; }

		/// <summary>
		/// Combo box Freeze/Thaw Attack
		/// </summary>
		public ExposureClassEc2 FreezeAttack { get; set; }

		/// <summary>
		/// Check box Chemical Attack
		/// </summary>
		public bool ChemicalAttackCheck { get; set; }

		/// <summary>
		/// Combo box Chemical Attack
		/// </summary>
		public ExposureClassEc2 ChemicalAttack { get; set; }
	}
}