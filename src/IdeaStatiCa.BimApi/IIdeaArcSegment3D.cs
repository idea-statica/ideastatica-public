namespace IdeaStatiCa.BimApi
{
    /// <summary>
    /// Represents the line segment of the element.
    /// <para>
    /// Line segment is defined by three points returned by the <see cref="IIdeaSegment3D.StartNode"/>, <see cref="IIdeaSegment3D.EndNode"/> of the base class <see cref="IIdeaSegment3D"/>
    /// and <see cref="ArcPoint"/>.
    /// </para>
    /// </summary>
    public interface IIdeaArcSegment3D : IIdeaSegment3D
    {
        /// <summary>
        /// Third point that defines the arc. Must not be equal to <see cref="IIdeaSegment3D.StartNode"/> or <see cref="IIdeaSegment3D.EndNode"/>
        /// </summary>
        IIdeaNode ArcPoint { get; }
    }
}