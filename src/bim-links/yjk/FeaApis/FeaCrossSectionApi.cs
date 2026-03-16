using APIData;
using IdeaRS.OpenModel.CrossSection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static yjk.Helpers.UnitConverter;

namespace yjk.FeaApis
{
	public interface IFeaCrossSectionApi
	{
		void ReadFromModel(APIData.Hi_DbModelData model);
		int GetCrossSectionId(int memberId, MemberType memberType, int yjkCrossSectionId, int matType, float matGrade, float matGrade2, float matGrade3, IFeaMaterialApi _materialApi);
		IFeaCrossSection GetCrossSection(int id);
		void ClearCrossSections();
	}

	internal class FeaCrossSectionApi : IFeaCrossSectionApi
	{
		APIData.Hi_DbModelData _model;
		List<FeaCrossSection> _crossSections = new List<FeaCrossSection>();
		int _id = 1;

		public void ClearCrossSections() { _crossSections.Clear(); }

		public void ReadFromModel(APIData.Hi_DbModelData model)
		{
			_model = model;
		}

		public int GetCrossSectionId(int memberId, MemberType memberType, int yjkCrossSectionId, int matType, 
			float matGrade, float matGrade2, float matGrade3, IFeaMaterialApi _materialApi)
		{
			int materialId = _materialApi.GetMaterialId(matType, matGrade, matGrade2, matGrade3);

			//Look for existing
			foreach (FeaCrossSection crossSection in _crossSections)
			{
				if (crossSection.YjkId == yjkCrossSectionId && crossSection.MaterialId == materialId && 
					crossSection.MemberType == memberType)
				{
					return crossSection.Id;
				}
			}

			//Add new cross section
			CrossSectionType crossSectionType;
			List<double> shapeParameters = new List<double>();

			(crossSectionType, shapeParameters) = TranslateCrossSection(yjkCrossSectionId, memberType);

			_crossSections.Add(new FeaCrossSection(_id, yjkCrossSectionId, materialId, crossSectionType, 
				shapeParameters, memberType));
			_id++;

			return _id - 1;
		}

		public (CrossSectionType, List<double>) TranslateCrossSection(int yjkCrossSectionId, MemberType memberType)
		{
			Mdl_Section section = new Mdl_Section();

			switch (memberType)
			{
				case MemberType.Column:
					section = _model.m_ColSect.FirstOrDefault(m => m.No == yjkCrossSectionId);

					break;

				case MemberType.Beam:
					section = _model.m_BeamSect.FirstOrDefault(m => m.No == yjkCrossSectionId);
					break;

				case MemberType.Brace:
					section = _model.m_BraceSect.FirstOrDefault(m => m.No == yjkCrossSectionId);
					break;
			}

			int kind = section.Kind;
			string shapeVal = section.ShapeVal;
			string name = section.Name;

			string[] splitShapeVal = shapeVal.Split(',');
			List<double> shapeParameters = new List<double>();

			switch (kind)
			{
				//Rectangle
				case 1:
					shapeParameters.Insert(0, CrossSectionDim(double.Parse(splitShapeVal[1])));
					shapeParameters.Insert(1, CrossSectionDim(double.Parse(splitShapeVal[2])));

					return (CrossSectionType.Rect, shapeParameters);
			}

			return (CrossSectionType.RolledI, shapeParameters);
		}

		public IFeaCrossSection GetCrossSection(int id) => _crossSections.FirstOrDefault(n => n.Id == id);

	}
}
