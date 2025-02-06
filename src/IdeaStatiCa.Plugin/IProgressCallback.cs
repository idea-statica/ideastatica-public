using System;


namespace IdeaStatiCa.Plugin
{
	
	public interface IProgressCallback
	{
		/// <summary>
		/// Method is called when an exception occurs on the server.
		/// </summary>
		/// <param name="function">Name of the method.</param>
		/// <param name="exception">Details of the exception</param>
		void ExceptionMessage(string function, Exception exception);

		/// <summary>
		/// infomation about progress of event.
		/// </summary>
		/// <param name="percent">percentage of event status</param>
		/// <param name="message">message</param>
		void ProgressMessage(double percent, string message);

		/// <summary>
		/// infomation about progress of event.
		/// </summary>
		/// <param name="percent">percentage of event status</param>
		/// <param name="message">message</param>
		/// <param name="iteration">Number of the iteration (FEM solver steps)</param>
		void IterationMessage(double percent, string message, string iteration);
	}
}