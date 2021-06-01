namespace IdeaStatiCa.BimApi
{
	public interface IIdeaLoadGroup : IIdeaObject
	{
		/// <summary>
		/// Relation
		/// </summary>
		IdeaRS.OpenModel.Loading.Relation Relation { get; set; }

		/// <summary>
		/// Group type
		/// </summary>
		IdeaRS.OpenModel.Loading.LoadGroupType GroupType { get; set; }

		/// <summary>
		/// γ <sub>Q</sub>
		/// </summary>
		System.Double GammaQ { get; }

		/// <summary>
		/// ζ
		/// </summary>
		System.Double Dzeta { get; }

		/// <summary>
		/// γ <sub>G,Inf</sub>
		/// </summary>
		System.Double GammaGInf { get; }

		/// <summary>
		/// γ <sub>G,Sup</sub>
		/// </summary>
		System.Double GammaGSup { get; }


		/// <summary>
		/// ψ <sub>0</sub>
		/// </summary>
		System.Double Psi0 { get; }

		/// <summary>
		/// ψ <sub>1</sub>
		/// </summary>
		System.Double Psi1 { get; }

		/// <summary>
		/// ψ <sub>2</sub>
		/// </summary>
		System.Double Psi2 { get; }

	}
}