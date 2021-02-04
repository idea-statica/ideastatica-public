
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public class IIdeaCrossSectionByCenterLine : IIdeaCrossSection
	{

		public IIdeaCrossSectionByCenterLine()
		{
		}

		public IIdeaMaterial material;

		public IdeaRS.OpenModel.CrossSection.CrossSectionType type;

		public IdeaRS.OpenModel.Geometry2D.PolyLine2D centerLine;

		public double radius;

		public double thickness;

	}
}