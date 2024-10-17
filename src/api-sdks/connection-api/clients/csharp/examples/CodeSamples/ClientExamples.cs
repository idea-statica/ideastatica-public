using IdeaStatiCa.ConnectionApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
	/// <summary>
	/// Class which holds all the static example methods to be selected and run by the user.
	/// </summary>
	public static partial class ClientExamples
	{
		/// <summary>
		/// Finds all the Methods which take only the <see cref="ConnectionApiClient"/> as a parameter. 
		/// </summary>
		/// <returns>List of example methods</returns>
		public static MethodInfo[] GetExampleMethods()
		{
			return typeof(ClientExamples)
				.GetMethods(BindingFlags.Public | BindingFlags.Static)
				.Where(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(ConnectionApiClient))
				.Where(m => typeof(Task).IsAssignableFrom(m.ReturnType)) // Ensure it returns a Task
				.ToArray();
		}

		/// <summary>
		/// Get a folder directory on desktop which can be used by the example. A unique Example Name should be provided.
		/// </summary>
		/// <param name="exampleName"></param>
		/// <returns>An existing or new directory to be used by the outputted example files </returns>
		public static string GetExampleFolderPathOnDesktop(string exampleName)
		{
			string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			string folderPath = Path.Combine(desktopPath, "ConApiSamples_" + exampleName);

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			return folderPath;
		}
	}
}