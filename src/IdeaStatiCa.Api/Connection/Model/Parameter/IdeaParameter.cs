using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model
{

	/// <summary>
	/// A change to one parameter. Only <see cref="Key"/> is required; every other field that is omitted
	/// keeps its present value.
	/// </summary>
	public class IdeaParameterUpdate
	{
		public string Key { get; set; }

		/// <summary>The new value, or an expression evaluating to it.</summary>
		public string Expression { get; set; }

		/// <summary>The label the application shows for the parameter.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Description { get; set; }

		/// <summary>Whether the application lists the parameter as an input.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsVisible { get; set; }

		/// <summary>
		/// The quantity the value stands for, e.g. <c>LengthComponent</c>, <c>WeldSize</c>, <c>Angle</c> -
		/// the application's <i>Value type</i>, which decides the unit it displays the value in.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ValueType { get; set; }
	}

	public class IdeaParameterValidation
	{
		public string ValidationExpression { get; set; }
		public bool ValidationExpressionEvaluated { get; set; }
		public string LowerBound { get; set; }
		public double LowerBoundEvaluated { get; set; }
		public string UpperBound { get; set; }
		public double UpperBoundEvaluated { get; set; }
		public string ValidationStatus { get; set; }
		public string Message { get; set; }
	}

	public class IdeaParameter
	{
		public string Key { get; set; }

		public string Expression { get; set; }

		public string Default { get; set; }

		public dynamic Value { get; set; }

		/// <summary>
		/// The unit the application displays the value in. Follows from <see cref="ValueType"/>; to change
		/// it, write the value type.
		/// </summary>
		public string Unit { get; set; }

		/// <summary>The quantity the value stands for - the application's <i>Value type</i>.</summary>
		public string ValueType { get; set; }

		public string ParameterType { get; set; }

		public string Description { get; set; }

		public string ValidationStatus { get; set; }

		public bool? IsVisible { get; set; }

		public string LowerBound { get; set; }

		public string UpperBound { get; set; }

		public IdeaParameterValidation ParameterValidation { get; set; }
	}
}
