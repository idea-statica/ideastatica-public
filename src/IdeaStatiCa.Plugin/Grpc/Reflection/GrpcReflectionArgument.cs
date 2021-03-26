using System;

namespace IdeaStatiCa.Plugin.Grpc.Reflection
{
	/// <summary>
	/// Represents an argument that Grpc will use to call the method invoked via reflection.
	/// </summary>
	public class GrpcReflectionArgument
	{
		/// <summary>
		/// FullName of the arguments value type.
		/// </summary>
		public string ParameterType { get; set; }

		/// <summary>
		/// Value of the argument.
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Initializes a reflection argument with specified parameters.
		/// </summary>
		public GrpcReflectionArgument(string type, object value)
		{
			if (type == null)
			{
				throw new ArgumentException("Type cannot be null!");
			}

			ParameterType = type;
			Value = value;
		}

		/// <summary>
		/// Initializes an empty <see cref="GrpcReflectionArgument"/>
		/// </summary>
		public GrpcReflectionArgument()
		{

		}
	}
}
