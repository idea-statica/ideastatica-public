using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Logger that does nothing.
	/// </summary>
	public class NullLogger : IPluginLogger
	{
		public void LogDebug(string message)
		{

		}

		public void LogError(string message, object parameter)
		{
			
		}

		public void LogInformation(string message)
		{
			
		}

		public void LogTrace(string message)
		{

		}
	}
}
