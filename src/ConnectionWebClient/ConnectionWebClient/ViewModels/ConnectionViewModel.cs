using IdeaStatiCa.Api.Connection.Model;

namespace ConnectionWebClient.ViewModels
{
	public class ConnectionViewModel : ViewModelBase
	{
		string? _name;
		int _id;
		string? _identifier;

		public ConnectionViewModel(ConConnection con)
		{
			Id = con.Id;
			Name = con.Name;
			Identifier = con.Identifier;
		}


		public int Id
		{
			get { return _id; }
			set { SetProperty(ref _id, value); }
		}

		public string? Identifier
		{
			get { return _identifier; }
			set { SetProperty(ref _identifier, value); }
		}

		public string? Name
		{
			get { return _name; }
			set { SetProperty(ref _name, value); }
		}
	}
}
