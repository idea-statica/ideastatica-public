using IdeaRS.OpenModel.CrossSection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yjk.BimApis
{
	public class CrossSectionParameterYjk : CrossSectionParameter
	{
		//Override Equals
		public override bool Equals(object obj) => Equals(obj as CrossSectionParameterYjk);

		public bool Equals(CrossSectionParameter other) 
		{

			bool parametersAreEqual = true;
			if (Parameters.Count() == other.Parameters.Count) {
				for (int i = 0; i < Parameters.Count; i++)
				{
					//Cast to ParameterDouble
					ParameterDouble parameterDouble = (ParameterDouble)Parameters[i];
					ParameterDouble otherParameterDouble = (ParameterDouble)other.Parameters[i];

					if (parameterDouble.Name != otherParameterDouble.Name || parameterDouble.Value != otherParameterDouble.Value)
					{
						parametersAreEqual = false;
						break;
					}
				}
			}
			else
			{
				parametersAreEqual = false;
			}

			return other != null && CrossSectionType == other.CrossSectionType && parametersAreEqual;
		}

		public override int GetHashCode()
		{
			var hash = new HashCode();

			hash.Add(CrossSectionType);

			if (Parameters != null)
			{
				foreach (var p in Parameters)
				{
					var pd = (ParameterDouble)p;
					hash.Add(pd.Name);
					hash.Add(pd.Value);
				}
			}

			return hash.ToHashCode();
		}

		public static bool operator ==(CrossSectionParameterYjk left, CrossSectionParameterYjk right) => Equals(left, right);
		public static bool operator !=(CrossSectionParameterYjk left, CrossSectionParameterYjk right) => !Equals(left, right);
	}
}
