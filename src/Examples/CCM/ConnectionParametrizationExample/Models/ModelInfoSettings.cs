using System.Collections.Generic;

namespace ConnectionParametrizationExample.Models
{
	public class ModelInfoSettings
	{
		public string IdeaAppLocation { get; set; }
		public string IdeaConFilesLocation { get; set; }
		public List<string> IdeaConFiles { get; set; } = new List<string>();
	}
}
