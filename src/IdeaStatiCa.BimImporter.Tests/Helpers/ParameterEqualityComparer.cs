using IdeaRS.OpenModel.CrossSection;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Tests.Helpers
{
	internal class ParameterEqualityComparer : IEqualityComparer<Parameter>
	{
		public bool Equals(Parameter x, Parameter y)
		{
			if (x.Name != y.Name)
			{
				return false;
			}

			return GetValue(x)?.Equals(GetValue(y)) ?? false;
		}

		public int GetHashCode(Parameter obj)
		{
			int hashCode = 68643797;
			hashCode = hashCode * 94162379 + obj.Name.GetHashCode();
			hashCode = hashCode * 94162379 + (GetValue(obj)?.GetHashCode() ?? 0);
			return hashCode;
		}

		private object GetValue(Parameter parameter)
		{
			switch (parameter)
			{
				case ParameterString pStr:
					return pStr.Value;

				case ParameterInt pInt:
					return pInt.Value;

				case ParameterBool pBool:
					return pBool.Value;

				case ParameterDouble pDouble:
					return pDouble.Value;

				case ParameterReferenceElement pRefElm:
					return pRefElm.Value;
			}

			return null;
		}
	}
}