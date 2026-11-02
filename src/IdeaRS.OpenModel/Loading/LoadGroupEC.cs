namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Load group for EC
	/// </summary>
	/// <example> 
	/// This sample shows how to create a load group.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// //Load group - needed for Load case
	/// LoadGroupEC loadGroup = new LoadGroupEC();
	/// loadGroup.Name = "LG1";
	/// loadGroup.GammaQ = 1.5;
	/// loadGroup.Psi0 = 0.7;
	/// loadGroup.Psi1 = 0.5;
	/// loadGroup.Psi2 = 0.3;
	/// loadGroup.GammaGInf = 1.0;
	/// loadGroup.GammaGSup = 1.35;
	/// loadGroup.Dzeta = 0.85;
	/// openModel.AddObject(loadGroup);
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Loading.LoadGroupEC,CI.Loading", "CI.StructModel.Loading.ILoadGroup,CI.BasicTypes", typeof(LoadGroup))]
	public class LoadGroupEC : LoadGroup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LoadGroupEC()
		{
		}

		/// <summary>
		/// ψ <sub>0</sub>
		/// </summary>
		public System.Double Psi0 { get; set; }

		/// <summary>
		/// ψ <sub>1</sub>
		/// </summary>
		public System.Double Psi1 { get; set; }

		/// <summary>
		/// ψ <sub>2</sub>
		/// </summary>
		public System.Double Psi2 { get; set; }
	}
}