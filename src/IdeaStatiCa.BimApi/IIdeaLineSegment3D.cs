namespace IdeaStatiCa.BimApi
{
    /// <summary>
    /// Represents the line segment of the element.
    /// <para>
    /// Line segment is defined by two points returned by the <see cref="IIdeaSegment3D.StartNode"/> and <see cref="IIdeaSegment3D.EndNode"/> of the base class <see cref="IIdeaSegment3D"/>.
    /// </para>
    /// </summary>
    public interface IIdeaLineSegment3D : IIdeaSegment3D
    {
    }
}