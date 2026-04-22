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

					//Cast to child class
					switch (Parameters[i])
					{
						case ParameterDouble p1 when other.Parameters[i] is ParameterDouble p2:
							if (p1.Name != p2.Name || p1.Value != p2.Value)
								parametersAreEqual = false;
							break;

						case ParameterInt p1 when other.Parameters[i] is ParameterInt p2:
							if (p1.Name != p2.Name || p1.Value != p2.Value)
								parametersAreEqual = false;
							break;

						case ParameterBool p1 when other.Parameters[i] is ParameterBool p2:
							if (p1.Name != p2.Name || p1.Value != p2.Value)
								parametersAreEqual = false;
							break;

						case ParameterString p1 when other.Parameters[i] is ParameterString p2:
							if (p1.Name != p2.Name || p1.Value != p2.Value)
								parametersAreEqual = false;
							break;

						case ParameterReferenceElement p1 when other.Parameters[i] is ParameterReferenceElement p2:
							if (p1.Name != p2.Name || p1.Value != p2.Value)
								parametersAreEqual = false;
							break;

						default:
							parametersAreEqual = false;
							break;
					}

					if (!parametersAreEqual)
						break;

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
				switch (param)
				{
					case ParameterDouble p:
						hash.Add(p.Name);
						hash.Add(p.Value);
						break;

					case ParameterInt p:
						hash.Add(p.Name);
						hash.Add(p.Value);
						break;

					case ParameterBool p:
						hash.Add(p.Name);
						hash.Add(p.Value);
						break;

					case ParameterString p:
						hash.Add(p.Name);
						hash.Add(p.Value);
						break;

					case ParameterReferenceElement p:
						hash.Add(p.Name);
						hash.Add(p.Value);
						break;
				}
			}

			return hash.ToHashCode();
		}

		public static bool operator ==(CrossSectionParameterYjk left, CrossSectionParameterYjk right) => Equals(left, right);
		public static bool operator !=(CrossSectionParameterYjk left, CrossSectionParameterYjk right) => !Equals(left, right);
	}
}
