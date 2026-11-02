namespace IdeaStatiCa.Public
{
	/// <summary>
	/// Interface used to report the user events with parameters. See many derived classes that construct the uniform arguments for individual user events
	/// </summary>
	public interface IIdeaUserEvent
	{
		/// <summary>
		/// Event category for the 'ec' Google Analytics parameter. Use one of the predefined categories. Must not be null.
		/// </summary>
		string EventCategory { get; }

		/// <summary>
		/// Unique event type identifier, lower case, underscore-seprated string. For example "navigation_click", "cb_publish", etc. 
		/// </summary>
		/// <remarks>Used for cross platform correlation of the event data in the Google Analuytics 4</remarks>
		string EventName { get; }

		/// <summary>
		/// Event action for the 'ea' Google Analytics parameter. Use the verb form, for example "Started application", "application started", 
		/// "clicked ribbon tab Design", "clicked ribbon command Calculate", "opened connection project", etc.
		/// </summary>
		string EventAction { get; }

		/// <summary>
		/// Event label for the 'el' Google Analytics parameter that specifies the additional event dimension. Can contain name of button, type of project, 
		/// etc. Can be null.
		/// </summary>
		string EventLabel { get; }

		/// <summary>
		/// Event value for the 'ev' Google Analytics parameter that specifies the numeric value of the event. Can contain the projects or connections count
		/// or anything else that might be interesting to aggregate in the reports. Can be 0, means no value.
		/// </summary>
		int EventValue { get; }
	}
}
