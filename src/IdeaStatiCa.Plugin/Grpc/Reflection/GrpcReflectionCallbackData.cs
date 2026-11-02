namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// Contains response returned from Grpc service.
	/// </summary>
	public class GrpcReflectionCallbackData
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public GrpcReflectionCallbackData() { }

		public GrpcReflectionCallbackData(string type, object value)
		{
			ValueType = type;
			Value = value;
		}

		/// <summary>
		/// FullName of the value type.
		/// </summary>
		public string ValueType { get; set; }

		/// <summary>
		/// Value returned from server.
		/// </summary>
		public object Value { get; set; }
	}
}
