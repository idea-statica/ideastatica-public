using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel;

namespace ConnectionIomGenerator.UI.Models
{
	public class IomGeneratorModel
	{
		public OpenModelContainer? IomContainer { get; set; }

		public ConnectionInput? ConnectionInput { get; set; }

		public LoadingInput? Loading { get; set; }
	}
}
