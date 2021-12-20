using Autofac;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Sections;
using IdeaStatiCa.RamToIdea.Utilities;
using RAMDATAACCESSLib;
using System;
using System.Runtime.InteropServices;

namespace IdeaStatiCa.RamToIdea
{
	public class RamDatabase : IDisposable
	{
		private bool _disposed;

		private readonly IDBIO1 _dbIo;

		// RamDataAccess1 must be released after IDBIO1 so it cannot be a local variable
		private readonly RamDataAccess1 _ramDataAccess;

		private readonly IContainer _container;

		public static RamDatabase Create(string path)
		{
			return new RamDatabase(path);
		}

		internal RamDatabase(string path)
		{
			_ramDataAccess = new RamDataAccess1();

			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterType<ObjectFactory>().As<IObjectFactory>().SingleInstance();
			builder.RegisterType<IMaterialFactory>().As<IMaterialFactory>().SingleInstance();
			builder.RegisterType<SectionFactory>().As<ISectionFactory>().SingleInstance();

			builder.RegisterType<RamModel>().FindConstructorsWith(new AllConstructorFinder()).AsSelf();

			builder.Register(x => (IModel)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IModel_INT));
			builder.Register(x => (IMemberData1)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IMemberData_INT));

			_container = builder.Build();

			_dbIo = (IDBIO1)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IDBIO1_INT);

			try
			{
				_dbIo.LoadDataBase(path);
			}
			catch (COMException)
			{
				string a = null, b = null;
				int c = 0;
				_ramDataAccess.GetLastError(ref a, ref b, ref c);
			}
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

			if (disposing)
			{
				// TODO: dispose managed state (managed objects)
			}

			_dbIo?.CloseDatabase();

			_disposed = true;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public RamModel GetModel()
		{
			//JsonPersistence persistence = new JsonPersistence();
			//Project project = new Project(new NullLogger(), persistence);

			//var importer = BimImporter.BimImporter.Create(new RamModel(_model), project, new NullLogger());

			//return importer.ImportConnections();
			return _container.Resolve<RamModel>();
		}
	}
}