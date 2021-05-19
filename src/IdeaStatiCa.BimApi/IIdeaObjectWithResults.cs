using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaObjectWithResults: IIdeaObject
	{
		IEnumerable<IIdeaResult> GetResults();
	}
}