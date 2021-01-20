namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// Flags controlling the method of the check by a interaction diagram
	/// </summary>
	public enum IntDiagCheckMethod
	{
		/// <summary>
		///  known Ned,Med; calculate Nrd,Mrd as intersection of ID and line=[0,0];[Ned,Med]
		/// </summary>
		IDCheckTypeNuMuMu = 1,

		/// <summary>
		/// know Med; calculate +-Nrd
		/// </summary>
		IDCheckTypeNuMM = 2,

		/// <summary>
		///  know Ned; calculate +-Mrd
		/// </summary>
		IDCheckTypeNMuMu = 4,

		/// <summary>
		/// Find intersection only in the direction of the Fd
		/// </summary>
		IDOnlyUlsForce1 = 8,

		/// <summary>
		/// Only planar check is required
		/// </summary>
		IDOnlyPlanarCheck = 16,

		/// <summary>
		/// Information about plane of deformation is required
		/// </summary>
		IDPlaneInfoRequest = 32,

		/// <summary>
		/// used by CheckForceQuick
		/// </summary>
		IDPlanar_0deg = 64,

		/// <summary>
		/// used by CheckForceQuick
		/// </summary>
		IDPlanar_90deg = 128
	}

	/// <summary>
	/// ULS Check Diagram Capacity base
	/// </summary>
	public abstract class ConcreteULSCheckResultDiagramCapacity : ConcreteCheckResultBase
	{
		/// <summary>
		/// calculated ultimate internal forces
		/// </summary>
		public IdeaRS.OpenModel.Result.ResultOfInternalForces Fu1 { get; set; }

		/// <summary>
		/// calculated ultimate internal forces
		/// </summary>
		public IdeaRS.OpenModel.Result.ResultOfInternalForces Fu2 { get; set; }

		/// <summary>
		/// calculated ultimate internal forces
		/// </summary>
		public IdeaRS.OpenModel.Result.ResultOfInternalForces Fu { get; set; }

		/// <summary>
		/// method of check
		/// </summary>
		public IntDiagCheckMethod CheckMethod { get; set; }
	}
}