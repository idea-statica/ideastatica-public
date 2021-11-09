using Dlubal.RSTAB6;
using Dlubal.RSTAB8;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaRstabPlugin.BimApi
{
	/// <inheritdoc cref="IIdeaMaterialSteel"/>
	internal class RstabMaterialSteel : RstabMaterial, IIdeaMaterialSteel
	{
		private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

		public string Id => $"material-steel-{GetData().No}";

		public string Name
		{
			get
			{
				Utils.ParseMaterialTextID(GetData().TextID, out string name, out string _, out _);
				return name;
			}
		}

		public MatSteel Material => CreateMaterial();

		private readonly IMaterial _material;
		private readonly CountryCode _countryCode;

		public RstabMaterialSteel(CountryCode countryCode, IMaterial material)
		{
			_countryCode = countryCode;
			_material = material;

			_logger.LogDebug($"Created {nameof(RstabMaterialSteel)} with id {Id}");
		}

		private Dlubal.RSTAB8.Material GetData()
		{
			return _material.GetData();
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

			FillMaterialData(GetData(), matSteel);

			return matSteel;
		}

		private MatSteelEc2 SetSteelEc2()
		{
			MatSteelEc2 material = new MatSteelEc2();
			IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			material.fy40 = material.fy;
			material.fu40 = material.fu;
			return material;
		}

		private MatSteelIND SetSteelIND()
		{
			MatSteelIND material = new MatSteelIND();
			IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			material.GammaOVfu = 1.25;
			material.GammaOVfy = 1.25;
			return material;
		}

		private MatSteelAISC SetSteelAISC()
		{
			MatSteelAISC material = new MatSteelAISC();
			IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			material.fy40 = material.fy;
			material.fu40 = material.fu;
			return material;
		}

		private MatSteelCISC SetSteelCISC()
		{
			MatSteelCISC material = new MatSteelCISC();
			IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			return material;
		}

		private MatSteelAUS SetSteelAUS()
		{
			MatSteelAUS material = new MatSteelAUS();
			IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			material.PhiOMFu = 1.3;
			material.PhiOMFy = 1.3;
			return material;
		}

		private MatSteelRUS SetSteelRUS()
		{
			MatSteelRUS material = new MatSteelRUS();
			IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			material.GammaM = 1.03;
			return material;
		}

		private MatSteelHKG SetSteelHKG()
		{
			MatSteelHKG material = new MatSteelHKG();
			IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			material.GammaOVfu = 1.25;
			material.GammaOVfy = 1.25;
			return material;
		}

		private MatSteelCHN SetSteelCHN()
		{
			MatSteelCHN material = new MatSteelCHN();
			IrsMaterialDB2 matDb = (IrsMaterialDB2)_material.GetDatabaseMaterial();
			material.fy = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fyk);
			material.fu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_fuk);
			material.PhiOMFu = 1.1;
			material.PhiOMFy = 1.1;
			return material;
		}
	}
}