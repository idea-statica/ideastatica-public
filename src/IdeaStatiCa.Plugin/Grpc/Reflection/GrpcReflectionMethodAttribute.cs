using System;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// Determines that method decorated with this attribute is a grpc reflection service method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class GrpcReflectionMethodAttribute : Attribute
	{

	}
}
