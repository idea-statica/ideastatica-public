using System;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Template
{
	[Flags]
	public enum CssType
	{
		/// <summary>
		/// I sections
		/// </summary>
		ISection = 1,

		/// <summary>
		/// square hollow sections
		/// </summary>
		SquareHollow = 2,

		/// <summary>
		/// rectangular hollow sections
		/// </summary>
		RectangularHollow = 4,

		/// <summary>
		/// circular hollow sections
		/// </summary>
		CircularHollow = 8,

		/// <summary>
		/// Angle sections
		/// </summary>
		Angle = 16,

		/// <summary>
		/// Channel
		/// </summary>
		Channel = 32,

		/// <summary>
		/// T
		/// </summary>
		T = 64,

		/// <summary>
		/// wide flat plate
		/// </summary>
		WideFlatPlate = 128,

		/// <summary>
		/// rod
		/// </summary>
		Rod = 256,

		/// <summary>
		/// All shapes
		/// </summary>
		All = 511
	}

	[Flags]
	public enum TableContainerType
	{
		CrossSection = 1,
		Material = 2,
		PlateThickness = 4,
		SteelStrands = 8,
		Fastener = 16,

	}
}
