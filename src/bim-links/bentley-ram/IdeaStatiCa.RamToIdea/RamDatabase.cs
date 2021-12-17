using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdea.Factories;
using RAMDATAACCESSLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea
{
	public class RamDatabase : IDisposable
	{
		private bool _disposed;

		private readonly IDBIO1 _dbIo;
		private readonly IModel _model;

		// RamDataAccess1 must be released after IDBIO1 so it cannot be a local variable
		private readonly RamDataAccess1 _ramDataAccess;

		public static RamDatabase Create(string path)
		{
			return new RamDatabase(path);
		}

		internal RamDatabase(string path)
		{
			_ramDataAccess = new RamDataAccess1();
			_dbIo = (IDBIO1)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IDBIO1_INT);
			_model = (IModel)_ramDataAccess.GetInterfacePointerByEnum(EINTERFACES.IModel_INT);

			_dbIo.LoadDataBase(@"C:\Users\dalibor.bacovsky\Downloads\PoCStructure.rss");
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

		private class RamModel : IIdeaModel
		{
			private readonly IModel _model;
			private readonly RamImporter _importer;
			private readonly List<IIdeaMember1D> _members;

			public RamModel(IModel model)
			{
				_model = model;
				_importer = new RamImporter(new ObjectFactory(_model), _model);
				_members = _importer.GetAllMembers().ToList();
			}

			public ISet<IIdeaLoading> GetLoads()
			{
				return new HashSet<IIdeaLoading>();
			}

			public ISet<IIdeaMember1D> GetMembers()
			{
				return _members.ToHashSet(); ;
			}

			public OriginSettings GetOriginSettings()
			{
				return new IdeaRS.OpenModel.OriginSettings()
				{
					CountryCode = IdeaRS.OpenModel.CountryCode.ECEN,
					ProjectName = _model.strProjectName
				}; ;
			}

			public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members)
			{
				nodes = new HashSet<IIdeaNode>();
				members = _members.ToHashSet();
			}
		}

		public ModelBIM GetModelBIM()
		{
			JsonPersistence persistence = new JsonPersistence();
			Project project = new Project(new NullLogger(), persistence, new DummyRestorer());

			var importer = BimImporter.BimImporter.Create(new RamModel(_model), project, new NullLogger());

			return importer.ImportConnections();
		}

		private class DummyRestorer : IObjectRestorer
		{
			public IIdeaPersistentObject Restore(IIdeaPersistenceToken token)
			{
				throw new NotImplementedException();
			}
		}
	}
}