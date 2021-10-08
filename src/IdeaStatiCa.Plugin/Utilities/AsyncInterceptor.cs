using Castle.DynamicProxy;
using Nito.AsyncEx.Synchronous;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Utilities
{
	/// <summary>
	/// Interface mock interceptor supporitn async interception.
	/// </summary>
	abstract class AsyncInterceptor : IInterceptor
	{
		class TaskCompletionSourceMethodMarkerAttribute : Attribute
		{

		}

		private static readonly MethodInfo _taskCompletionSourceMethod = typeof(AsyncInterceptor)
			.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
			.Single(x => x.GetCustomAttributes(typeof(TaskCompletionSourceMethodMarkerAttribute)).Any());


		protected virtual Task<Object> InterceptAsync(Object target, MethodBase method, object[] arguments, Func<Task<Object>> proceed)
		{
			return proceed();
		}

		protected virtual Task<object> Intercept(Object target, MethodBase method, object[] arguments, Action proceed)
		{
			return null;
		}

		[TaskCompletionSourceMethodMarker]
		Task<TResult> TaskCompletionSource<TResult>(IInvocation invocation)
		{
			var tcs = new TaskCompletionSource<TResult>();

			var task = InterceptAsync(invocation.InvocationTarget, invocation.Method, invocation.Arguments, () =>
			{
				var task2 = (Task)invocation.Method.Invoke(invocation.InvocationTarget, invocation.Arguments);
				var tcs2 = new TaskCompletionSource<Object>();
				task2.ContinueWith(x =>
				{
					if (x.IsFaulted)
					{
						tcs2.SetException(x.Exception);
						return;
					}
					dynamic dynamicTask = task2;
					Object result = dynamicTask.Result;
					tcs2.SetResult(result);
				});
				return tcs2.Task;
			});

			task.ContinueWith(x =>
			{
				if (x.IsFaulted)
				{
					tcs.SetException(x.Exception);
					return;
				}

				tcs.SetResult((TResult)x.Result);
			});

			return tcs.Task;
		}
		void IInterceptor.Intercept(IInvocation invocation)
		{
			if (!typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
			{
				var returnValue = Intercept(invocation.InvocationTarget, invocation.Method, invocation.Arguments, invocation.Proceed).WaitAndUnwrapException();
				invocation.ReturnValue = returnValue;
				return;
			}
			var returnType = invocation.Method.ReturnType.IsGenericType ? invocation.Method.ReturnType.GetGenericArguments()[0] : typeof(object);
			var method = _taskCompletionSourceMethod.MakeGenericMethod(returnType);
			invocation.ReturnValue = method.Invoke(this, new object[] { invocation });
		}
	}
}
