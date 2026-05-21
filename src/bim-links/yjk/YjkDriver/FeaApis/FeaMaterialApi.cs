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
					switch (matGrade2)
					{
						case 80201:
							matGrade2Translated = "Q235";
							break;
						case 80202:
							matGrade2Translated = "Q345";
							break;
						case 80203:
							matGrade2Translated = "Q390";
							break;
						case 80204:
							matGrade2Translated = "Q420";
							break;
						case 80205:
							matGrade2Translated = "Q460";
							break;
						case 80206:
							matGrade2Translated = "Q500";
							break;
						case 80207:
							matGrade2Translated = "Q550";
							break;
						case 80208:
							matGrade2Translated = "Q620";
							break;
						case 80209:
							matGrade2Translated = "Q690";
							break;
					}

					if (string.IsNullOrEmpty(matGrade2Translated))
						_logger.LogWarning($"Unrecognised steel grade {matGrade2}, material name will be empty");

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
