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
		public string Key { get; set; }
		public string Message { get; set; }
		public string ValidationStatus { get; set; }

		public IdeaParameterValidationResponse()
		{

		}

		public IdeaParameterValidationResponse(IdeaParameter parameter)
		{
			Key = parameter.Key;
			Message = parameter?.ParameterValidation?.Message;
			ValidationStatus = parameter.ValidationStatus;
		}
	}
}
