using IdeaRS.OpenModel;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
    /// <summary>
    /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        OriginSettings GetOriginSettings();
    }
}