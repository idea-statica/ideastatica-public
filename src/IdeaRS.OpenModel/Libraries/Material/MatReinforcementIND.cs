namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material reinforcement IND
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.American.MatReinforcementIND,CI.Material", "CI.StructModel.Libraries.Material.IMatReinforcement,CI.BasicTypes", typeof(MatReinforcement))]
	public class MatReinforcementIND : MatReinforcement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatReinforcementIND()
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
