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
using System.Xml.Linq;
using yjk.BimApis;
using static yjk.Helpers.UnitConverter;

namespace yjk.FeaApis
{
	public interface IFeaCrossSectionApi
	{
		int GetCrossSectionId(int memberId, MemberType memberType, int yjkCrossSectionId, int matType, 
			float matGrade, float matGrade2, float matGrade3, IFeaMaterialApi _materialApi, APIData.Hi_DbModelData model);
		IFeaCrossSection GetCrossSection(int id);
		void ClearCrossSections();
	}

	internal class FeaCrossSectionApi : IFeaCrossSectionApi
	{
		List<FeaCrossSection> _crossSections = new List<FeaCrossSection>();
		int _id = 1;

		public void ClearCrossSections() { _crossSections.Clear(); }

		public int GetCrossSectionId(int memberId, MemberType memberType, int yjkCrossSectionId, int matType, 
			float matGrade, float matGrade2, float matGrade3, IFeaMaterialApi _materialApi, APIData.Hi_DbModelData model)
		{
			int materialId = _materialApi.GetMaterialId(matType, matGrade, matGrade2, matGrade3);

			/*			//Look for existing
						foreach (FeaCrossSection crossSection in _crossSections)
						{
							if (crossSection.YjkId == yjkCrossSectionId && crossSection.MaterialId == materialId *//*&& 
								crossSection.MemberType == memberType*//*)
							{
								return crossSection.Id;
							}
						}*/

			//Add new cross section
			CrossSectionParameterYjk crossSectionParameterYjk = new CrossSectionParameterYjk();

			string name = "";
			CrossSectionBy crossSectionBy = CrossSectionBy.ByParameters;
			(name, crossSectionBy) = CreateCrossSection(crossSectionParameterYjk, yjkCrossSectionId, memberType, model);

			FeaCrossSection newFeaCrossSection = new FeaCrossSection(_id, yjkCrossSectionId, name, materialId, memberType, crossSectionParameterYjk, crossSectionBy);

			//Look for existing
			foreach (FeaCrossSection crossSection in _crossSections)
			{
				if (newFeaCrossSection.MaterialId == crossSection.MaterialId &&
					newFeaCrossSection.MemberType == crossSection.MemberType &&
					newFeaCrossSection.CrossSectionParameterYjk == crossSection.CrossSectionParameterYjk &&
					newFeaCrossSection.CrossSectionBy == crossSection.CrossSectionBy
					)
				{
					return crossSection.Id;
				}

			}

			_crossSections.Add(newFeaCrossSection);
			_id++;

			return _id - 1;
		}

