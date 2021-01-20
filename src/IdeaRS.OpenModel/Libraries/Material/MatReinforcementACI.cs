namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material reinforcement ACI
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.American.MatReinforcementACI,CI.Material", "CI.StructModel.Libraries.Material.IMatReinforcement,CI.BasicTypes", typeof(MatReinforcement))]
	public class MatReinforcementACI : MatReinforcement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatReinforcementACI()
		{
		}

		/// <summary>
		/// Characteristic strain of reinforcement
		/// </summary>
		public double Epssu { get; set; }

		/// <summary>
		/// Characteristic yield strength of reinforcement
		/// </summary>
		public double Fy { get; set; }
	}
}
