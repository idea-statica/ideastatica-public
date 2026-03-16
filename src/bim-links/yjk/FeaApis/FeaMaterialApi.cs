using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yjk.FeaApis
{
	public interface IFeaMaterialApi
	{
		int GetMaterialId(int matType, float matGrade, float matGrade2, float matGrade3);
		IFeaMaterial GetMaterial(int id);
		void ClearMaterials();
	}

	public class FeaMaterialApi : IFeaMaterialApi
	{
		List<FeaMaterial> _materials = new List<FeaMaterial>();
		int _id = 1;

		public void ClearMaterials() {_materials.Clear(); }

		public int GetMaterialId(int matType, float matGrade, float matGrade2, float matGrade3)
		{
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
					_materials.Add(new FeaMaterialSteel(_id, matGrade2Translated));
					_id++;

					return _id - 1;

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
					_materials.Add(new FeaMaterialConcrete(_id, matGrade.ToString(), matGrade));
					_id++;

					return _id - 1;
			}

			return -1;
		}

		public IFeaMaterial GetMaterial(int id) => _materials.FirstOrDefault(n => n.Id == id);
	}
}
