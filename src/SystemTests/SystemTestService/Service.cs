using System;

namespace SystemTestService
{
	public class Service : IService
	{
		public string Foo(string arg1)
		{
			return string.Format($"Hi 'arg1'");
		}
	}
}
