using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace yjk.Helpers
{
	public static class YjkDispatcher
	{
		public static Dispatcher Dispatcher { get; set; }

		public static void Invoke(Action action)
		{
			if (Dispatcher.CheckAccess())
				action();
			else
				Dispatcher.Invoke(action);
		}
	}
}
