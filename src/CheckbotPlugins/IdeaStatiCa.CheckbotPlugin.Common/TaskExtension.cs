using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.CheckbotPlugin.Common
{
	public static class TaskExtension
	{
		public static Task<R> Then<T, R>(this Task<T> task, Func<T, R> next)
		{
			return task.ContinueWith(t => next(t.Result));
		}
	}
}