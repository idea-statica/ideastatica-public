using System;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// Determines that interface decorated with this attribtue is a grpc reflection service.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public class GrpcReflectionServiceAttribute : Attribute
	{

	}
}
