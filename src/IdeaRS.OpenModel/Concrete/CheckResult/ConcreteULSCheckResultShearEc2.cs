namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// Method of shear calculation
	/// </summary>
	public enum ShearMethod
	{
		/// <summary>
		/// not defined
		/// </summary>
		NoDefined = 0,

		/// <summary>
		/// method according 6.2.2.1
		/// </summary>
		MethodVrdcCracked,

		/// <summary>
		/// method according 6.2.2.2
		/// </summary>
		MethodVrdcPrestress,

		/// <summary>
		/// method according 6.2.2.2
		/// </summary>
		MethodVrdcSimpleHcs,

		/// <summary>
		/// method according EN1168
		/// </summary>
		MethodVrdcGeneralHcs,

		/// <summary>
		/// method according 12.6.3
		/// </summary>
		MethodVrdcPlain,

		/// <summary>
		/// method according 6.2.3.3
		/// </summary>
		MethodVrds,

		/// <summary>
		/// method according 6.2.5(1)
		/// </summary>
		MethodJoints,
	}

	/// <summary>
	/// ULS Check Shear Ec2
	/// </summary>
	public class ConcreteULSCheckResultShearEc2 : ConcreteULSCheckResultShear
	{
		///// <summary>
		///// Gets code type
		///// </summary>
		//public EC2CodeType CodeType { get; internal set; }

		///// <summary>
		///// method of shear calculation
		///// </summary>
		//public ShearMethod method { get; set; }

		///// <summary>
		///// method of Vrdc calculation
		///// </summary>
		//public ShearMethod methodVrdc { get; set; }

		///// <summary>
		///// angle between shear reinforcement and the beam axis perpendicular to the shear force
		///// </summary>
		//public double alpha { get; set; }

		///// <summary>
		///// angle between the concrete compression strut and the beam axis perpendicular to the shear force
		///// </summary>
		//public double theta { get; set; }

		///// <summary>
		///// longitudinal reinforcement area
		///// </summary>
		//public double asl { get; set; }

		///// <summary>
		///// shear reinforcement area
		///// </summary>
		//public double asw { get; set; }

		///// <summary>
		///// strength of shear reinforcement
		///// </summary>
		//public double fywd { get; set; }

		///// <summary>
		///// shear width
		///// </summary>
		//public double bw { get; set; }

		///// <summary>
		///// effective height
		///// </summary>
		//public double d { get; set; }

		///// <summary>
		///// lever arm
		///// </summary>
		//public double z { get; set; }

		///// <summary>
		///// longitudinal force due to shear
		///// </summary>
		//public double fs { get; set; }

		///// <summary>
		///// number of cuts
		///// </summary>
		//public double nc { get; set; }

		///// <summary>
		///// coefficient crdc
		///// </summary>
		//public double crdc { get; set; }

		///// <summary>
		///// coefficient k
		///// </summary>
		//public double k { get; set; }

		///// <summary>
		///// coefficient k1
		///// </summary>
		//public double k1 { get; set; }

		///// <summary>
		///// rho l
		///// </summary>
		//public double rhol { get; set; }

		///// <summary>
		///// sigma cp
		///// </summary>
		//public double sigmacp { get; set; }

		///// <summary>
		///// coefficient vmin
		///// </summary>
		//public double vmin { get; set; }

		///// <summary>
		///// coefficient ni
		///// </summary>
		//public double ni { get; set; }

		///// <summary>
		///// coefficient ni1
		///// </summary>
		//public double ni1 { get; set; }

		///// <summary>
		///// coefficient alpha cw
		///// </summary>
		//public double alphacw { get; set; }

		/// <summary>
		/// shear force
		/// </summary>
		public double Ved { get; set; }

		/// <summary>
		/// shear resistance of concrete
		/// </summary>
		public double Vrdc { get; set; }

		/// <summary>
		/// shear resistance
		/// </summary>
		public double Vrdr { get; set; }

		/// <summary>
		/// shear resistance of compression strut
		/// </summary>
		public double Vrdmax { get; set; }

		/// <summary>
		/// shear resistance of shear reinforcement
		/// </summary>
		public double Vrds { get; set; }

		/// <summary>
		/// decisive shear resistance
		/// </summary>
		public double Vrd { get; set; }

		///// <summary>
		///// second moment of area in formula 6.4
		///// </summary>
		//public double iy { get; set; }

		///// <summary>
		///// first moment of area above abd about centroidical axis in formula 6.4
		///// </summary>
		//public double sy { get; set; }

		///// <summary>
		///// coefficient alpha l in formula 6.4
		///// </summary>
		//public double alphal { get; set; }

		///// <summary>
		///// compressive area of concrete
		///// </summary>
		//public double ac { get; set; }

		///// <summary>
		///// concrete design strength n shear and compression n article 12.6.3
		///// </summary>
		//public double fcvd { get; set; }

		///// <summary>
		///// coefficient k in article 12.6.3
		///// </summary>
		//public double kp { get; set; }

		///// <summary>
		///// fctd in 6.2.2(2)
		///// </summary>
		//public double fctd { get; set; }

		///// <summary>
		///// sigma c lim in 12.6.3
		///// </summary>
		//public double sigmaclim { get; set; }

		///// <summary>
		///// stress in stirrups
		///// </summary>
		//public double sigmawd { get; set; }

		///// <summary>
		///// stress in concrete strut
		///// </summary>
		//public double sigmac { get; set; }

		///// <summary>
		///// infinity check value, maximum display check value
		///// </summary>
		//public double InfinityCheckValue { get; set; }

		///// <summary>
		///// plane deformation
		///// </summary>
		//public PlaneDeformation Plane { get; set; }

		///// <summary>
		///// design value of the shear component of the force in the compression area, in the case of an inclined compression chord
		///// </summary>
		//public double vcd { get; set; }

		///// <summary>
		///// design value of the shear component of the force in the tensile reinforcement, in the case of an inclined tensile chord
		///// </summary>
		//public double vtd { get; set; }

		///// <summary>
		///// force in compression chord
		///// </summary>
		//public double fc { get; set; }

		///// <summary>
		///// force in tensioned chord
		///// </summary>
		//public double ft { get; set; }

		///// <summary>
		///// inflination of force in compression chord
		///// </summary>
		//public double alphaFc { get; set; }

		///// <summary>
		///// inflination of force in tensioned chord
		///// </summary>
		//public double alphaFt { get; set; }

		///// <summary>
		///// position of concrete resultant
		///// </summary>
		//public Point2D Point_cc { get; set; }

		///// <summary>
		///// position of reinforcement resultant
		///// </summary>
		//public Point2D Point_rt { get; set; }

		///// <summary>
		///// type how effective depth was determined
		///// </summary>
		//public DeterminationType d_type { get; set; }

		///// <summary>
		///// type how lever arm was determined
		///// </summary>
		//public DeterminationType z_type { get; set; }

		///// <summary>
		///// type how shear width was determined
		///// </summary>
		//public DeterminationType bw_type { get; set; }

		///// <summary>
		///// point in which shear width was determined
		///// </summary>
		//public Point2D bw_point { get; set; }

		///// <summary>
		///// reduction forces
		///// </summary>
		//public InternalForce Fdred { get; set; }

		///// <summary>
		///// reduction fyd
		///// </summary>
		//public bool reductionFyd { get; set; }

		///// <summary>
		///// stirrup area
		///// </summary>
		//public double asw_s { get; set; }

		///// <summary>
		///// bent-up bars area
		///// </summary>
		//public double asw_b { get; set; }

		///// <summary>
		///// number of cuts
		///// </summary>
		//public double nc_s { get; set; }

		///// <summary>
		///// number of cuts
		///// </summary>
		//public double nc_b { get; set; }

		///// <summary>
		///// shear resistance of stirrups
		///// </summary>
		//public double vrds_s { get; set; }

		///// <summary>
		///// shear resistance of bent-up bars
		///// </summary>
		//public double vrds_b { get; set; }

		///// <summary>
		///// length of shear crack in longitudinal direction
		///// </summary>
		//public double c { get; set; }

		///// <summary>
		///// number of bent-up bars on length c
		///// </summary>
		//public double nb { get; set; }

		///// <summary>
		///// angle of bent up bars
		///// </summary>
		//public double alphab { get; set; }
	}
}