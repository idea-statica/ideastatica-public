using System;
using System.Reflection;

namespace CI.Common.MultiMethods
{
	using CI.Common.MultiMethods.Delegates;

	public delegate object MethodInvoker(object target, params object[] args);

	public static class MethodInvokerHelper
	{
		private static MethodInfo[] actionMaker = {
													  GetMethod("Action0"),
													  GetMethod("Action1"),
													  GetMethod("Action2"),
													  GetMethod("Action3"),
													  GetMethod("Action4"),
												  };

		private static MethodInfo[] funcMaker = {
													  GetMethod("Func0"),
													  GetMethod("Func1"),
													  GetMethod("Func2"),
													  GetMethod("Func3"),
													  GetMethod("Func4"),
												  };

		public static MethodInvoker CreateInvoker(MethodInfo info)
		{
			Constraints.CheckMethod(info);

			ParameterInfo[] parameters = info.GetParameters();

			bool action = info.ReturnType == typeof(void);

			Type[] types = null;

			if (action)
				types = new Type[parameters.Length + 1];
			else
				types = new Type[parameters.Length + 2];

			types[0] = info.DeclaringType;

			for (int i = 0; i < parameters.Length; ++i)
			{
				types[i + 1] = parameters[i].ParameterType;
			}

			if (action)
			{
				return (MethodInvoker)actionMaker[parameters.Length]
					.MakeGenericMethod(types).Invoke(null, new object[] { info });
			}
			else
			{
				types[parameters.Length + 1] = info.ReturnType;

				return (MethodInvoker)funcMaker[parameters.Length]
					.MakeGenericMethod(types).Invoke(null, new object[] { info });
			}
		}

		private static MethodInfo GetMethod(string name)
		{
			return typeof(MethodInvokerHelper)
				.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
		}

		#region Action Invokers

		private static MethodInvoker Action0<T>(MethodInfo info)
		{
			Action<T> action = Delegate.CreateDelegate(
				typeof(Action<T>), null, info) as Action<T>;

			return (target, args) =>
			{
				action((T)target); return null;
			};
		}

		private static MethodInvoker Action1<T, A1>(MethodInfo info)
		{
			Action<T, A1> action = Delegate.CreateDelegate(
				typeof(Action<T, A1>), null, info) as Action<T, A1>;

			return (target, args) =>
			{
				action((T)target, (A1)args[0]); return null;
			};
		}

		private static MethodInvoker Action2<T, A1, A2>(MethodInfo info)
		{
			Action<T, A1, A2> action = Delegate.CreateDelegate(
				typeof(Action<T, A1, A2>), null, info) as Action<T, A1, A2>;

			return (target, args) =>
			{
				action((T)target, (A1)args[0], (A2)args[1]); return null;
			};
		}

		private static MethodInvoker Action3<T, A1, A2, A3>(MethodInfo info)
		{
			Action<T, A1, A2, A3> action = Delegate.CreateDelegate(
				typeof(Action<T, A1, A2, A3>), null, info) as Action<T, A1, A2, A3>;

			return (target, args) =>
			{
				action((T)target, (A1)args[0], (A2)args[1], (A3)args[2]); return null;
			};
		}

		private static MethodInvoker Action4<T, A1, A2, A3, A4>(MethodInfo info)
		{
			Action<T, A1, A2, A3, A4> action = Delegate.CreateDelegate(
				typeof(Action<T, A1, A2, A3, A4>), null, info) as Action<T, A1, A2, A3, A4>;

			return (target, args) =>
			{
				action((T)target, (A1)args[0], (A2)args[1], (A3)args[2], (A4)args[3]);
				return null;
			};
		}

		#endregion

		#region Func Invokers

		private static MethodInvoker Func0<T, R>(MethodInfo info)
		{
			Func<T, R> func = Delegate.CreateDelegate(
				typeof(Func<T, R>), null, info) as Func<T, R>;

			return (target, args) => func((T)target);
		}

		private static MethodInvoker Func1<T, A1, R>(MethodInfo info)
		{
			Func<T, A1, R> func = Delegate.CreateDelegate(
				typeof(Func<T, A1, R>), null, info) as Func<T, A1, R>;

			return (target, args) => func((T)target, (A1)args[0]);
		}

		private static MethodInvoker Func2<T, A1, A2, R>(MethodInfo info)
		{
			Func<T, A1, A2, R> func = Delegate.CreateDelegate(
				typeof(Func<T, A1, A2, R>), null, info) as Func<T, A1, A2, R>;

			return (target, args) =>
				func((T)target, (A1)args[0], (A2)args[1]);
		}

		private static MethodInvoker Func3<T, A1, A2, A3, R>(MethodInfo info)
		{
			Func<T, A1, A2, A3, R> func = Delegate.CreateDelegate(
				typeof(Func<T, A1, A2, A3, R>), null, info) as Func<T, A1, A2, A3, R>;

			return (target, args) =>
				func((T)target, (A1)args[0], (A2)args[1], (A3)args[2]);
		}

		private static MethodInvoker Func4<T, A1, A2, A3, A4, R>(MethodInfo info)
		{
			Func<T, A1, A2, A3, A4, R> func = Delegate.CreateDelegate(
				typeof(Func<T, A1, A2, A3, A4, R>), null, info) as Func<T, A1, A2, A3, A4, R>;

			return (target, args) =>
				func((T)target, (A1)args[0], (A2)args[1], (A3)args[2], (A4)args[3]);
		}

		#endregion
	}
}
