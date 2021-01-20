namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material reinforcement CAN
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.American.MatReinforcementCAN,CI.Material", "CI.StructModel.Libraries.Material.IMatReinforcement,CI.BasicTypes", typeof(MatReinforcement))]
	public class MatReinforcementCAN : MatReinforcement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatReinforcementCAN()
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
