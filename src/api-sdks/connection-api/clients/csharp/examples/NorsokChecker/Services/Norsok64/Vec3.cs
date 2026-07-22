namespace NorsokChecker.Services.Norsok64
{
	/// <summary>
	/// Minimal immutable 3D vector for the joint-topology math. Port of extract.py's list-based
	/// vector helpers (dot/nrm/cross/unit/scal/add/sub). All quantities SI (metres / N / N·m).
	/// </summary>
	public readonly struct Vec3
	{
		public readonly double X, Y, Z;
		public Vec3(double x, double y, double z) { X = x; Y = y; Z = z; }

		public static readonly Vec3 Zero = new(0, 0, 0);

		public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		public static Vec3 operator *(Vec3 a, double k) => new(a.X * k, a.Y * k, a.Z * k);
		public static Vec3 operator -(Vec3 a) => new(-a.X, -a.Y, -a.Z);

		public static double Dot(Vec3 a, Vec3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		public static Vec3 Cross(Vec3 a, Vec3 b) =>
			new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

		public double Norm => Math.Sqrt(X * X + Y * Y + Z * Z);

		/// <summary>Unit vector; Zero when the norm is ~0 (mirrors extract.py's unit()).</summary>
		public Vec3 Unit()
		{
			double n = Norm;
			return n > 1e-12 ? new Vec3(X / n, Y / n, Z / n) : Zero;
		}

		public override string ToString() => $"({X:F4}, {Y:F4}, {Z:F4})";
	}
}