		private (string, CrossSectionBy) CreateCrossSection(CrossSectionParameterYjk crossSectionParameterYjk, int yjkCrossSectionId, 
			MemberType memberType, APIData.Hi_DbModelData model)
		{
			Mdl_Section section = new Mdl_Section();

			switch (memberType)
			{
				case MemberType.Column:
					section = model.m_ColSect.FirstOrDefault(m => m.No == yjkCrossSectionId);
					break;

				case MemberType.Beam:
					section = model.m_BeamSect.FirstOrDefault(m => m.No == yjkCrossSectionId);
					break;

				case MemberType.Brace:
					section = model.m_BraceSect.FirstOrDefault(m => m.No == yjkCrossSectionId);
					break;
			}

			int kind = section.Kind;
			string shapeVal = section.ShapeVal;

			//string name = section.Name;
			string name = section.ShapeVal;

			string[] splitShapeVal = shapeVal.Split(',');
			CrossSectionBy crossSectionBy = CrossSectionBy.ByParameters;

			switch (kind)
			{
				//Rectangle
				case 1:
					{
						double width = MmToM(double.Parse(splitShapeVal[1]));
						double height = MmToM(double.Parse(splitShapeVal[2]));

						CrossSectionFactory.FillRectangle(crossSectionParameterYjk, width, height);
						break;
					}

				//I section
				case 2:
					{

						double webThk = MmToM(double.Parse(splitShapeVal[1]));
						double height = MmToM(double.Parse(splitShapeVal[2]));
						double upperFlangeWidth = MmToM(double.Parse(splitShapeVal[3]));
						double upperFlangeThk = MmToM(double.Parse(splitShapeVal[4]));
						double bottomFlageWidth = MmToM(double.Parse(splitShapeVal[5]));
						double bottomFlangeThk = MmToM(double.Parse(splitShapeVal[6]));

						if (upperFlangeWidth == bottomFlageWidth && upperFlangeThk == bottomFlangeThk)
						{
							//RolledI
							CrossSectionFactory.FillRolledI(crossSectionParameterYjk, upperFlangeWidth, height, webThk,
								upperFlangeThk, 0, 0, 0);
						}
						else
						{
							//WeldedAsymI
							CrossSectionFactory.FillWeldedAsymI(crossSectionParameterYjk, upperFlangeWidth, bottomFlageWidth,
								height - upperFlangeThk - bottomFlangeThk, webThk, upperFlangeThk, bottomFlangeThk);
						}
						break;
					}

				//Circle
				case 3:
					{
						double diameter = MmToM(double.Parse(splitShapeVal[1]));
						CrossSectionFactory.FillCircle(crossSectionParameterYjk, diameter);
						break;
					}

				//Regular polygon
				case 4:
					{
						//Not implemented, use filled circle
						double diameter = MmToM(double.Parse(splitShapeVal[1]));
						double numSides = MmToM(double.Parse(splitShapeVal[2]));

						CrossSectionFactory.FillCircle(crossSectionParameterYjk, diameter);
						break;
					}

				//Channel
				case 5:
					{
						double webThk = MmToM(double.Parse(splitShapeVal[1]));
						double height = MmToM(double.Parse(splitShapeVal[2]));
						double topFlangeWidth = MmToM(double.Parse(splitShapeVal[3]));
						double topFlangeThk = MmToM(double.Parse(splitShapeVal[4]));
						double bottomFlangeWidth = MmToM(double.Parse(splitShapeVal[5]));
						double bottomFlangeThk = MmToM(double.Parse(splitShapeVal[6]));

						if (topFlangeWidth == bottomFlangeWidth && topFlangeThk == bottomFlangeThk)
						{
							CrossSectionFactory.FillRolledChannel(crossSectionParameterYjk, topFlangeWidth+webThk, height, webThk, 
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
						double width = MmToM(double.Parse(splitShapeVal[1]));
						double height = MmToM(double.Parse(splitShapeVal[2]));
						double topFlangeThk = MmToM(double.Parse(splitShapeVal[3]));
						double leftFlangeThk = MmToM(double.Parse(splitShapeVal[4]));
						double bottomFlangeThk = MmToM(double.Parse(splitShapeVal[5]));
						double rightFlangeThk = MmToM(double.Parse(splitShapeVal[6]));

						if (leftFlangeThk == rightFlangeThk)
						{
							CrossSectionFactory.FillWeldedBoxFlange(crossSectionParameterYjk, width, width, height - topFlangeThk - bottomFlangeThk,
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
						double outerDiameter = MmToM(double.Parse(splitShapeVal[1]));
						double innerDiameter = MmToM(double.Parse(splitShapeVal[2]));

						CrossSectionFactory.FillRolledCHS(crossSectionParameterYjk, outerDiameter * 0.5, (outerDiameter - innerDiameter) * 0.5);

						break;
					}

				//Double channel 2Uc
				case 9:
					{
						double webThk = MmToM(double.Parse(splitShapeVal[1]));
						double height = MmToM(double.Parse(splitShapeVal[2]));
						double topFlangeWidth = MmToM(double.Parse(splitShapeVal[3]));
						double flangeThk = MmToM(double.Parse(splitShapeVal[4]));
						double bottomFlangeWidth = MmToM(double.Parse(splitShapeVal[5]));
						double spacing = MmToM(double.Parse(splitShapeVal[6]));

						if (topFlangeWidth == bottomFlangeWidth)
						{
							CrossSectionFactory.FillComposedDblUo(crossSectionParameterYjk, topFlangeWidth + flangeThk, height, flangeThk,
								flangeThk, spacing);
						}
						else
						{
							//not implemented
							goto default;
						}

						break;
					}

				//Use steel profile
				case 26:
					{
						name = section.Name;

						//Still return by name
						goto default;
					}


				//L
				case 28:
					{
						double webThk = MmToM(double.Parse(splitShapeVal[1]));
						double height = MmToM(double.Parse(splitShapeVal[2]));
						double flangeWidth = MmToM(double.Parse(splitShapeVal[3]));
						double flangeThk = MmToM(double.Parse(splitShapeVal[4]));

						if (webThk == flangeThk)
						{
							CrossSectionFactory.FillRolledAngle(crossSectionParameterYjk, flangeWidth + webThk, height, webThk, 0, 0, 0);
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
						double webThk = MmToM(double.Parse(splitShapeVal[1]));
						double height = MmToM(double.Parse(splitShapeVal[2]));
						double flangeWidth = MmToM(double.Parse(splitShapeVal[3]));
						double flangeThk = MmToM(double.Parse(splitShapeVal[4]));

						CrossSectionFactory.FillRolledT(crossSectionParameterYjk, flangeWidth, height, webThk, flangeThk, 0, 0, 0, 0, 0, true);

						break;
					}

				//Cold formed profile
				case 303:
					{
						name = section.Name;

						//Still return by name
						goto default;
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
