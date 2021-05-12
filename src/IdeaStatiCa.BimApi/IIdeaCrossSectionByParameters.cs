using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
    /// <summary>
    /// A cross-section specifed by parameters.
    /// </summary>
    public interface IIdeaCrossSectionByParameters : IIdeaCrossSection
    {
        /// <summary>
        /// Material of the cross-section.
        /// </summary>
        IIdeaMaterial Material { get; }

        /// <summary>
        /// Type of the cross-section.
        /// </summary>
        IdeaRS.OpenModel.CrossSection.CrossSectionType Type { get; }

        /// <summary>
        /// Parameters of the cross-section. Parameters depend of the type of the cross-section.
        /// See <see cref="IdeaRS.OpenModel.CrossSection.CrossSectionFactory"/> for more information about parameters.
        /// </summary>
        HashSet<IdeaRS.OpenModel.CrossSection.Parameter> Parameters { get; }
    }
}