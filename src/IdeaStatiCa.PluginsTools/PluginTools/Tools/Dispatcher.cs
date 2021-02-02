using System;
using System.ComponentModel;
using System.Reflection;

namespace CI.Common.MultiMethods
{
	using CI.Common.MultiMethods.Delegates;

	public class Dispatcher
	{
		private Type _implementation;
		private MethodInfo _multiMethod;
		private DispatchTable _dispatchTable;

		private static BindingFlags DefaultBindingFlags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private Dispatcher(Type implementation, MethodInfo multiMethod, bool is32BitPlatform)
		{
			Constraints.CheckImplementationType(implementation);
			Constraints.CheckMultiMethod(multiMethod);

			int argumentCount = multiMethod.GetParameters().Length;

			DispatchTable dispatchTable = new DispatchTable(argumentCount, is32BitPlatform);

			Type currentType = implementation;

			while (currentType != typeof(object))
			{
				MethodInfo[] methods = currentType.GetMethods(
					DefaultBindingFlags | BindingFlags.DeclaredOnly);

				foreach (MethodInfo candidate in methods)
				{
					if (candidate.MethodHandle != multiMethod.MethodHandle
						&& candidate.Name == multiMethod.Name
						&& candidate.GetParameters().Length == argumentCount
						&& !candidate.ContainsGenericParameters
						&& !HasInterfaceOrAbstractParameter(candidate))
					{
						Type[] types = GetParameterTypes(candidate);

						//System.Diagnostics.Debug.WriteLine(
						//    string.Format("{0}.{1}", candidate.DeclaringType, candidate));

						dispatchTable.InternalAdd(types,
							MethodInvokerHelper.CreateInvoker(candidate));
					}
				}

				currentType = currentType.BaseType;
			}

			_implementation = implementation;
			_multiMethod = multiMethod;
			_dispatchTable = dispatchTable;
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Dispatcher Create(Type implementation, MethodInfo multiMethod, bool is32BitPlatform)
		{
			return new Dispatcher(implementation, multiMethod, is32BitPlatform);
		}

		public static Dispatcher For(Type implementation, MethodInfo multiMethod)
		{
			return DispatcherCache.ForType(implementation).DispatcherFor(multiMethod);
		}

		public int DispatchTableSize
		{
			get { return _dispatchTable.Size; }
		}

		public DispatchResult Dispatch(object target, params object[] args)
		{
			if (TargetOrArgumentIsNull(target, args))
			{
				return new DispatchResult(DispatchStatus.NoMatch);
			}

			MethodInvoker method = _dispatchTable.Match(args);

			if (method != null)
			{
				return InvokeMethod(method, target, args);
			}

			try
			{
				MethodInfo info = TryReflectionMatch(args);

				if (info != null && info.MethodHandle != _multiMethod.MethodHandle) // always use MethodInfo.MethodHandle to compare!
				{
					method = MethodInvokerHelper.CreateInvoker(info);
				}
				else
				{
					method = delegate
					{
						return new DispatchResult(DispatchStatus.NoMatch);
					};
				}
			}
			catch (AmbiguousMatchException)
			{
				method = delegate
				{
					return new DispatchResult(DispatchStatus.AmbiguousMatch);
				};
			}

			_dispatchTable.Add(args, method);

			DispatchResult result = InvokeMethod(method, target, args);

			result.IsDynamicInvoke = true;

			return result;
		}

		#region Syntactic Sugar

		public static MultiMethod.Action<A1> Action<A1>(Action<A1> action)
		{
			object target = action.Target;
			Dispatcher dispatcher = Dispatcher.For(target.GetType(), action.Method);
			return (a1) => dispatcher.Dispatch(target, a1);
		}

		public static MultiMethod.Action<A1, A2> Action<A1, A2>(Action<A1, A2> action)
		{
			object target = action.Target;
			Dispatcher dispatcher = Dispatcher.For(target.GetType(), action.Method);
			return (a1, a2) => dispatcher.Dispatch(target, a1, a2);
		}

		public static MultiMethod.Action<A1, A2, A3> Action<A1, A2, A3>(Action<A1, A2, A3> action)
		{
			object target = action.Target;
			Dispatcher dispatcher = Dispatcher.For(target.GetType(), action.Method);
			return (a1, a2, a3) => dispatcher.Dispatch(target, a1, a2, a3);
		}

		public static MultiMethod.Action<A1, A2, A3, A4> Action<A1, A2, A3, A4>(Action<A1, A2, A3, A4> action)
		{
			object target = action.Target;
			Dispatcher dispatcher = Dispatcher.For(target.GetType(), action.Method);
			return (a1, a2, a3, a4) => dispatcher.Dispatch(target, a1, a2, a3, a4);
		}

		public static MultiMethod.Func<A1, R> Func<A1, R>(Func<A1, R> func)
		{
			object target = func.Target;
			Dispatcher dispatcher = Dispatcher.For(target.GetType(), func.Method);
			return (a1) => dispatcher.Dispatch(target, a1).Typed<R>();
		}

		public static MultiMethod.Func<A1, A2, R> Func<A1, A2, R>(Func<A1, A2, R> func)
		{
			object target = func.Target;
			Dispatcher dispatcher = Dispatcher.For(target.GetType(), func.Method);
			return (a1, a2) => dispatcher.Dispatch(target, a1, a2).Typed<R>();
		}

		public static MultiMethod.Func<A1, A2, A3, R> Func<A1, A2, A3, R>(Func<A1, A2, A3, R> func)
		{
			object target = func.Target;
			Dispatcher dispatcher = Dispatcher.For(target.GetType(), func.Method);
			return (a1, a2, a3) => dispatcher.Dispatch(target, a1, a2, a3).Typed<R>();
		}

		public static MultiMethod.Func<A1, A2, A3, A4, R> Func<A1, A2, A3, A4, R>(Func<A1, A2, A3, A4, R> func)
		{
			object target = func.Target;
			Dispatcher dispatcher = Dispatcher.For(target.GetType(), func.Method);
			return (a1, a2, a3, a4) => dispatcher.Dispatch(target, a1, a2, a3, a4).Typed<R>();
		}

		#endregion

		#region Private

		private DispatchResult InvokeMethod(MethodInvoker method, object target, object[] args)
		{
			object returnValue = method(target, args);

			DispatchResult result = new DispatchResult(DispatchStatus.Success);

			if (returnValue != null && returnValue is DispatchResult) // we registered a NoMatch / AmbiguousMatch
			{
				result = (DispatchResult)returnValue;
			}
			else
			{
				result.ReturnValue = returnValue;
			}

			return result;
		}

		private bool TargetOrArgumentIsNull(object target, object[] args)
		{
			if (target == null)
				return true;

			foreach (object arg in args)
			{
				if (arg == null)
					return true;
			}

			return false;
		}

		private MethodInfo TryReflectionMatch(object[] args)
		{
			Type[] types = new Type[args.Length];

			for (int i = 0; i < types.Length; ++i)
			{
				types[i] = args[i].GetType();
			}

			return _implementation.GetMethod(
				_multiMethod.Name, DefaultBindingFlags, Type.DefaultBinder, types, null);
		}

		private static bool HasInterfaceOrAbstractParameter(MethodInfo method)
		{
			foreach (ParameterInfo p in method.GetParameters())
			{
				if (p.ParameterType.IsInterface || p.ParameterType.IsAbstract)
				{
					return true;
				}
			}

			return false;
		}

		private static Type[] GetParameterTypes(MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();

			Type[] types = new Type[parameters.Length];

			for (int i = 0; i < parameters.Length; ++i)
			{
				types[i] = parameters[i].ParameterType;
			}

			return types;
		}

		#endregion
	}
}
