using Castle.DynamicProxy;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.Utilities;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	/// <summary>
	/// Creates a dynamic instance of the interface implemented on server to proxy the calls from grpc client service to grpc server.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GrpcReflectionServiceFactory
	{
		public static T CreateInstance<T>(IMethodInvoker methodInvoker) where T : class
		{
			ProxyGenerator generator = new ProxyGenerator();
			return generator.CreateInterfaceProxyWithoutTarget<T>(new ReflectionServiceInterceptor(methodInvoker));
		}

		class OpCodeContainer
		{
			IPluginLogger ideaLogger;
			public OpCode? code;
			byte data;

			public OpCodeContainer(byte opCode, IPluginLogger logger)
			{
				this.ideaLogger = logger ?? new Plugin.NullLogger();
				data = opCode;
				try
				{
					code = (OpCode)typeof(OpCodes).GetFields().First(t => ((OpCode)(t.GetValue(null))).Value == opCode).GetValue(null);
				}
				catch (Exception ex)
				{
					ideaLogger.LogDebug("Fields by opcode was not find", ex);
				}
			}
		}
	}

	internal class ReflectionServiceInterceptor : AsyncInterceptor
	{
		private IMethodInvoker methodInvoker;

		public ReflectionServiceInterceptor(IMethodInvoker methodInvoker)
		{
			if (methodInvoker == null)
			{
				throw new ArgumentNullException("Client cannot be null");
			}

			this.methodInvoker = methodInvoker;
		}

		protected override async Task<Object> InterceptAsync(object target, MethodBase method, object[] arguments, Type returnType, Func<Task<object>> proceed)
		{
			try
			{
				var returnValue = methodInvoker.InvokeMethod<object>(method.Name, returnType, arguments);

				await Task.CompletedTask;

				return returnValue;
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Grpc method couldn't be invoked. Check if your server implemnents specified interface", e);
			}
		}

		protected override async Task<object> Intercept(object target, MethodBase method, object[] arguments, Type returnType, Action proceed)
		{
			try
			{
				await Task.CompletedTask;
				return methodInvoker.InvokeMethod<object>(method.Name, returnType, arguments);
			}
			catch(ArgumentException e)
			{
				// rethrow exeption which were handled by a plugin
				throw e.InnerException;
			}
			catch (Exception e)
			{
				// this is a general error
				throw new InvalidOperationException("Grpc method couldn't be invoked. Check if your server implemnents specified interface", e);
			}
		}
	}
}
