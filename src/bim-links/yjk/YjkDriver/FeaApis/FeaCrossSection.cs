using CsToYjk;
using IdeaRS.OpenModel.CrossSection;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yjk.BimApis;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Xml.Linq;

namespace yjk.FeaApis
{
	public interface IFeaCrossSection
	{
		string Id { get; set; }
		int YjkId { get; set; }
		string Name { get; set; }
		string MaterialId { get; set; }
		MemberType MemberType { get; set; }
		CrossSectionParameterYjk CrossSectionParameterYjk { get; set; }
		CrossSectionBy CrossSectionBy { get; set; }
	}

	internal class FeaCrossSection : IFeaCrossSection
	{
		public FeaCrossSection(string id, int yjkId, string name, string materialId, MemberType memberType, 
			CrossSectionParameterYjk crossSectionParameterYjk, CrossSectionBy crossSectionBy)
		{
			Id = id;
			YjkId = yjkId;
			Name = name;
			MaterialId = materialId;
			MemberType = memberType;
			CrossSectionParameterYjk = crossSectionParameterYjk;
			CrossSectionBy = crossSectionBy;
		}


		public string Id { get; set;  }
		public int YjkId { get; set; }
		public string Name { get; set; }
		public string MaterialId { get; set; }
		public MemberType MemberType { get; set; }
		public CrossSectionParameterYjk CrossSectionParameterYjk { get; set; }
		public CrossSectionBy CrossSectionBy { get; set; }

	}

	public enum CrossSectionBy
	{
		ByParameters,
		ByName,
	}
}
