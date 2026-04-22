using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimImporter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace yjk.BimApis
{
	internal class CrossSectionByParameters : IdeaCrossSectionByParameters
	{
		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialNo);

		public int MaterialNo { get; set; }

		//override public double  Rotation { get; set; }

		//override public IdeaRS.OpenModel.CrossSection.CrossSectionType Type { get; set; }

		public CrossSectionByParameters(string stringId) : base(stringId)
		{
		}


	}
}
