using System.Collections.Generic;

namespace IdeaRstabPlugin.Factories
{
	/// <summary>
	/// Class responsible for mapping RSTAB parameters
	/// </summary>
	internal class RstabParamsMap
	{
		private readonly IList<double> parameters;
		private IDictionary<RstabParamName, int> paramsMap = new Dictionary<RstabParamName, int>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parameters">RSTAB parameters</param>
		public RstabParamsMap(IList<double> parameters)
		{
			this.parameters = parameters;
		}

		/// <summary>
		/// Registers parameter (index, name) value pairs
		/// </summary>
		/// <param name="paramIndex">RSTAB parameter index</param>
		/// <param name="paramName">RSTAB parameter enum name</param>
		public void RegisterParam(int paramIndex, RstabParamName paramName)
		{
			if (paramIndex < parameters.Count)
			{
				if (!paramsMap.TryGetValue(paramName, out _))
				{
					paramsMap.Add(paramName, paramIndex);
				}
			}
		}

		/// <summary>
		/// Gets parameter value by name
		/// </summary>
		/// <param name="paramName">RSTAB parameter enum name</param>
		/// <returns>Parameter value or 0.0 if not found</returns>
		public double Get(RstabParamName paramName)
		{
			if (paramsMap.TryGetValue(paramName, out int paramIndex))
			{
				return parameters[paramIndex];
			}

			return 0.0;
		}

		/// <summary>
		/// Gets parameter value by index
		/// </summary>
		/// <param name="paramIndex">RSTAB parameter index</param>
		/// <returns>Parameter value or 0.0 if not found</returns>
		public double Get(int paramIndex)
		{
			foreach (var item in paramsMap)
			{
				if (item.Value == paramIndex)
				{
					return parameters[paramIndex];
				}
			}

			return 0.0;
		}
	}
}