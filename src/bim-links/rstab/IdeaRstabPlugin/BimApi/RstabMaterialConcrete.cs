using Dlubal.RSTAB6;
using Dlubal.RSTAB8;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaRstabPlugin.BimApi
{
	/// <inheritdoc cref="IIdeaMaterialConcrete"/>
	internal class RstabMaterialConcrete : RstabMaterial, IIdeaMaterialConcrete
	{
		private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

		public string Id => $"material-concrete-{GetData().No}";
		private CountryCode m_CountryCode = CountryCode.ECEN;
		private readonly IMaterial _material;

		public RstabMaterialConcrete(CountryCode countryCode, IMaterial source)
		{
			this.m_CountryCode = countryCode;
			_material = source;

			_logger.LogDebug($"Created {nameof(RstabMaterialConcrete)} with id {Id}");
		}

		public string Name
		{
			get
			{
				Utils.ParseMaterialTextID(GetData().TextID, out string name, out string _, out _);
				return name;
			}
		}

		public MatConcrete Material => CreateMaterial();

		private Dlubal.RSTAB8.Material GetData()
		{
			return _material.GetData();
		}

		private MatConcrete CreateMaterial()
		{
			Dlubal.RSTAB8.Material data = GetData();
			MatConcrete matConcrete = null;
			switch (m_CountryCode)
			{
				case CountryCode.None:
					throw new NotImplementedException($"Unsupported country code {m_CountryCode}");

				case CountryCode.ECEN:
					matConcrete = new MatConcreteEc2();
					SetConcreteEc2(matConcrete as MatConcreteEc2);
					break;

				//case CountryCode.India:
				//	matConcrete = new MatConcreteIND();
				//	SetConcreteIND(matConcrete as MatConcreteIND);
				//	break;

				//case CountryCode.SIA:
				//	matConcrete = new MatConcreteSIA();
				//	SetConcreteSIA(matConcrete as MatConcreteSIA);
				//	break;

				//case CountryCode.American:
				//	matConcrete = new MatConcreteACI();
				//	SetConcreteACI(matConcrete as MatConcreteACI);
				//	break;

				//case CountryCode.Canada:
				//	matConcrete = new MatConcreteCAN();
				//	SetConcreteCAN(matConcrete as MatConcreteCAN);
				//	break;

				//case CountryCode.Australia:
				//	matConcrete = new MatConcreteAUS();
				//	SetConcreteAUS(matConcrete as MatConcreteAUS);
				//	break;

				//case CountryCode.RUS:
				//	matConcrete = new MatConcreteRUS();
				//	SetConcreteRUS(matConcrete as MatConcreteRUS);
				//	break;

				//case CountryCode.CHN:
				//	matConcrete = new MatConcreteCHN();
				//	SetConcreteCHN(matConcrete as MatConcreteCHN);
				//	break;

				//case CountryCode.HKG:
				//	matConcrete = new MatConcreteHKG();
				//	SetConcreteHKG(matConcrete as MatConcreteHKG);
				//	break;

				default:
					break;
			}

			FillMaterialData(data, matConcrete);

			return matConcrete;
		}

		private void SetConcreteEc2(MatConcreteEc2 material)
		{
			Dlubal.RSTAB6.IrsDatabaseMat pDBMat = _material.GetDatabaseMaterial() as Dlubal.RSTAB6.IrsDatabaseMat;
			IrsMaterialDB pMat = pDBMat.rsGetMaterial(GetData().TextID);

			material.Fck = pMat.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_f_ck);
		}
		/*
		private void SetConcreteIND(MatConcreteIND material)
		{
			IrsMaterialDB2 matDb = _material.GetDatabaseMaterial() as IrsMaterialDB2;
			material.Fck = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_f_ck);
		}

		private void SetConcreteSIA(MatConcreteSIA material)
		{
			IrsMaterialDB2 matDb = _material.GetDatabaseMaterial() as IrsMaterialDB2;
			material.Fck = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_f_ck);
			material.CalculateDependentValues = true;
		}

		private void SetConcreteACI(MatConcreteACI material)
		{
			IrsMaterialDB2 matDb = _material.GetDatabaseMaterial() as IrsMaterialDB2;
			material.Fcc = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_f_ck);
		}

		private void SetConcreteCAN(MatConcreteCAN material)
		{
			IrsMaterialDB2 matDb = _material.GetDatabaseMaterial() as IrsMaterialDB2;
			material.Fcc = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_f_ck);
		}

		private void SetConcreteAUS(MatConcreteAUS material)
		{
			IrsMaterialDB2 matDb = _material.GetDatabaseMaterial() as IrsMaterialDB2;
			material.Fcc = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_f_ck);
		}

		private void SetConcreteRUS(MatConcreteRUS material)
		{
			IrsMaterialDB2 matDb = _material.GetDatabaseMaterial() as IrsMaterialDB2;
			material.Fck = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_f_ck);
		}

		private void SetConcreteCHN(MatConcreteCHN material)
		{
			IrsMaterialDB2 matDb = _material.GetDatabaseMaterial() as IrsMaterialDB2;
			material.Fck = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_f_ck);
		}

		private void SetConcreteHKG(MatConcreteHKG material)
		{
			IrsMaterialDB2 matDb = _material.GetDatabaseMaterial() as IrsMaterialDB2;
			material.Fcu = matDb.rsGetProperty(DB_MAT_PROPERTY.MAT_PROP_f_ck);
		}
		*/
	}
}