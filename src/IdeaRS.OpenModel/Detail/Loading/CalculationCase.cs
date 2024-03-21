using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail.Loading
{
	/// <summary>
	/// load case/combination data
	/// </summary>
	[XmlInclude(typeof(DetailLoadCase))]
	[XmlInclude(typeof(DetailCombination))]
	public abstract class CalculationCase : OpenElementId
	{
		/// <summary>
		/// constructor
		/// </summary>
		public CalculationCase()
		{
			IsActive = true;
		}

		/// <summary>
		/// Gets or set the name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// active load case/combination
		/// </summary>
		public bool IsActive { get; set; }
	}
}
