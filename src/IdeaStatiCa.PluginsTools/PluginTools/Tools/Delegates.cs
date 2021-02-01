namespace CI.Common.MultiMethods
{
	public static class MultiMethod
	{
		public delegate DispatchResult Action<A1>(A1 a1);
		public delegate DispatchResult Action<A1, A2>(A1 a1, A2 a2);
		public delegate DispatchResult Action<A1, A2, A3>(A1 a1, A2 a2, A3 a3);
		public delegate DispatchResult Action<A1, A2, A3, A4>(A1 a1, A2 a2, A3 a3, A4 a4);

		public delegate DispatchResult<R> Func<A1, R>(A1 a1);
		public delegate DispatchResult<R> Func<A1, A2, R>(A1 a1, A2 a2);
		public delegate DispatchResult<R> Func<A1, A2, A3, R>(A1 a1, A2 a2, A3 a3);
		public delegate DispatchResult<R> Func<A1, A2, A3, A4, R>(A1 a1, A2 a2, A3 a3, A4 a4);
	}

	namespace Delegates
	{
		public delegate void Action<A1>(A1 a1);
		public delegate void Action<A1, A2>(A1 a1, A2 a2);
		public delegate void Action<A1, A2, A3>(A1 a1, A2 a2, A3 a3);
		public delegate void Action<A1, A2, A3, A4>(A1 a1, A2 a2, A3 a3, A4 a4);
		public delegate void Action<A1, A2, A3, A4, A5>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5);

		public delegate R Func<A1, R>(A1 a1);
		public delegate R Func<A1, A2, R>(A1 a1, A2 a2);
		public delegate R Func<A1, A2, A3, R>(A1 a1, A2 a2, A3 a3);
		public delegate R Func<A1, A2, A3, A4, R>(A1 a1, A2 a2, A3 a3, A4 a4);
		public delegate R Func<A1, A2, A3, A4, A5, R>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5);
	}
}