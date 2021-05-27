using IdeaRS.OpenModel;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
    /// <summary>
    /// Model for project to import.
    /// </summary>
    public interface IIdeaModel
    {
        /// <summary>
        /// Returns a set of all members in the model.
        /// </summary>
        /// <returns>Set of all members in the model.</returns>
        ISet<IIdeaMember1D> GetMembers();

        /// <summary>
        /// Returns currently selected nodes and/or members. At least one of the sets must be non-empty
        /// unless the user did not select anything.¨Set of <paramref name="members"/> must be a
        /// subset of <see cref="GetMembers"/>.
        /// </summary>
        void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members);

        /// <summary>
        /// Returns information from the original application about the project this class refers to.
        /// </summary>
        /// <returns>Project infomation</returns>
        OriginSettings GetOriginSettings();

		/// <summary>
		/// Returns a set of all load cases in the model.
		/// </summary>
		/// <returns>Set of all load case in the model.</returns>
		ISet<IIdeaLoadCase> ImportLoadCases();

		/// <summary>
		/// Returns a set of all Combination in the model.
		/// </summary>
		/// <returns></returns>
		ISet<IIdeaCombiInput> ImportCombiInput();
	}
}
