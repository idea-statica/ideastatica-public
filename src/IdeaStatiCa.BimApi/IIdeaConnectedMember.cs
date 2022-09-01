namespace IdeaStatiCa.BimApi
{
	public enum IdeaGeometricalType
	{
		Continuous,
		Ended
	}
	public enum IdeaConnectedMemberType
	{
		Structural,
		Stiffening,
		Negative
	}

	public enum IdeaForcesIn
	{
		Position = 0,
		Node = 1,
		Bolts = 2,
	}

	public enum IdeaBeamSegmentModelType : int
	{
		/// <summary>
		/// N-Vy-Vz-Mx-My-Mz
		/// </summary>
		LoadedInXYZ = IdeaBeamSegmentEndFlags.AllDirActive,

		/// <summary>
		/// N-Vy-Mz
		/// </summary>
		LoadedInXY = IdeaBeamSegmentEndFlags.DoNotActDirZ,

		/// <summary>
		/// N-Vz-My
		/// </summary>
		LoadedInXZ = IdeaBeamSegmentEndFlags.DoNotActDirY,

		/// <summary>
		/// N
		/// </summary>
		LoadedInX = (IdeaBeamSegmentEndFlags.DoNotActDirY + IdeaBeamSegmentEndFlags.DoNotActDirZ),

		/// <summary>
		/// Rod
		/// </summary>
		Rod = (LoadedInXZ + IdeaBeamSegmentEndFlags.Rod),

		/// <summary>
		/// N + Vz
		/// </summary>
		LoadedInXYZ_NoBending = (IdeaBeamSegmentEndFlags.AllDirActive + IdeaBeamSegmentEndFlags.NoBending),
	}

	public enum IdeaBeamSegmentEndFlags : int
	{
		/// <summary>
		/// Free end, fully loaded
		/// </summary>
		AllDirActive = 0,

		DoNotActDirX = 1,

		/// <summary>
		/// Loaded in XZ, internal forces Vy, Mx, Mz are not allowed, fixed at Y, rz
		/// </summary>
		DoNotActDirY = 2,

		/// <summary>
		/// Loaded in XY, internal forces Vz, Mx, My are not allowed, fixed at Z, ry
		/// </summary>
		DoNotActDirZ = 4,

		/// <summary>
		/// End is fully supported
		/// </summary>
		Supported = 8,

		/// <summary>
		/// Only axial force
		/// </summary>
		Rod = 16,

		/// <summary>
		/// No bending moments
		/// </summary>
		NoBending = 32,
	}

	public interface IIdeaConnectedMember : IIdeaObject
	{
		IdeaGeometricalType GeometricalType { get; }

		IdeaConnectedMemberType ConnectedMemberType { get; }

		bool IsBearing { get; }

		IdeaForcesIn ForcesIn { get; set; }

		IdeaBeamSegmentModelType MemberSegmentType { get; }

		IIdeaMember1D IdeaMember { get; }

		bool AutoAddCutByWorkplane { get; }
	}
}
