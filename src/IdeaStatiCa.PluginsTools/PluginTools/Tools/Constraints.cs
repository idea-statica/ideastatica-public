using System;
using System.Reflection;

namespace CI.Common.MultiMethods
{
	public class Constraints
	{
		public const int MaxParameterCount = 4;

		public static void CheckMethod(MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();

			if (parameters.Length > MaxParameterCount)
			{
				throw new ArgumentException(
					string.Format("{0}.{1} only upto {2} parameters are supported",
						method.DeclaringType.Name, method, MaxParameterCount));
			}

			if (method.ContainsGenericParameters)
			{
				throw new ArgumentException(
					string.Format("{0}.{1} generic methods are not supported",
						method.DeclaringType.Name, method));
			}

			foreach (ParameterInfo pi in parameters)
			{
				if (pi.ParameterType.IsByRef)
				{
					throw new ArgumentException(
						string.Format("{0}.{1} out and ref arguments are not supported",
							method.DeclaringType.Name, method));
				}
			}
		}

		public static void CheckMultiMethod(MethodInfo method)
		{
			if (method.GetParameters().Length == 0)
			{
				throw new ArgumentException(
					string.Format("{0}.{1} multimethods should take atleast one parameter",
						method.DeclaringType.Name, method));
			}

			if (method.IsStatic) // does it makes sense to support?
			{
				throw new ArgumentException(
					string.Format("{0}.{1} static multimethods are not supported",
						method.DeclaringType.Name, method));
			}

			CheckMethod(method);
		}

		public static void CheckImplementationType(Type type)
		{
			if (type.ContainsGenericParameters)
			{
				throw new ArgumentException(
					string.Format("{0} open generic types are not supported", type));
			}
		}
	}
}
