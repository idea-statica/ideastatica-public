﻿using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiFeaLink.BimApi
{
	internal class Member1D : IdeaMember1D
	{
		public int CrossSectionNo { get; set; }

		public override IIdeaCrossSection CrossSection => Get<IIdeaCrossSection>(CrossSectionNo);

		public Member1D(int no) : base(no)
		{
		}
	}
}