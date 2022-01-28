using System;
using System.Reflection;

namespace IdeaStatiCa.Public.Tools
{
	public static class AssemblyResolver
	{
		/// <summary>
		/// AssemblyResolve for GRPC 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string IdeaDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			if (args.Name.Contains("System.Runtime.CompilerServices.Unsafe")) //Only missing DLL
			{
				return Assembly.LoadFrom(System.IO.Path.Combine(IdeaDirectory, "System.Runtime.CompilerServices.Unsafe.dll")); //Resolve our missing DLL
			}
			return null;
		}
	}
}
