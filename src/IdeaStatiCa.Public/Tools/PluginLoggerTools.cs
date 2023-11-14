using System.Diagnostics;

namespace IdeaStatiCa.Plugin
{
	public static class PluginLoggerTools
	{
		/// <summary>
		/// Log the content of a stack in debug severity
		/// </summary>
		/// <param name="logger"></param>
		public static void LogStack(this IPluginLogger logger)
		{
			StackTrace st = new StackTrace();
			for (int i = 0; i < st.FrameCount; i++)
			{
				logger.LogDebug($"{st.GetFrame(i).GetMethod().Name}");
			}
		}
	}
}
