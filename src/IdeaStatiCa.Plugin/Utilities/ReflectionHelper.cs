using IdeaStatiCa.Plugin.Grpc.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Utilities
{
	/// <summary>
	/// Helper for reflection methods used by gRPC.
	/// </summary>
	public static class ReflectionHelper
	{
		static readonly ConcurrentDictionary<Type, bool> IsSimpleTypeCache = new ConcurrentDictionary<System.Type, bool>();

		static ReflectionHelper()
		{
			string curAssPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Assembly.LoadFrom(Path.Combine(curAssPath, "IdeaRS.OpenModel.dll"));
		}

		/// <summary>
		/// Gets a gRPC message for invoking method.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="methodName"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static IEnumerable<GrpcReflectionArgument> GetMethodInvokeArguments(params object[] args)
		{
			var parsedArguments = new List<GrpcReflectionArgument>();

			foreach (var arg in args)
			{
				var argType = arg.GetType();
				var parsedArg = new GrpcReflectionArgument(argType.ToString(), JsonConvert.SerializeObject(arg));

				parsedArguments.Add(parsedArg);
			}

			return parsedArguments;
		}

		/// <summary>
		/// Invokes method based on parameters passed from grpc message.
		/// </summary>
		/// <returns></returns>
		public static async Task<object> InvokeMethodFromGrpc(object instance, string methodName, IEnumerable<GrpcReflectionArgument> arguments)
		{
			var instanceType = instance.GetType();
			var methods = instanceType.GetMethods();

			// get calling parameters            
			var method = methods.FirstOrDefault(w => w.Name == methodName);

			// Send back an exception if message is not present.
			if (method == null)
			{
				throw new ApplicationException($"Method {methodName} not found on {instanceType.Name}");
			}

			var parsedArguments = new List<object>();

			// Check whether all types used by method are loaded, if yes, deserialize and put them into values. 
			foreach (var arg in arguments)
			{
				var targetArgType = GetLoadedType(arg.ParameterType);

				if (targetArgType == null)
				{
					throw new ApplicationException($"Target type {arg.ParameterType} was not loaded in assembly.");
				}

				var deserializedArg = JsonConvert.DeserializeObject(arg.Value.ToString(), targetArgType);

				parsedArguments.Add(deserializedArg);
			}

			// determine whether method is async 
			var isAwaitable = method.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;

			object invokeResult = null;
			if (isAwaitable)
			{
				// If async then check for return types
				if (method.ReturnType.IsGenericType)
				{
					invokeResult = (object)await (dynamic)method.Invoke(instance, parsedArguments.ToArray());
				}
				else
				{
					await (Task)method.Invoke(instance, parsedArguments.ToArray());
				}
			}
			else
			{
				// If not async chceck whether it returns void or value.
				if (method.ReturnType == typeof(void))
				{
					method.Invoke(instance, parsedArguments.ToArray());
				}
				else
				{
					invokeResult = method.Invoke(instance, parsedArguments.ToArray());
				}
			}

			return invokeResult;
		}

		/// <summary>
		/// Determine whether the target type is simple type.
		/// </summary>
		/// <param name="type">Type to check</param>
		/// <returns></returns>
		public static bool IsSimpleType(Type type)
		{
			return IsSimpleTypeCache.GetOrAdd(type, t =>
				type.IsPrimitive ||
				type.IsEnum ||
				type == typeof(string) ||
				type == typeof(decimal) ||
				type == typeof(DateTime) ||
				type == typeof(DateTimeOffset) ||
				type == typeof(TimeSpan) ||
				type == typeof(Guid) ||
				IsNullableSimpleType(type));
		}

		static bool IsNullableSimpleType(Type t)
		{
			var underlyingType = Nullable.GetUnderlyingType(t);
			return underlyingType != null && IsSimpleType(underlyingType);
		}

		/// <summary>
		/// TODO - how to get the type from the string <paramref name="fullName"/> correctly ?
		/// </summary>
		/// <param name="fullName"></param>
		/// <returns></returns>
		public static Type GetLoadedType(string fullName)
		{
			// Do we need to iterate all types in all loaded assemblies ?
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (a.ManifestModule.Name == "IdeaRS.OpenModel.dll" || a.ManifestModule.Name == "IdeaStatiCa.Plugin.dll")
				{
					try
					{
						foreach (Type t in a.GetTypes())
						{
							if (t.FullName == fullName)
							{
								return t;
							}
						}
					}
					catch (System.Reflection.ReflectionTypeLoadException)
					{
						//some assembly from revit cannot be loaded and GetTypes throw this ex
						continue;
					}
				}
			}

			return Type.GetType(fullName);
		}
	}
}
