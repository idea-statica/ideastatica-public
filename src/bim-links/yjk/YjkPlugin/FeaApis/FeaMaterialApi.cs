using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using yjk.ViewModels;

namespace yjk.FeaApis
{
	public interface IFeaMaterialApi
	{
		string GetMaterialId(int matType, float matGrade, float matGrade2, float matGrade3);
		IFeaMaterial GetMaterial(string id);
		void ClearMaterials();
	}

	public class FeaMaterialApi : IFeaMaterialApi
	{
		List<FeaMaterial> _materials = new List<FeaMaterial>();
		int _id = 1;
		private IPluginLogger _logger = AppLogger.Instance;

		public void ClearMaterials() {_materials.Clear(); }

		public string GetMaterialId(int matType, float matGrade, float matGrade2, float matGrade3)
		{
			_logger.LogInformation($"FeaMaterialApi.GetMaterialId: matType={matType}, matGrade={matGrade}, matGrade2={matGrade2}");
			//Convert matType
			MaterialType materialType = new MaterialType();
			switch (matType)
			{
				//Steel
				case 5:
					materialType = MaterialType.Steel;

					//matGrade2 translator
					string matGrade2Translated = "";

					// Sourced from 前处理计算参数宏定义.docx - 基本信息 section
					const int YP_GANGH = 802;
					const int YP_RES_SIZE = 100;  // #define YP_RES_SIZE 100

					switch (matGrade2)
					{
						// Chinese Standard (GB)
						case YP_GANGH * YP_RES_SIZE + 1: matGrade2Translated = "Q235"; break;
						case YP_GANGH * YP_RES_SIZE + 2: matGrade2Translated = "Q345"; break;
						case YP_GANGH * YP_RES_SIZE + 3: matGrade2Translated = "Q390"; break;
						case YP_GANGH * YP_RES_SIZE + 4: matGrade2Translated = "Q420"; break;
						case YP_GANGH * YP_RES_SIZE + 5: matGrade2Translated = "Q460"; break;
						case YP_GANGH * YP_RES_SIZE + 6: matGrade2Translated = "Q500"; break;
						case YP_GANGH * YP_RES_SIZE + 7: matGrade2Translated = "Q550"; break; // Q560 and Q550 share the same value
						case YP_GANGH * YP_RES_SIZE + 8: matGrade2Translated = "Q620"; break;
						case YP_GANGH * YP_RES_SIZE + 9: matGrade2Translated = "Q690"; break;
						case YP_GANGH * YP_RES_SIZE + 10: matGrade2Translated = "Q235GJ"; break;
						case YP_GANGH * YP_RES_SIZE + 11: matGrade2Translated = "Q345GJ"; break;
						case YP_GANGH * YP_RES_SIZE + 12: matGrade2Translated = "Q390GJ"; break;
						case YP_GANGH * YP_RES_SIZE + 13: matGrade2Translated = "Q420GJ"; break;
						case YP_GANGH * YP_RES_SIZE + 14: matGrade2Translated = "Q460GJ"; break;
						case YP_GANGH * YP_RES_SIZE + 15: matGrade2Translated = "Q355"; break;
						case YP_GANGH * YP_RES_SIZE + 16: matGrade2Translated = "Q355N"; break;
						case YP_GANGH * YP_RES_SIZE + 17: matGrade2Translated = "Q390N"; break;
						case YP_GANGH * YP_RES_SIZE + 18: matGrade2Translated = "Q420N"; break;
						case YP_GANGH * YP_RES_SIZE + 19: matGrade2Translated = "Q460N"; break;
						case YP_GANGH * YP_RES_SIZE + 20: matGrade2Translated = "Q355M"; break;
						case YP_GANGH * YP_RES_SIZE + 21: matGrade2Translated = "Q390M"; break;
						case YP_GANGH * YP_RES_SIZE + 22: matGrade2Translated = "Q420M"; break;
						case YP_GANGH * YP_RES_SIZE + 23: matGrade2Translated = "Q460M"; break;

						// European Standard (EN)
						case YP_GANGH * YP_RES_SIZE + 51: matGrade2Translated = "S235"; break;
						case YP_GANGH * YP_RES_SIZE + 52: matGrade2Translated = "S275"; break;
						case YP_GANGH * YP_RES_SIZE + 53: matGrade2Translated = "S355"; break;
						case YP_GANGH * YP_RES_SIZE + 54: matGrade2Translated = "S450"; break;

						// US Standard (ASTM)
						case YP_GANGH * YP_RES_SIZE + 71: matGrade2Translated = "A36"; break;
						case YP_GANGH * YP_RES_SIZE + 72: matGrade2Translated = "A53 (Gr.B)"; break;
						case YP_GANGH * YP_RES_SIZE + 73: matGrade2Translated = "A500 (Gr.B42)"; break;
						case YP_GANGH * YP_RES_SIZE + 74: matGrade2Translated = "A500 (Gr.B46)"; break;
						case YP_GANGH * YP_RES_SIZE + 75: matGrade2Translated = "A500 (Gr.C46)"; break;
						case YP_GANGH * YP_RES_SIZE + 76: matGrade2Translated = "A500 (Gr.C50)"; break;
						case YP_GANGH * YP_RES_SIZE + 77: matGrade2Translated = "A572 (Gr.50)"; break;
						case YP_GANGH * YP_RES_SIZE + 78: matGrade2Translated = "A992"; break;
						default:
							_logger.LogWarning($"Unrecognised steel grade {matGrade2}, material will be named unknown");
							matGrade2Translated = "unknown";
							break;
					}

					//Look at available materials
					foreach (FeaMaterial material in _materials)
					{
						if (material.MaterialType == materialType && material.Name == matGrade2Translated)
						{
							return material.Id;
						}
					}

					//Add new material
					_logger.LogInformation($"Material added: Steel, name={matGrade2Translated}");
					_materials.Add(new FeaMaterialSteel(matGrade2Translated, matGrade2Translated));
					//_id++;

					return matGrade2Translated;

				//Concrete
				case 6:
					materialType = MaterialType.Concrete;

					//Look at available materials
					foreach (FeaMaterial material in _materials)
					{
						if (material.MaterialType == materialType)
						{
							if (material.MaterialType == materialType && ((FeaMaterialConcrete)material).Fck == matGrade)
							{
								return material.Id;
							}
						}
					}

					//Add new material
					_logger.LogInformation($"Material added: Concrete, Fck={matGrade}");
					_materials.Add(new FeaMaterialConcrete(matGrade.ToString(), matGrade.ToString(), matGrade));
					//_id++;

					return matGrade.ToString();
			}

			_logger.LogWarning($"Unrecognised material type {matType}, returning empty material ID");
			return "";
		}

		public IFeaMaterial GetMaterial(string id) => _materials.FirstOrDefault(n => n.Id == id);
	}
}
