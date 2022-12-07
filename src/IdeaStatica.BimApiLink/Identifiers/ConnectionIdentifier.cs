using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public class ConnectionIdentifier<T> : ImmutableIdentifier<T>
		where T : IIdeaObject
	{
		public double X { get; }

		public double Y { get; }

		public double Z { get; }


		public ConnectionIdentifier(double X, double Y, double Z)
			: base(typeof(T).FullName + "-" + $"{X};{Y};{Z}")
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}
	}
}