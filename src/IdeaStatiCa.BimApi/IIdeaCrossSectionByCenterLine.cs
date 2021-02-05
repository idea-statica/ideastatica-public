
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaCrossSectionByCenterLine : IIdeaCrossSection
	{

		IIdeaMaterial Material { get; }

		IdeaRS.OpenModel.CrossSection.CrossSectionType Type { get; }

		IdeaRS.OpenModel.Geometry2D.PolyLine2D CenterLine { get; }

		double Radius { get; }

		double Thickness { get; }

	}
}