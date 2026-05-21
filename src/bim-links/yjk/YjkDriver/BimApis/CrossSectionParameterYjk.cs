using IdeaRS.OpenModel.CrossSection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
					if (Parameters[i].GetType() != other.Parameters[i].GetType())
					{
						parametersAreEqual = false;
						break;
					}

					if (!ParametersEqual(Parameters[i], other.Parameters[i]))
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

			foreach (var param in Parameters)
			{
				AddParameterToHash(ref hash, param);
			}

			return hash.ToHashCode();
		}

		private static bool ParametersEqual(Parameter a, Parameter b)
		{
			if (a.GetType() != b.GetType()) return false;
			if (a.Name != b.Name) return false;
			dynamic da = a, db = b;
			return Equals(da.Value, db.Value);
		}

		private static void AddParameterToHash(ref HashCode hash, Parameter p)
		{
			hash.Add(p.Name);
			dynamic d = p;
			hash.Add(d.Value);
		}

		public static bool operator ==(CrossSectionParameterYjk left, CrossSectionParameterYjk right) => Equals(left, right);
		public static bool operator !=(CrossSectionParameterYjk left, CrossSectionParameterYjk right) => !Equals(left, right);
	}
}
