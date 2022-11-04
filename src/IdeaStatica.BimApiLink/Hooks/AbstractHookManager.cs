﻿using System;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.Hooks
{
	internal class AbstractHookManager<T>
	{
		private readonly List<T> _hooks = new List<T>();

		public void Add(T obj)
		{
			_hooks.Add(obj);
		}

		protected void Invoke(Action<T> func)
		{
			foreach (T hook in _hooks)
			{
				try
				{
					func(hook);
				}
				catch
				{
					// TODO: log exception
				}
			}
		}
	}
}