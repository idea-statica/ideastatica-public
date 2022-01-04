using Dlubal.RSTAB8;
using IdeaStatiCa.Plugin;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace IdeaRstabPlugin
{
	[ComVisible(true)]
	public class CheckbotCommand : IExternalCommand
	{
		private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

		public CheckbotCommand()
		{
		}

		public void Execute(object Model, string Params)
		{
			if (!(Model is IModel rstabModel))
			{
				throw new ArgumentException($"{nameof(Model)} must be instance of {nameof(IModel)}.");
			}

			// RSTAB is blocked during execution of this method so we start a new thread
			// where we can do whatever we need to.
			Thread pluginThread = new Thread(PluginThread)
			{
				IsBackground = true
			};
			pluginThread.Start(rstabModel);
		}

		private async static void PluginThread(object param)
		{
			Debug.Fail("Plugin for RSTAB is starting");
			AppDomain domain = null;
			try
			{
				_logger.LogInformation("RSTAB Link started");

				string IdeaDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); //Get Idea location

				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve); //Assembly resolver for our current domain
				domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), new System.Security.Policy.Evidence(), IdeaDirectory, IdeaDirectory, true); //Create subdomain for our RSTAB Link
				_logger.LogInformation("RSTAB Link new Domain created");

				domain.AssemblyResolve += Domain_AssemblyResolve; //Create resolver for our subdomain

				Type assemblySandboxType = new ProxyDomain().GetType();
				var assemblySandbox = (ProxyDomain)domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(assemblySandboxType).FullName, assemblySandboxType.ToString()); //Initialize ProxyDomain class
				_logger.LogInformation("RSTAB Link ProxyDomain ready");

				string[] assembliesToLoad = new string[] {
						"System.Threading.Tasks.Extensions.dll",
						"Microsoft.Extensions.Configuration.Abstractions.dll",
						"Microsoft.Extensions.Primitives.dll",
						"Microsoft.Extensions.DependencyInjection.Abstractions.dll",
						"Newtonsoft.Json.dll",
						"Microsoft.Extensions.Options.dll",
						"Microsoft.Extensions.Logging.Abstractions.dll",
						"Microsoft.Extensions.DependencyInjection.dll",
						"Microsoft.Extensions.Configuration.Binder.dll",
						"Microsoft.Extensions.Logging.dll",
						"Microsoft.Extensions.DependencyModel.dll",
						"System.Runtime.CompilerServices.Unsafe.dll",
						"System.Buffers.dll",
						"System.Numerics.Vectors.dll",
						"System.Memory.dll"
					}; //ADD DLLS if needed

				foreach (var item in assembliesToLoad)
				{
					assemblySandbox.GetAssembly(System.IO.Path.Combine(IdeaDirectory, item)); //Load our assemblies, this is just for safety
				}
				_logger.LogInformation("RSTAB Link calling main Execute");
				await System.Threading.Tasks.Task.Run(() => assemblySandbox.Execute((IModel)param, IdeaDirectory)); //Execute main work 

				//using (BIMPluginHosting pluginHosting = new BIMPluginHosting(pluginFactory))
				//{
				//	await pluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), pluginFactory.WorkingDirectory);
				//}
			}
			catch (Exception e)
			{
				_logger.LogError("RSTAB link crashed", e);
			}
			finally
			{
//				if (domain != null)
//					AppDomain.Unload(domain);
				// Here we need to manually release the COM object or RSTAB will hang on exit.

				// This is most likely because the GC thinks (and is probably right)
				// that the COM object is held by some native code (RSTAB in this case)
				// so it never decreases the refcount. This prevents RSTAB from exiting properly.

				// decrease a refcount on IModel
				
			}
		}

		private static Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string IdeaDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			if (args.Name.Contains("System.Runtime.CompilerServices.Unsafe")) //Only missing DLL
			{
				return Assembly.LoadFrom(System.IO.Path.Combine(IdeaDirectory, "System.Runtime.CompilerServices.Unsafe.dll")); //Resolve our missing DLL
			}
			return null;
		}

		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			_logger.LogInformation($"RSTAB Link asked to Resolve {args.Name}");
			string IdeaDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (args.Name.Contains("IdeaRstabPlugin")) //Our DLL
				return Assembly.LoadFile(System.IO.Path.Combine(IdeaDirectory, "IdeaRstabPlugin.dll")); //Resolve our missing DLL
			_logger.LogInformation($"RSTAB Link failed to Resolve {args.Name}");
			return null;
		}

		public class ProxyDomain : MarshalByRefObject
		{
			public void GetAssembly(string AssemblyPath)
			{
				try
				{
					_logger.LogInformation($"ProxyDomain loading {AssemblyPath}");
					Assembly.LoadFrom(AssemblyPath); //Load assembly in Proxy Domain
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException(ex.Message, ex);
				}
			}
			public async void Execute(IModel model, string ideaDirectory) 
			{
				try
				{
					_logger.LogInformation($"ProxyDomain executing main code");
					PluginFactory pluginFactory = new PluginFactory(model, ideaDirectory, _logger);

					// It will be used for gRPC communication
					var bimPluginHosting = new BIMPluginHostingGrpc(pluginFactory, _logger);
					_logger.LogInformation($"ProxyDomain GRPC ready, calling RunAsync");
					//Run GRPC
					await bimPluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), pluginFactory.WorkingDirectory);
				}
				catch (Exception e)
				{
					_logger.LogError("ProxyDomain crashed", e);
				}
				finally
				{
					Marshal.ReleaseComObject(model);

					// house cleaning
					GC.Collect();
					GC.WaitForPendingFinalizers();

					_logger.LogInformation("RSTAB Link finished");
				}
			}
		}
	}
}