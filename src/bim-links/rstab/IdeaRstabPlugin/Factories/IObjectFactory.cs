using Dlubal.RSTAB8;
using IdeaStatiCa.BimApi;

namespace IdeaRstabPlugin.Factories
{
	public interface IObjectFactory
	{
		/// <summary>
		/// Creates an instance of <see cref="IIdeaMember1D"/> from member material with given number.
		/// </summary>
		/// <param name="memberNo">Number of RSTAB member</param>
		/// <returns>BimApi material</returns>
		IIdeaMember1D GetMember(int memberNo);

		/// <summary>
		/// Creates an instance of <see cref="IIdeaCrossSection"/> from RSTAB cross-section with given number.
		/// </summary>
		/// <param name="memberNo">Number of RSTAB cross-section</param>
		/// <returns>BimApi cross-section</returns>
		IIdeaCrossSection GetCrossSection(int cssNo);

		/// <summary>
		/// Creates an instance of <see cref="IIdeaCrossSection"/> from RSTAB node with given number.
		/// </summary>
		/// <param name="memberNo">Number of RSTAB Node</param>
		/// <returns>BimApi node</returns>
		IIdeaNode GetNode(int nodeNo);

		/// <summary>
		/// Creates an instance of <see cref="IIdeaMaterial"/> from RSTAB material with given number.
		/// </summary>
		/// <param name="memberNo">Number of RSTAB material</param>
		/// <returns>BimApi material</returns>
		IIdeaMaterial GetMaterial(int materialNo);

		/// <summary>
		/// Creates an instance of <see cref="IIdeaLoadGroup"/>.
		/// </summary>
		/// <param name="loadgroupNo">Number of loadgrou</param>
		/// <returns>BimApi LoadGroup</returns>
		IIdeaLoadGroup GetLoadGroup(int lgNo);

		///// <summary>
		///// Creates an instance of <see cref="IIdeaLoadCase"/>.
		///// </summary>
		///// <param name="lcNo">Id of loadcase</param>
		///// <returns>BimApi LoadCase</returns>
		//IIdeaLoadCase GetLoadCombiInput(int lgNo);

		/// <summary>
		/// Creates an instance of <see cref="IIdeaLoadCase"/>.
		/// </summary>
		/// <param name="lcNo">Id of loadcase</param>
		/// <returns>BimApi LoadCase</returns>
		IIdeaLoadCase GetLoadCase(int lcNo);

		/// <summary>
		/// Creates an instance of <see cref="IIdeaCombiInput"/>.
		/// </summary>
		/// <param name="combiId">Id of combiInput</param>
		/// <returns>BimApi CombiInput</returns>
		IIdeaCombiInput GetResultCombiInput(int combiId);

		IIdeaLoading GetLoading(Loading loading);
	}
}