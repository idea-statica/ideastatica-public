
namespace IdeaStatiCa.BimApi
{
	public interface IIdeaLoadCase : IIdeaObject
	{
		/// <summary>
		/// Load case type
		/// </summary>
		IdeaRS.OpenModel.Loading.LoadCaseType LoadType { get;  }
		/// <summary>
		/// Sub type
		/// </summary>
		IdeaRS.OpenModel.Loading.LoadCaseSubType Type { get; }

		/// <summary>
		/// Variable type
		/// </summary>
		IdeaRS.OpenModel.Loading.VariableType Variable { get; }


		/// <summary>
		/// Load group
		/// </summary>
		IIdeaLoadGroup LoadGroup { get;  }

		/// <summary>
		/// Additional info
		/// </summary>
		System.String Description { get; }
	}

}
