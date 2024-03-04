using System.Collections.Generic;

namespace IdeaStatiCa.IntermediateModel.IRModel
{
	// Model containing intermediate objects
	public class SModel
	{
		public string Version { get; set; }
		public ISIntermediate RootItem { get; set; }

		public ISIntermediate ModelDeclaration { get; set; }

		public Dictionary<string, SAttribute> RootNameSpaces { get; set; }

		public SModel()
		{
			RootNameSpaces = new Dictionary<string, SAttribute>();
		}
	}
}
