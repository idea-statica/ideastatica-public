using System;

namespace CI.Common.MultiMethods
{
	[Flags]
	public enum DispatchStatus
	{
		Success = 1,
		NoMatch = 2,
		AmbiguousMatch = 4,
		DynamicInvoke = 8
	}

	public struct DispatchResult
	{
		private DispatchStatus _status;

		internal DispatchResult(DispatchStatus status)
			: this()
		{
			_status = status;
		}

		public object ReturnValue { get; internal set; }

		public bool Success
		{
			get
			{
				return (_status & DispatchStatus.Success) == DispatchStatus.Success;
			}
		}

		public bool AmbiguousMatch
		{
			get
			{
				return (_status & DispatchStatus.AmbiguousMatch) == DispatchStatus.AmbiguousMatch;
			}
		}

		public bool NoMatch
		{
			get
			{
				return (_status & DispatchStatus.NoMatch) == DispatchStatus.NoMatch;
			}
		}

		public bool IsDynamicInvoke
		{
			get
			{
				return (_status & DispatchStatus.DynamicInvoke) == DispatchStatus.DynamicInvoke;
			}
			internal set
			{
				_status |= DispatchStatus.DynamicInvoke;
			}
		}

		public DispatchResult<R> Typed<R>()
		{
			DispatchResult<R> result = new DispatchResult<R>(_status);
			if (this.ReturnValue != null)
			{
				result.ReturnValue = (R)this.ReturnValue;
			}
			return result;
		}
	}

	public struct DispatchResult<R>
	{
		private DispatchStatus _status;

		internal DispatchResult(DispatchStatus status)
			: this()
		{
			_status = status;
		}

		public R ReturnValue { get; internal set; }

		public bool Success
		{
			get
			{
				return (_status & DispatchStatus.Success) == DispatchStatus.Success;
			}
		}

		public bool AmbiguousMatch
		{
			get
			{
				return (_status & DispatchStatus.AmbiguousMatch) == DispatchStatus.AmbiguousMatch;
			}
		}

		public bool NoMatch
		{
			get
			{
				return (_status & DispatchStatus.NoMatch) == DispatchStatus.NoMatch;
			}
		}

		public bool IsDynamicInvoke
		{
			get
			{
				return (_status & DispatchStatus.DynamicInvoke) == DispatchStatus.DynamicInvoke;
			}
		}
	}
}