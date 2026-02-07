using IdeaStatiCa.Api.Connection.Model;

namespace ConApiWpfClientApp.ViewModels
{
	/// <summary>
	/// View model representing a single connection in a project.
	/// Wraps a <see cref="ConConnection"/> model for data binding.
	/// </summary>
	public class ConnectionViewModel : ViewModelBase
	{
		string? _name;
		int _id;
		string? _identifier;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionViewModel"/> class from a <see cref="ConConnection"/> model.
		/// </summary>
		/// <param name="con">The connection model to wrap.</param>
		public ConnectionViewModel(ConConnection con)
		{
			Id = con.Id;
			Name = con.Name;
			Identifier = con.Identifier;
		}

		/// <summary>
		/// Gets or sets the numeric identifier of the connection.
		/// </summary>
		public int Id
		{
			get { return _id; }
			set { SetProperty(ref _id, value); }
		}

		/// <summary>
		/// Gets or sets the unique string identifier of the connection.
		/// </summary>
		public string? Identifier
		{
			get { return _identifier; }
			set { SetProperty(ref _identifier, value); }
		}

		/// <summary>
		/// Gets or sets the display name of the connection.
		/// </summary>
		public string? Name
		{
			get { return _name; }
			set { SetProperty(ref _name, value); }
		}
	}
}
