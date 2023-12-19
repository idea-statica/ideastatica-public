using IdeaStatiCa.BimApiLink;

namespace IdeaStatiCa.TeklaStructuresPlugin.UserData
{
	internal class ImportParameters
	{
		public GlobalAxesOrientation GlobalAxesOrientation { get; set; }

		public ImportParameters(GlobalAxesOrientation globalAxesOrientation)
		{
			GlobalAxesOrientation = globalAxesOrientation;
		}
	}

	internal class UserDataSource : IBimUserDataSource
	{
		private readonly ModelClient _model;

		public UserDataSource(ModelClient model)
		{
			_model = model;
		}

		public object GetUserData()
		{

			return new ImportParameters(GetGlobalAxes());
		}

		private static GlobalAxesOrientation GetGlobalAxes()
		{
			return GlobalAxesOrientation.ZUp;
		}
	}
}
