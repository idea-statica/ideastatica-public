#if NETSTANDARD2_0_OR_GREATER

using System;

namespace IdeaStatiCa.Plugin
{
	public class ServiceContractAttribute : Attribute
	{
	}

	public class OperationContractAttribute : Attribute
	{
	public bool IsOneWay {get; set;}
	}

	public class ErrorHandlerAttribute : Attribute
	{
		public ErrorHandlerAttribute(Type errorHandler)
		{
		}
	}
}

#endif