using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ParameterUpdateResponse
	{
		public bool SetToModel { get; set; }

		public List<IdeaParameter> Parameters { get; set; } = new List<IdeaParameter>();

		public List<IdeaParameterValidationResponse> FailedValidations { get; set; } = new List<IdeaParameterValidationResponse>();
	}

	public class IdeaParameterValidationResponse
	{
		public IdeaParameterValidationResponse(IdeaParameter parameter)
		{

		}
	}
}
