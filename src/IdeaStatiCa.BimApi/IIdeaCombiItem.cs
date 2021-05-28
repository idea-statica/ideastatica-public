using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaCombiItem : IIdeaObject
	{
		IIdeaLoadCase LoadCase { get; set; }

		/// <summary>
		/// Coefficient
		/// </summary>
		System.Double Coeff { get; set; }

		/// <summary>
		/// Inserted another combination
		/// </summary>
		IIdeaCombiInput Combination { get; set; }
	}
}
