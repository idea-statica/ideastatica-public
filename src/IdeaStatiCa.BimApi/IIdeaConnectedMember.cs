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

	/// <summary>
	/// Represent Connected Member - member 1D assigned in to connection 
	/// </summary>
	public interface IIdeaConnectedMember : IIdeaObjectConnectable
	{
		/// <summary>
		/// Member is [Continuous|Ended]
		/// </summary>
		IdeaGeometricalType GeometricalType { get; }

		/// <summary>
		/// Connected member type [Structural|Stiffening|Negative]
		/// </summary>
		IdeaConnectedMemberType ConnectedMemberType { get; set; }

		/// <summary>
		/// Is member bearing?
		/// </summary>
		bool IsBearing { get; }

		/// <summary>
		/// Load forces placed in
		/// </summary>
		IdeaForcesIn ForcesIn { get; set; }

		/// <summary>
		/// Member segment type
		/// </summary>
		IdeaBeamSegmentModelType MemberSegmentType { get; }

		/// <summary>
		/// member
		/// </summary>
		IIdeaMember1D IdeaMember { get; }

		/// <summary>
		/// Is Cross section place on reference line or center of gravity - affected member offsets
		/// </summary>
		bool IsReferenceLineInCenterOfGravity { get; }

		/// <summary>
		/// Add automatically cut by work plane on beginning and end of member
		/// </summary>
		bool AutoAddCutByWorkplane { get; }
	}
}
