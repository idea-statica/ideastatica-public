using APIData;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Message;
using IdeaStatiCa.BimApiLink.BimApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
				if (crossSection.YjkId == yjkCrossSectionId && crossSection.MaterialId == materialId /*&& 
					crossSection.MemberType == memberType*/)
				{
					return crossSection.Id;
				}
			}

			//Add new cross section
			CrossSectionParameter css = new CrossSectionParameter();

			string name = "";
			CrossSectionBy crossSectionBy = CrossSectionBy.ByParameters;
			(name, crossSectionBy) = CreateCrossSection(css, yjkCrossSectionId, memberType);

			_crossSections.Add(new FeaCrossSection(_id, yjkCrossSectionId, name, materialId, memberType, css, crossSectionBy));
			_id++;

			return _id - 1;
		}

		public (string, CrossSectionBy) CreateCrossSection(CrossSectionParameter css, int yjkCrossSectionId, MemberType memberType)
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
			CrossSectionBy crossSectionBy = CrossSectionBy.ByParameters;

			switch (kind)
			{
				//Rectangle
				case 1:
					{
						double width = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double height = CrossSectionDim(double.Parse(splitShapeVal[2]));

						CrossSectionFactory.FillRectangle(css, width, height);
						break;
					}

				//I section
				case 2:
					{

						double webThk = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double height = CrossSectionDim(double.Parse(splitShapeVal[2]));
						double upperFlangeWidth = CrossSectionDim(double.Parse(splitShapeVal[3]));
						double upperFlangeThk = CrossSectionDim(double.Parse(splitShapeVal[4]));
						double bottomFlageWidth = CrossSectionDim(double.Parse(splitShapeVal[5]));
						double bottomFlangeThk = CrossSectionDim(double.Parse(splitShapeVal[6]));

						if (upperFlangeWidth == bottomFlageWidth && upperFlangeThk == bottomFlangeThk)
						{
							//RolledI
							CrossSectionFactory.FillRolledI(css, upperFlangeWidth, height, webThk,
								upperFlangeThk, 0, 0, 0);
						}
						else
						{
							//WeldedAsymI
							CrossSectionFactory.FillWeldedAsymI(css, upperFlangeWidth, bottomFlageWidth,
								height - upperFlangeThk - bottomFlangeThk, webThk, upperFlangeThk, bottomFlangeThk);
						}
						break;
					}

				//Circle
				case 3:
					{
						double diameter = CrossSectionDim(double.Parse(splitShapeVal[1]));
						CrossSectionFactory.FillCircle(css, diameter);
						break;
					}

				//Regular polygon
				case 4:
					{
						//Not implemented, use filled circle
						double diameter = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double numSides = CrossSectionDim(double.Parse(splitShapeVal[2]));

						CrossSectionFactory.FillCircle(css, diameter);
						break;
					}

				//Channel
				case 5:
					{
						double webThk = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double height = CrossSectionDim(double.Parse(splitShapeVal[2]));
						double topFlangeWidth = CrossSectionDim(double.Parse(splitShapeVal[3]));
						double topFlangeThk = CrossSectionDim(double.Parse(splitShapeVal[4]));
						double bottomFlangeWidth = CrossSectionDim(double.Parse(splitShapeVal[5]));
						double bottomFlangeThk = CrossSectionDim(double.Parse(splitShapeVal[6]));

						if (topFlangeWidth == bottomFlangeWidth && topFlangeThk == bottomFlangeThk)
						{
							CrossSectionFactory.FillRolledChannel(css, topFlangeWidth+webThk, height, webThk, 
								topFlangeThk, 0, 0, 0);
						}
						else
						{
							//not implemented
							goto default;
						}
						break;
					}

				//Box
				case 7:
					{
						double width = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double height = CrossSectionDim(double.Parse(splitShapeVal[2]));
						double topFlangeThk = CrossSectionDim(double.Parse(splitShapeVal[3]));
						double leftFlangeThk = CrossSectionDim(double.Parse(splitShapeVal[4]));
						double bottomFlangeThk = CrossSectionDim(double.Parse(splitShapeVal[5]));
						double rightFlangeThk = CrossSectionDim(double.Parse(splitShapeVal[6]));

						if (leftFlangeThk == rightFlangeThk)
						{
							CrossSectionFactory.FillWeldedBoxFlange(css, width, width, height - topFlangeThk - bottomFlangeThk,
								width - leftFlangeThk - rightFlangeThk, leftFlangeThk, topFlangeThk, bottomFlangeThk);
						}
						else
						{
							//not implemented
							goto default;
						}
						break;
					}

				//Circular hollow section
				case 8:
					{
						double outerDiameter = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double innerDiameter = CrossSectionDim(double.Parse(splitShapeVal[2]));

						CrossSectionFactory.FillRolledCHS(css, outerDiameter * 0.5, (outerDiameter - innerDiameter) * 0.5);

						break;
					}

				//Double channel 2Uc
				case 9:
					{
						double webThk = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double height = CrossSectionDim(double.Parse(splitShapeVal[2]));
						double topFlangeWidth = CrossSectionDim(double.Parse(splitShapeVal[3]));
						double flangeThk = CrossSectionDim(double.Parse(splitShapeVal[4]));
						double bottomFlangeWidth = CrossSectionDim(double.Parse(splitShapeVal[5]));
						double spacing = CrossSectionDim(double.Parse(splitShapeVal[6]));

						if (topFlangeWidth == bottomFlangeWidth)
						{
							CrossSectionFactory.FillComposedDblUo(css, topFlangeWidth + flangeThk, height, flangeThk,
								flangeThk, spacing);
						}
						else
						{
							//not implemented
							goto default;
						}

						break;
					}


				//L
				case 28:
					{
						double webThk = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double height = CrossSectionDim(double.Parse(splitShapeVal[2]));
						double flangeWidth = CrossSectionDim(double.Parse(splitShapeVal[3]));
						double flangeThk = CrossSectionDim(double.Parse(splitShapeVal[4]));

						if (webThk == flangeThk)
						{
							CrossSectionFactory.FillRolledAngle(css, flangeWidth + webThk, height, webThk, 0, 0, 0);
						}
						else
						{
							//not implemented
							goto default;
						}

						break;
					}

				//T
				case 29:
					{
						double webThk = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double height = CrossSectionDim(double.Parse(splitShapeVal[2]));
						double flangeWidth = CrossSectionDim(double.Parse(splitShapeVal[3]));
						double flangeThk = CrossSectionDim(double.Parse(splitShapeVal[4]));

						CrossSectionFactory.FillRolledT(css, flangeWidth, height, webThk, flangeThk, 0, 0, 0, 0, 0, true);

						break;
					}

				//Cold formed profile
				case 303:
					{
						Debug.WriteLine("a");


						double webThk = CrossSectionDim(double.Parse(splitShapeVal[1]));
						double height = CrossSectionDim(double.Parse(splitShapeVal[2]));
						double flangeWidth = CrossSectionDim(double.Parse(splitShapeVal[3]));
						double flangeThk = CrossSectionDim(double.Parse(splitShapeVal[4]));

						CrossSectionFactory.FillRolledT(css, flangeWidth, height, webThk, flangeThk, 0, 0, 0, 0, 0);

						break;
					}

				default:
					{
						crossSectionBy = CrossSectionBy.ByName;
						break;
					}
			}
			return (name, crossSectionBy);
		}
	
		
		public IFeaCrossSection GetCrossSection(int id) => _crossSections.FirstOrDefault(n => n.Id == id);

	}
}
