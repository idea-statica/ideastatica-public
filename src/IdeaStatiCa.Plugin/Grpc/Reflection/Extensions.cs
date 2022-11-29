using Newtonsoft.Json;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	public static class Extensions
	{
		public static GrpcReflectionArgument FromVal(this GrpcReflectionArgument src, object value)
		{
			src.ParameterType = value.GetType().ToString();
			src.Value = value;
			return src;
		}

		public static GrpcReflectionCallbackData FromVal(this GrpcReflectionCallbackData src, object value)
		{
			src.ValueType = value.GetType().ToString();
			src.Value = value;
			return src;
		}
	}
}
