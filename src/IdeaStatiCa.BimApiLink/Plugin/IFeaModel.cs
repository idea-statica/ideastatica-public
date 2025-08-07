using System;
using System.Collections.Generic;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	public interface IFeaModel : IModel
	{
		FeaUserSelection GetUserSelection();
		/// <summary>
		/// Gets all load combinations or load cases in current model in Fea application
		/// </summary>
		/// <returns>Identifiers of load combinations or load cases</returns>
		/// <remarks>More general version of obsolete method <see cref="GetAllCombinations"/> </remarks>		
		IEnumerable<Identifier<IIdeaLoading>> GetAllLoadings();

		/// <summary>
		/// Gets all load combinations in current model in Fea application
		/// </summary>
		/// <returns>Identifiers of load combinations</returns>
		/// <remarks>This method is obsolete now and replaced by <see cref="GetAllLoadings"/> </remarks>
		[Obsolete]
		IEnumerable<Identifier<IIdeaCombiInput>> GetAllCombinations();

		/// <summary>
		/// Get's the user selected nodes and members as input parameters and projects the selection in connected Fea application
		/// </summary>
		/// <param name="nodes">Nodes to be selected</param>
		/// <param name="members">Members to be selected</param>
		void SelectUserSelection(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members);
	}
}
