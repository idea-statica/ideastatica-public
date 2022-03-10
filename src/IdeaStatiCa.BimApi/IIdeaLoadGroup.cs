using IdeaRS.OpenModel.Loading;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaLoadGroup : IIdeaObject
	{
		/// <summary>
		/// Relation
		/// </summary>
		Relation Relation { get; }

		/// <summary>
		/// Group type
		/// </summary>
		LoadGroupType GroupType { get; }

		/// <summary>
		/// γ <sub>Q</sub>
		/// </summary>
		double GammaQ { get; }

		/// <summary>
		/// ζ
		/// </summary>
		double Dzeta { get; }

		/// <summary>
		/// γ <sub>G,Inf</sub>
		/// </summary>
		double GammaGInf { get; }

		/// <summary>
		/// γ <sub>G,Sup</sub>
		/// </summary>
		double GammaGSup { get; }

		/// <summary>
		/// ψ <sub>0</sub>
		/// </summary>
		double Psi0 { get; }

		/// <summary>
		/// ψ <sub>1</sub>
		/// </summary>
		double Psi1 { get; }

		/// <summary>
		/// ψ <sub>2</sub>
		/// </summary>
		double Psi2 { get; }
	}
}