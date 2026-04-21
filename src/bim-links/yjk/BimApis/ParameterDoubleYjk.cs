using IdeaRS.OpenModel.CrossSection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yjk.BimApis
{
	public class ParameterDoubleYjk : ParameterDouble
	{
		//Override Equals
		public override bool Equals(object obj) => Equals(obj as ParameterDoubleYjk);

		public bool Equals(ParameterDoubleYjk other)
		{
			return other != null && Name == other.Name && Value == other.Value;
		}

		public override int GetHashCode() => HashCode.Combine(Name, Value);

		public static bool operator ==(ParameterDoubleYjk left, ParameterDoubleYjk right) => Equals(left, right);
		public static bool operator !=(ParameterDoubleYjk left, ParameterDoubleYjk right) => !Equals(left, right);
	}
}
