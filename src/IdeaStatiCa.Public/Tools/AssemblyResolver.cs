using System;
using System.IO;
using System.Reflection;

namespace IdeaStatiCa.Public.Tools
{
	public static class AssemblyResolver
	{
		/// <summary>
		/// AssemblyResolve forIDEA StatiCa 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			AssemblyName assemblyName = new AssemblyName(args.Name);

			string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string path;

			if (string.IsNullOrEmpty(assemblyName.CultureName))
			{
				path = Path.Combine(directory, assemblyName.Name + ".dll");
			}
			else
			{
				path = Path.Combine(directory, assemblyName.CultureName, assemblyName.Name + ".dll");
			}

			if (File.Exists(path))
			{
				return Assembly.LoadFrom(path);
			}

			return null;
		}
	}
}
