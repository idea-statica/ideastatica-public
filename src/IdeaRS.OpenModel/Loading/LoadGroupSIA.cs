namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Load group for SIA
	/// </summary>
	[OpenModelClass("CI.StructModel.Loading.LoadGroupSIA,CI.Loading", "CI.StructModel.Loading.ILoadGroup,CI.BasicTypes", typeof(LoadGroup))]
	public class LoadGroupSIA : LoadGroup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LoadGroupSIA()
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