using System.Collections.Generic;

namespace IdeaStatiCa.IntermediateModel.IRModel
{
	// Object intermediate object
	public class SObject : ISIntermediate
	{
		public string TypeName { get; set; }

		public Dictionary<string, ISIntermediate> Properties { get; set; } = new Dictionary<string, ISIntermediate>();

	}
}
