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
		int Id { get; set; }
		int YjkId { get; set; }
		string Name { get; set; }
		int MaterialId { get; set; }
		MemberType MemberType { get; set; }
		CrossSectionParameter CrossSectionByParameters { get; set; }
		CrossSectionBy CrossSectionBy { get; set; }
	}

	internal class FeaCrossSection : IFeaCrossSection
	{
		public FeaCrossSection(int id, int yjkId, string name, int materialId, MemberType memberType, 
			CrossSectionParameter crossSectionByParameters, CrossSectionBy crossSectionBy)
		{
			Id = id;
			YjkId = yjkId;
			Name = name;
			MaterialId = materialId;
			MemberType = memberType;
			CrossSectionByParameters = crossSectionByParameters;
			CrossSectionBy = crossSectionBy;
		}

		public int Id { get; set;  }
		public int YjkId { get; set; }
		public string Name { get; set; }
		public int MaterialId { get; set; }
		public MemberType MemberType { get; set; }
		public CrossSectionParameter CrossSectionByParameters { get; set; }
		public CrossSectionBy CrossSectionBy { get; set; }

	}

	public enum CrossSectionBy
	{
		ByParameters,
		ByName,
	}
}
