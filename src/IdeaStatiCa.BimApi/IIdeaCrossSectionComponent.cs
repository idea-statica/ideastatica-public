
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaCrossSectionComponent
	{
		IIdeaMaterial Material { get; }

		IdeaRS.OpenModel.Geometry2D.Region2D Geometry { get; }

		int Phase { get; }



	}
}