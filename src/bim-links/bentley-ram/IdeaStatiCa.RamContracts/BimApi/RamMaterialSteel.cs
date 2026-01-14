using ISteelMaterial = RAMDATAACCESSLib.ISteelMaterial;

using IdeaStatiCa.BimApi;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.RamToIdea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	public class RamMaterialSteel : RamMaterial, IIdeaMaterialSteel
	{

		//TODO
		//private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

		public string Id => $"material-steel-{_material.Id}";

		public string Name
		{
			get
			{
				
				//Utils.ParseMaterialTextID(GetData().TextID, out string name, out string _, out _);
				return "Steel " + _material.Fy.ToString();
			}
		}


		public MatSteel Material => CreateMaterial();

		private readonly RamSteelMaterialWrapper _material;
		
		//TODO NEED TO CHECK IF COUNTRY CODE IS RETRIEVED
		private readonly CountryCode _countryCode;

		public RamMaterialSteel(CountryCode countryCode, RamSteelMaterialWrapper material)
		{
			_countryCode = countryCode;
			_material = material;

			//TODO
			//_logger.LogDebug($"Created {nameof(RamMaterialSteel)} with id {Id}");
		}

		private MatSteel CreateMaterial()
		{
			MatSteel matSteel = null;

			switch (_countryCode)
			{
				case CountryCode.ECEN:
					matSteel = SetSteelEc2();
					break;

				case CountryCode.India:
					matSteel = SetSteelIND();
					break;

				case CountryCode.SIA:
					break;

				case CountryCode.American:
					matSteel = SetSteelAISC();
					break;

				case CountryCode.Canada:
					matSteel = SetSteelCISC();
					break;

				case CountryCode.Australia:
					matSteel = SetSteelAUS();
					break;

				case CountryCode.RUS:
					matSteel = SetSteelRUS();
					break;

				case CountryCode.CHN:
					matSteel = SetSteelCHN();
					break;

				case CountryCode.HKG:
					matSteel = SetSteelHKG();
					break;

				default:
					throw new NotImplementedException($"Unsupported country code {_countryCode}");
			}

			FillMaterialData(_material, matSteel);

			return matSteel;
		}

		private MatSteelEc2 SetSteelEc2()
		{
			MatSteelEc2 material = new MatSteelEc2();
			//IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			
			material.fy = _material.Fy;

			//Can we get the Material Fu from Ram??
			material.fu = _material.Fu;
			
			material.fy40 = material.fy;
			material.fu40 = material.fu;
			return material;
		}

		//TODO all other materials except Euro. Need to look at which codes are avaliable in RAM.
		private MatSteelIND SetSteelIND()
		{
			MatSteelIND material = new MatSteelIND();
			//IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			//material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			//material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			//material.GammaOVfu = 1.25;
			//material.GammaOVfy = 1.25;
			return material;
		}

		private MatSteelAISC SetSteelAISC()
		{
			MatSteelAISC material = new MatSteelAISC();
			//IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			//material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			//material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			//material.fy40 = material.fy;
			//material.fu40 = material.fu;
			return material;
		}

		private MatSteelCISC SetSteelCISC()
		{
			MatSteelCISC material = new MatSteelCISC();
			//IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			//material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			//material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			return material;
		}

		private MatSteelAUS SetSteelAUS()
		{
			MatSteelAUS material = new MatSteelAUS();
			//IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			//material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			//material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			//material.PhiOMFu = 1.3;
			//material.PhiOMFy = 1.3;
			return material;
		}

		private MatSteelRUS SetSteelRUS()
		{
			MatSteelRUS material = new MatSteelRUS();
			//IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			//material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			//material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			//material.GammaM = 1.03;
			return material;
		}

		private MatSteelHKG SetSteelHKG()
		{
			MatSteelHKG material = new MatSteelHKG();
			//IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			//material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			//material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			//material.GammaOVfu = 1.25;
			//material.GammaOVfy = 1.25;
			return material;
		}

		private MatSteelCHN SetSteelCHN()
		{
			MatSteelCHN material = new MatSteelCHN();
			//IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			//material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			//material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			//material.PhiOMFu = 1.1;
			//material.PhiOMFy = 1.1;
			return material;
		}
	}
}
