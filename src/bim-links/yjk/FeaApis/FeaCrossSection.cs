using CsToYjk;
using IdeaRS.OpenModel.CrossSection;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yjk.FeaApis
{
	public interface IFeaCrossSection
	{
		int Id { get; set; }
		int YjkId { get; set; }
		int MaterialId { get; set; }
		CrossSectionType CrossSectionType { get; set; }
		List<double> ShapeParameters { get; set; }
		MemberType MemberType { get; set; }
	}

	internal class FeaCrossSection : IFeaCrossSection
	{
		public FeaCrossSection(int id, int yjkId, int materialId, CrossSectionType csType, 
			List<double> shapeParameters, MemberType memberType)
		{
			Id = id;
			YjkId = yjkId;
			MaterialId = materialId;
			CrossSectionType = csType;
			ShapeParameters = shapeParameters;
			MemberType = memberType;
		}

		public int Id { get; set;  }
		public int YjkId { get; set; }
		public int MaterialId { get; set; }
		public CrossSectionType CrossSectionType { get; set; }
		public List<double> ShapeParameters { get; set; }
		public MemberType MemberType { get; set; }



	}
}
