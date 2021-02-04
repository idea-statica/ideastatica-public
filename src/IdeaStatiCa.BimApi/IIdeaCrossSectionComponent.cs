
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public class IIdeaCrossSectionComponent
	{

		public IIdeaCrossSectionComponent()
		{
		}

		public IIdeaMaterial material;

		public IdeaRS.OpenModel.Geometry2D.Region2D geometry;

		public int phase;



	}
}