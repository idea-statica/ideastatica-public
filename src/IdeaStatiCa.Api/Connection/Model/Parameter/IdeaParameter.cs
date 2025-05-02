using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class IdeaParameterUpdate
	{
		public string Key { get; set; }

		public string Expression { get; set; }
	}

	public class IdeaParameter
	{
		public string Key { get; set; }

		public string Expression { get; set; }

		public dynamic Value { get; set; }

		public string Unit { get; set; }

		public string ParameterType { get; set; }

		public string Description { get; set; }

		public bool? IsVisible { get; set; }

		public IdeaParameterValidation ParameterValidation { get; set; }
	}

	public class IdeaParameterValidation
	{
		public string ValidationStatus { get; set; }

		public string ValidationExpression { get; set; }

		public bool ValidationExpressionEvaluated { get; set; }

		public string LowerBound { get; set; }

		public double LowerBoundEvaluated { get; set; }

		public string UpperBound { get; set; }

		public double UpperBoundEvaluated { get; set; }

		public string Message { get; set; }
	}

	public class IdeaParameterValidationResponse
	{
		public string Key { get; set; }

		public string Message { get; set; }

		public string ValidationStatus { get; set; }

		public IdeaParameterValidationResponse(IdeaParameter failedParameter)
		{
			Key = failedParameter.Key;
			Message = failedParameter.ParameterValidation.Message;
			ValidationStatus = failedParameter.ParameterValidation.ValidationStatus;
		}
	}

	public class ParameterUpdateResponse
	{
		public bool SetToModel { get; set; } = true;

		public List<IdeaParameterValidationResponse> FailedValidations { get; set; } = new List<IdeaParameterValidationResponse>();

		public List<IdeaParameter> Parameters { get; set; } = new List<IdeaParameter>();
	}
}
