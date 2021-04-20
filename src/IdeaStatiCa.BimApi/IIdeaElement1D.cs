namespace IdeaStatiCa.BimApi
{
    /// <summary>
    /// An element is a part of a member. Every member is comprised of one or more elements.
    /// It's geometry is defined by a <see cref="Segment"/> that can be either a line or an arc. It must specify
    /// cross-sections at both ends.
    /// </summary>
    public interface IIdeaElement1D : IIdeaObject
    {
        /// <summary>
        /// Cross-section at the start of the element.
        /// </summary>
        IIdeaCrossSection StartCrossSection { get; }

        /// <summary>
        /// Cross-section at the and of the element.
        /// </summary>
        IIdeaCrossSection EndCrossSection { get; }

        /// <summary>
        /// Eccentricity (offset) at the start of the element.
        /// </summary>
        IdeaVector3D EccentricityBegin { get; }

        /// <summary>
        /// Eccentricity (offset) at the end of the element.
        /// </summary>
        IdeaVector3D EccentricityEnd { get; }

        /// <summary>
        /// Rotation of the element around the x-axis of the <see cref="Segment"/>'s LCS. 
        /// </summary>
        double RotationRx { get; }

        /// <summary>
        /// Returns the segment object that should implement either <see cref="IIdeaLineSegment3D"/> or <see cref="IIdeaArcSegment3D"/>. Must not return null.
        /// <para>Hint: use is-operator to determine the segment type.</para>
        /// <para>Segments's <see cref="IIdeaSegment3D.StartNode"/> is connected to <see cref="StartNode"/> and segments's <see cref="IIdeaSegment3D.EndNode"/> is connected to <see cref="EndNode"/></para>
        /// </summary>
        IIdeaSegment3D Segment { get; }
    }
}