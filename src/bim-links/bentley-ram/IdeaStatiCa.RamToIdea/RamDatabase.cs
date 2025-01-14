using Autofac;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Providers;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IdeaStatiCa.RamToIdea
{
	public class RamDatabase : IDisposable
	{
		private bool _disposed;
		private bool _isOpen;

		private readonly IDBIO1 _dbIo;
		private readonly IContainer _container;
		private readonly string _path;

		// RamDataAccess1 must be released after IDBIO1 so it cannot be a local variable
		private readonly RamDataAccess1 _ramDataAccess;
		IdeaRS.OpenModel.CountryCode _countryCode;
		public static bool IsInstalled()
		{
			try
			{
				_ = new RamDataAccess1();
				return true;
			}
			catch (COMException)
			{
				return false;
			}
		}

		public RamDatabase(string path, IdeaRS.OpenModel.CountryCode countryCode)
		{
			_path = path;
			_ramDataAccess = new RamDataAccess1();
			_countryCode = countryCode;
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterType<ObjectFactory>().As<IObjectFactory>().SingleInstance();
			builder.RegisterType<MaterialFactory>().As<IMaterialFactory>().SingleInstance();
			builder.RegisterType<SectionFactory>().As<ISectionFactory>().SingleInstance();
			builder.RegisterType<SectionPropertiesConverter>().As<ISectionPropertiesConverter>().SingleInstance();
			builder.RegisterType<Geometry.Geometry>().As<Geometry.IGeometry>().SingleInstance();
			builder.RegisterType<SegmentFactory>().As<ISegmentFactory>().SingleInstance();

			builder.RegisterType<LoadsProvider>().As<ILoadsProvider>().SingleInstance();
			builder.RegisterType<RamModel>().AsSelf();

			builder.Register(x => (IModel)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IModel_INT));
			builder.Register(x => (IMemberData1)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IMemberData_INT));
			builder.Register(x => (IForces1)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IForces_INT));
			builder.Register(x => (IForces2)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IForces2_INT));

			_container = builder.Build();

			_dbIo = (IDBIO1)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IDBIO1_INT);
		}

		~RamDatabase()
		{
			Dispose(disposing: false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (_isOpen)
			{
				_dbIo?.CloseDatabase();
				_isOpen = false;
			}

			_disposed = true;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public IIdeaModel GetModel()
		{
			_dbIo.LoadDataBase(_path);
			_isOpen = true;
			return _container.Resolve<RamModel>(new List<Autofac.Core.Parameter> { new TypedParameter(_countryCode.GetType(), _countryCode) });
			//return _container.Resolve<RamModel>();
		}

		public void GetLastError(out string shortError, out string longError, out int errorId)
		{
			shortError = null;
			longError = null;
			errorId = 0;
			_ramDataAccess.GetLastError(ref shortError, ref longError, ref errorId);
		}
	}
}