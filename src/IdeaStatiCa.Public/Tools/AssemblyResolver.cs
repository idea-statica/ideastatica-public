using System;
using System.IO;
using System.Reflection;

namespace IdeaStatiCa.Public.Tools
{
	public static class AssemblyResolver
	{
		/// <summary>
		/// Because the SAP's assembly resolve handler is not implemented properly (it return non-null value even when it can't load the assembly),
		/// we have to register our handler before it.
		/// </summary>
		public static void ReplaceAssemblyResolve()
		{
			// Because the SAP's assembly resolve handler is not implemented properly (it return non-null value even when it can't load the assembly),
			// we have to register our handler before it.

			// AppDomain.AssemblyResolve internally references _AssemblyResolve (see .NET source code);
			FieldInfo fieldInfo = typeof(AppDomain).GetField("_AssemblyResolve", BindingFlags.NonPublic | BindingFlags.Instance);

			ResolveEventHandler resolveEvent = (ResolveEventHandler)fieldInfo.GetValue(AppDomain.CurrentDomain); // Original event
			ResolveEventHandler myResolveEvent = new ResolveEventHandler(Domain_AssemblyResolve); // Our event

			// Add all existing handlers to the our event
			foreach (Delegate handler in resolveEvent.GetInvocationList())
			{
				myResolveEvent += (ResolveEventHandler)handler;
			}

			// Replace the original event with ours in AppDomain.CurrentDomain
			fieldInfo.SetValue(AppDomain.CurrentDomain, myResolveEvent);
		}

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
