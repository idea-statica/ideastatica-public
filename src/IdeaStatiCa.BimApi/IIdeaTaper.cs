using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaTaper : IIdeaObject
	{
		/// <summary>
		/// List of <see cref="Span"/>.
		/// </summary>
		IEnumerable<IIdeaSpan> Spans { get; }
	}
}