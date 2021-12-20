using Autofac;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;
using System;

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
			builder.RegisterType<RamSectionProvider>().As<IRamSectionProvider>().SingleInstance();

			builder.RegisterType<RamModel>();

			builder.RegisterInstance((IModel)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IModel_INT));
			builder.RegisterInstance((IModelData1)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IModelData_INT));

			_container = builder.Build();

			_dbIo = (IDBIO1)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IDBIO1_INT);
			_dbIo.LoadDataBase(path);
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