namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Setup value types Ec2
	/// </summary>
	public enum SetupValueEc2
	{
		/// <summary>
		/// The limit check value, when check is over - check is not satisfactory
		/// </summary>
		LimitCheckValue,

		/// <summary>
		/// The infinity check value. This will be displayed when the check value equals to infinity
		/// or check wasn't calculated.
		/// </summary>
		MaxDisplayCheckValue,

		/// <summary>
		/// partial factor for concrete for ULS accidental design situation 2.4.2.4(1) Setup2Values, double, double
		/// </summary>
		NA_GammaC,

		/// <summary>
		/// partial factor for reinforcement for ULS accidental design situation 2.4.2.4(1) Setup2Values, double, double
		/// </summary>
		NA_GammaS,

		/// <summary>
		/// partial factor for prestressed reinforcement for ULS accidental design situation 2.4.2.4(1) Setup2Values, double, double
		/// </summary>
		NA_GammaSP,

		/// <summary>
		/// ration of design and characteristics strain limit - reinforcement
		/// </summary>
		NA_CoeffEpsudByEpsuk,

		/// <summary>
		/// ration of design and characteristics strain limit - prestressed reinforcement
		/// </summary>
		NA_CoeffEpsudByEpsuk_p,

		/// <summary>
		/// Iteration Precission
		/// </summary>
		IterationPrecission,

		/// <summary>
		/// Iteration Steps
		/// </summary>
		IterationSteps,

		/// <summary>
		/// InteractionDiagramCheckType
		/// </summary>
		InteractionDiagramCheckType,

		/// <summary>
		/// NA_7_3_1_Wmax_Ec2_1_1
		/// </summary>
		NA_7_3_1_Wmax_Ec2_1_1,

		/// <summary>
		/// EN 1992-1-1 7.2 (2)
		/// DIN EN 1992-1-1/NA:2011-01 April 2013 NDP re 7.2 (2)
		/// </summary>
		NA_7_2_K1,

		/// <summary>
		/// EN 1992-1-1 7.2 (3)
		/// DIN EN 1992-1-1/NA:2011-01 April 2013 NDP re 7.2 (3)
		/// </summary>
		NA_7_2_K2,

		/// <summary>
		/// EN 1992-1-1 7.2 (5)
		/// DIN EN 1992-1-1/NA:2011-01 April 2013 NDP re 7.2 (5)
		/// </summary>
		NA_7_2_K3,

		/// <summary>
		/// EN 1992-1-1 7.2 (5)
		/// DIN EN 1992-1-1/NA:2011-01 April 2013 NDP re 7.2 (5)
		/// </summary>
		NA_7_2_K4,

		/// <summary>
		/// EN 1992-1-1 7.2 (5)
		/// DIN EN 1992-1-1/NA:2011-01 April 2013 NDP re 7.2 (5)
		/// </summary>
		NA_7_2_K5,

		/// <summary>
		/// NA_7_3_4_K3
		/// </summary>
		NA_7_3_4_K3,

		/// <summary>
		/// NA_7_3_4_K4
		/// </summary>
		NA_7_3_4_K4,

		/// <summary>
		/// NA_LoadDuration
		/// </summary>
		NA_LoadDuration,

		/// <summary>
		/// NA_Alphacc_92_1_1
		/// </summary>
		NA_Alphacc_92_1_1,

		/// <summary>
		/// NA_Alphact_92_1_1
		/// </summary>
		NA_Alphact_92_1_1,

		/// <summary>
		/// NA_Alphaccpl_92_1_1
		/// </summary>
		NA_Alphaccpl_92_1_1,

		/// <summary>
		/// NA_Alphactpl_92_1_1
		/// </summary>
		NA_Alphactpl_92_1_1,

		/// <summary>
		/// NA_Alphacc_92_2
		/// </summary>
		NA_Alphacc_92_2,

		/// <summary>
		/// NA_Alphact_92_2
		/// </summary>
		NA_Alphact_92_2,

		/// <summary>
		/// Theta
		/// </summary>
		Theta,

		/// <summary>
		/// ThetaMin
		/// </summary>
		ThetaMin,

		/// <summary>
		/// ThetaMax
		/// </summary>
		ThetaMax,

		/// <summary>
		/// CoeffCrdc
		/// </summary>
		CoeffCrdc,

		/// <summary>
		/// CoeffK1
		/// </summary>
		CoeffK1,

		/// <summary>
		/// CoeffVmin
		/// </summary>
		CoeffVmin,

		/// <summary>
		/// CoeffNi1
		/// </summary>
		CoeffNi1,

		/// <summary>
		/// AlphaCw
		/// </summary>
		AlphaCw,

		/// <summary>
		/// CoeffKp
		/// </summary>
		CoeffKp,

		/// <summary>
		/// detailing beam
		/// minimum long reinf percentage
		/// </summary>
		MinLongReinfPercBeam,

		/// <summary>
		/// maximum long reinf percentage
		/// </summary>
		MaxLongReinfPercBeam,

		/// <summary>
		/// minimum long reinf distance
		/// </summary>
		MinLongReinfDist,

		/// <summary>
		/// maximum long reinf distance
		/// </summary>
		MaxLongReinfDist,

		/// <summary>
		/// minimum shear reinf percentage
		/// </summary>
		MinShearReinfPercBeam,

		/// <summary>
		/// maximum shear reinf percentage
		/// </summary>
		MaxShearReinfPercBeam,

		/// <summary>
		/// maximum shear reinf distance
		/// </summary>
		MaxShearReinfDistBeam,

		/// <summary>
		/// maximum shear reinf trans distance
		/// </summary>
		MaxShearReinfTransDist,

		/// <summary>
		/// detailing column
		/// minimum long reinf percentage
		/// </summary>
		MinLongReinfPercColumn,

		/// <summary>
		/// maximum long reinf percentage
		/// </summary>
		MaxLongReinfPercColumn,

		/// <summary>
		/// minimum long reinf diameter
		/// </summary>
		MinLongReinfDiamColumn,

		/// <summary>
		/// minimum number of bars of long reinf
		/// </summary>
		MinNoBarCircColumn,

		/// <summary>
		/// maximum shear reinf distance
		/// </summary>
		MaxShearReinfDistColumn,

		/// <summary>
		/// minimum shear reinf diameter
		/// </summary>
		MinShearReinfDiamColumn,

		/// <summary>
		/// stress limitation - type calculation of concrete stress limitation in tension
		/// </summary>
		StressLimit_TypeFctm,

		/// <summary>
		/// lateral shear - coefficient k (6.2.4 (6))
		/// </summary>
		CoeffKl,

		/// <summary>
		/// lateral shear - angle theta - tensioned flange
		/// </summary>
		Theta_t,

		/// <summary>
		/// lateral shear - angle theta - compressed flange
		/// </summary>
		Theta_c,

		/// <summary>
		/// lateral shear - min angle theta - tensioned flange
		/// </summary>
		Theta_min_t,

		/// <summary>
		/// lateral shear - min angle theta - compressed flange
		/// </summary>
		Theta_min_c,

		/// <summary>
		/// lateral shear - max angle theta
		/// </summary>
		Theta_max_f,

		/// <summary>
		/// The the basic value of imperfections (See 5.2 (5)).
		/// </summary>
		NA_5_2_5_Theta0,

		/// <summary>
		/// Partial safety factor for second order effect design, see 5.8.6 (3).
		/// </summary>
		NA_5_8_6_3_GammaCe,

		/// <summary>
		/// ductility factor for prestressed reinforcement
		/// </summary>
		NA_k_p,

		/// <summary>
		/// weakened by bars - all bars area is subtract for concrete area
		/// </summary>
		WeakenedByBars,

		/// <summary>
		/// weakened by tendons - all tendons area is subtract for concrete area
		/// </summary>
		WeakenedByTendons,

		/// <summary>
		/// weakened by ducts - all ducts area is subtract for concrete area
		/// </summary>
		WeakenedByDucts,

		/// <summary>
		/// Type of E-modulus, which is used in calculations
		/// </summary>
		ModulusType,

		/// <summary>
		/// Effects of prestressing at serviceability limit state and limit state of fatigue
		/// The values of rsup and rinf for use in a Country may be found in its National Annex. The recommended values are:
		/// - for pre-tensioning or unbonded tendons: rsup = 1,05 and rinf = 0,95
		/// - when appropriate measures (e.g. direct measurements of pretensioning) are taken: rsup = rinf = 1,0.
		/// </summary>
		NA_5_10_9_r_sup_pre,

		/// <summary>
		/// Effects of prestressing at serviceability limit state and limit state of fatigue
		/// The values of rsup and rinf for use in a Country may be found in its National Annex. The recommended values are:
		/// - for pre-tensioning or unbonded tendons: rsup = 1,05 and rinf = 0,95
		/// - when appropriate measures (e.g. direct measurements of pretensioning) are taken: rsup = rinf = 1,0.
		/// </summary>
		NA_5_10_9_r_inf_pre,

		/// <summary>
		/// Effects of prestressing at serviceability limit state and limit state of fatigue
		/// The values of rsup and rinf for use in a Country may be found in its National Annex. The recommended values are:
		/// - for post-tensioning with bonded tendons: rsup = 1,10 and rinf = 0,90
		/// - when appropriate measures (e.g. direct measurements of pretensioning) are taken: rsup = rinf = 1,0.
		/// </summary>
		NA_5_10_9_r_sup_post,

		/// <summary>
		/// Effects of prestressing at serviceability limit state and limit state of fatigue
		/// The values of rsup and rinf for use in a Country may be found in its National Annex. The recommended values are:
		/// - for post-tensioning with bonded tendons: rsup = 1,10 and rinf = 0,90
		/// - when appropriate measures (e.g. direct measurements of pretensioning) are taken: rsup = rinf = 1,0.
		/// </summary>
		NA_5_10_9_r_inf_post,

		/// <summary>
		/// Partial factors for prestress
		/// The value of 􀁊P,fav for use in a Country may be found in its National Annex. The recommended value for
		/// persistent and transient design situations is 1,0. This value may also be used for fatigue verification
		/// </summary>
		NA_2_4_2_2_Gamma_p_fav,

		/// <summary>
		/// Partial factors for prestress
		/// The value of 􀁊P,unfav in the stability limit state for use in a Country may be found in its National Annex. The
		/// recommended value for global analysis is 1,3.
		/// </summary>
		NA_2_4_2_2_Gamma_p_unfav,

		/// <summary>
		/// The durability of prestressed members may be more critically affected by cracking. In the absence of more
		/// detailed requirements, it may be assumed that limiting the calculated crack widths to the values of wmax given
		/// in Table 7.1N, under the frequent combination of loads, will generally be satisfactory for prestressed concrete
		/// members. The decompression limit requires that all parts of the bonded tendons or duct lie at least 100 mm
		/// within concrete in compression
		/// </summary>
		NA_7_3_1_DecompressionDistance,

		/// <summary>
		/// Number of the plane interaction diagrams
		/// </summary>
		PlaneDiagramCount,

		/// <summary>
		/// Division of the plane interaction diagram
		/// </summary>
		DivisionStrain,

		/// <summary>
		/// The age of the concrete (days) at the beginning of drying shrinkage (or swelling) in days. Normally this is at the end of curing.
		/// </summary>
		EndOfCuring,

		/// <summary>
		/// The safety factor for long-term extrapolation of delayed strains, see Ec2-2 B.105.
		/// Is used for calculation of creep effect. The value Gamma lt is calculated.
		/// </summary>
		UseGammalt,

		/// <summary>
		/// NA_7_3_1_Wmax_Ec2_2
		/// </summary>
		NA_7_3_1_Wmax_Ec2_2,

		/// <summary>
		/// Check cracks during shear calculation
		/// </summary>
		CheckCrackedCss,

		/// <summary>
		/// User values for shear calculation - d and z
		/// </summary>
		UserValuesForShear,

		/// <summary>
		/// Values in table 74N
		/// </summary>
		//[Obsolete]
		Table74N,

		/// <summary>
		/// The decompression limit requires that all parts of the bonded tendons or duct lie at least 25 mm within concrete in compression for 1992-1-1
		/// </summary>
		DecompressionLimit,

		/// <summary>
		/// The decompression limit requires that all parts of the bonded tendons or duct lie at least 25 mm within concrete in compression for 1992_2
		/// </summary>
		DecompressionLimit_2,

		/// <summary>
		/// Minimal reinforcement ratio of vertical reinforcement
		/// </summary>
		MinVertReinfPercWall,

		/// <summary>
		/// Maximal reinforcement ratio of vertical reinforcement
		/// </summary>
		MaxVertReinfPercWall,

		/// <summary>
		/// Maximal spacing of vertical reinforcement
		/// </summary>
		MaxVertReinfDistWall,

		/// <summary>
		/// Minimal reinforcement ratio of horizontal reinforcement
		/// </summary>
		MinHorReinfPercWall,

		/// <summary>
		/// Maximal spacing of horizontal reinforcement
		/// </summary>
		MaxHorReinfDistWall,

		/// <summary>
		/// Minimal reinforcement ratio
		/// </summary>
		MinReinfPercDeepBeam,

		/// <summary>
		/// Maximal spacing of reinforcement
		/// </summary>
		MaxReinfDistDeepBeam,

		/// <summary>
		/// Minimal reinforcement ratio of main reinforcement
		/// </summary>
		MinMainReinfPercSlab,

		/// <summary>
		/// Maximal reinforcement ratio of main reinforcement
		/// </summary>
		MaxMainReinfPercSlab,

		/// <summary>
		/// Minimal reinforcement ratio of transverse reinforcement
		/// </summary>
		MinTransReinfPercSlab,

		/// <summary>
		/// Maximal spacing of main reinforcement
		/// </summary>
		MaxMainReinfDistSlab,

		/// <summary>
		/// Maximal spacing of transverse reinforcement
		/// </summary>
		MaxTransReinfDistSlab,

		/// <summary>
		/// limit deformation - quasi-permanent deformation
		/// </summary>
		NA_LimitQuasiPermanent,

		/// <summary>
		/// limit deformation - characteristics deformation
		/// </summary>
		NA_LimitCharacteristic,

		/// <summary>
		/// limit deformation - infrequent deformation
		/// </summary>
		NA_LimitInfrequent,

		/// <summary>
		/// Minimal vertical distance of tendons
		/// </summary>
		MinTendonVertDist,

		/// <summary>
		/// Minimal horizontal distance of tendons
		/// </summary>
		MinTendonHorDist,

		/// <summary>
		/// Minimal vertical distance of ducts
		/// </summary>
		MinDuctVertDist,

		/// <summary>
		/// Minimal horizontal distance of ducts
		/// </summary>
		MinDuctHorDist,

		/// <summary>
		/// maximum tendon stress coeff k1 in 5.10.2.1(1)P
		/// </summary>
		NA_5_10_2_1_1_K1,

		/// <summary>
		/// maximum tendon stress coeff k2 in 5.10.2.1(1)P
		/// </summary>
		NA_5_10_2_1_1_K2,

		/// <summary>
		/// maximum tendon stress coeff k7 in 5.10.3(2)
		/// </summary>
		NA_5_10_3_2_K7,

		/// <summary>
		/// maximum tendon stress coeff k8 in 5.10.3(2)
		/// </summary>
		NA_5_10_3_2_K8,

		/// <summary>
		/// switch if the cross section should be calculated as cracked
		/// </summary>
		CheckCrossSectionCrackedIfOneCracked_7_2_Chapter,

		/// <summary>
		/// minimal diameter of mandrel acc table 8.1N
		/// </summary>
		NA_8_3_2_MinDiameterOfMandrel,

		/// <summary>
		/// strength reduction factor
		/// </summary>
		CoeffNi,

		/// <summary>
		/// limit value of deflection for deflection requirement normal
		/// </summary>
		LimitDeflectionNormal,

		/// <summary>
		/// limit value of deflection for deflection requrement advanced
		/// </summary>
		LimitDeflectionAdvanced,

		/// <summary>
		/// maximum length of zone to divide member for stiffness calculation
		/// </summary>
		MaxLengthOfZone,

		/// <summary>
		/// limit value as value
		/// </summary>
		LimitDeflectionValue,

		/// <summary>
		/// linear stiffness for deflection calculation - only for debug
		/// </summary>
		LinearStiffnessForDeflection,

		/// <summary>
		/// direction of imperfection for second order effect
		/// </summary>
		ImperfectionDirection,

		/// <summary>
		/// type of interpolate curce
		/// </summary>
		InterpolationCurve,

		/// <summary>
		/// division of interaction diagram for export
		/// </summary>
		InteractionDiagramDivision,

		/// <summary>
		/// coefficient ni - SaT
		/// </summary>
		NA_6_5_2_3_Ni,

		/// <summary>
		/// coefficient k1 - SaT
		/// </summary>
		NA_6_5_4_4_K1,

		/// <summary>
		/// coefficient k2 - SaT
		/// </summary>
		NA_6_5_4_4_K2,

		/// <summary>
		/// coefficient k3 - SaT
		/// </summary>
		NA_6_5_4_4_K3,

		/// <summary>
		/// coefficient k1 - SaT bracket
		/// </summary>
		NA_J_3_2_K1,

		/// <summary>
		/// coefficient k2 - SaT bracket
		/// </summary>
		NA_J_3_3_K2,

		/// <summary>
		/// type of SAT method
		/// </summary>
		SaTMethodType,

		/// <summary>
		/// calculation method of transversal reinforcement
		/// </summary>
		DetailingBracketMethodType,

		/// <summary>
		/// minimum anchorage length of shear reinforcement
		/// </summary>
		MinAnchLenShear,

		/// <summary>
		/// anchorage of reinforcement according to type
		/// </summary>
		AnchorageDetailType,

		/// <summary>
		/// The partial factor for fatigue loads
		/// DIN EN 1992-1-1/NA:2011-01 April 2013 NDP re 6.8.4 (1)
		/// </summary>
		NA_GammaFfat,

		/// <summary>
		/// partial factor for concrete for fatigue - ULS accidental design situation 2.4.2.4(1) Setup2Values, double, double
		/// </summary>
		NA_GammaCfat,

		/// <summary>
		/// partial factor for reinforcement for fatigue - ULS accidental design situation 2.4.2.4(1) Setup2Values, double, double
		/// </summary>
		NA_GammaSfat,

		/// <summary>
		/// partial factor for fatigue - for prestressed reinforcement for ULS accidental design situation 2.4.2.4(1) Setup2Values, double, double
		/// </summary>
		NA_GammaSPfat,

		/// <summary>
		/// table 6.3N for fatigue - parameters for reinforcement steel
		/// DIN EN 1992-1-1/NA:2011-01 April 2013 NCI re 6.8.4, Table 6.3N
		/// </summary>
		NA_TableFatigue63N,

		/// <summary>
		/// table 6.4N for fatigue - parameters for prestress steel
		/// DIN EN 1992-1-1/NA:2011-01 April 2013 NCI re 6.8.4, Table 6.4N
		/// </summary>
		NA_TableFatigue64N,

		/// <summary>
		/// table 6.101N for fatigue - parameters for reinforcement steel - Dutch annex
		/// </summary>
		NA_TableFatigue6101N,

		/// <summary>
		/// N fatigue cycles 6.8.7 Verification of concrete under compression or shear
		/// DIN EN 1992-1-1/NA:2011-01 April 2013 NDP re 6.8.7 (1)
		/// </summary>
		NA_NCyclesFatigue,

		/// <summary>
		/// coefficient k1 for fatigue
		/// </summary>
		NA_K1Fatigue,

		/// <summary>
		/// table 4.5(n) acc 1991-2 - Indicative number of heavy vehicles expected per year and per slow lane
		/// </summary>
		SetupTable45n1991_2,

		/// <summary>
		/// 6.8.7 Verification of concrete under compression or shear
		/// </summary>
		FatigueMethod,

		/// <summary>
		/// on - the cross-section is calculated always as cracked
		/// </summary>
		CrossSectionCrackedPlate,

		/// <summary>
		/// sr,max, is calculated according to user
		/// </summary>
		CalculateSrMaxAccUserSettings,

		/// <summary>
		/// 6.2.5 (2) Parameters of joint
		/// </summary>
		TableJointParameters,

		/// <summary>
		/// Type of E-modulus, which is used in calculations of long-term effects
		/// </summary>
		ModulusTypeLongTermEffects,

		/// <summary>
		/// Type of shear stress calculation
		/// </summary>
		JointShearStressType,

		/// <summary>
		/// Minimal number of time nodes per decade
		/// </summary>
		SubintervalsPerDecade,

		/// <summary>
		/// returns K1 coefficient according to 5.5 EN 1992-1-1
		/// </summary>
		NA_5_5_K1,

		/// <summary>
		/// returns K2 coefficient according to 5.5 EN 1992-1-1
		/// </summary>
		NA_5_5_K2,

		/// <summary>
		/// returns K3 coefficient according to 5.5 EN 1992-1-1
		/// </summary>
		NA_5_5_K3,

		/// <summary>
		/// returns K4 coefficient according to 5.5 EN 1992-1-1
		/// </summary>
		NA_5_5_K4,

		/// <summary>
		/// returns K5 coefficient according to 5.5 EN 1992-1-1
		/// </summary>
		/// <returns>k5 - coeffciennt</returns>
		NA_5_5_K5,

		/// <summary>
		/// returns K6 coefficient according to 5.5 EN 1992-1-1
		/// </summary>
		NA_5_5_K6,

		/// <summary>
		/// type of interaction diagram for export
		/// </summary>
		InteractionDiagramExportType,

		/// <summary>
		/// settings if null forces are checked with some precissions
		/// </summary>
		IsSetPrecissionForNullForces,

		/// <summary>
		/// No resistance of concrete in tension - members 1D
		/// </summary>
		NoResistanceOfConcreteInTension1D,

		/// <summary>
		/// Neglect redistribution of moments My, Mz, if the ratio My/Mz is less than 10%
		/// </summary>
		NeglectRedistributionOfMoments,

		/// <summary>
		/// NA_1_Wmax_Ec2
		/// </summary>
		//[Obsolete]
		NA_1_Wmax_Ec2,

		/// <summary>
		/// calculation of non-linear creep
		/// </summary>
		NonlinearCreep,

		/// <summary>
		/// true to allow check in concrete age less than 3 days
		/// </summary>
		AllowCheckInAgeLess3Days,

		/// <summary>
		/// TypeOfInitialstateOfCSS
		/// </summary>
		TypeOfInitialstateOfCSS,

		/// <summary>
		/// reduction coefficinet of bent-up bars
		/// </summary>
		BentUpBarsReduction,

		/// <summary>
		/// calculate optimalization of strut angle in shear, torsion and interaction
		/// </summary>
		StrutAngleOptimalization,

		/// <summary>
		/// IsSetMrForULSMNKappaDiagram
		/// </summary>
		IsSetMrForULSMNKappaDiagram,

		/// <summary>
		/// x lim crack width
		/// </summary>
		NA_EN1992_3_7_3_1_112_x_min,

		/// <summary>
		/// crack width limitation
		/// </summary>
		NA_EN1992_3_CrackWidth,

		/// <summary>
		/// CracksPassThrough
		/// </summary>
		CracksPassThrough,

		/// <summary>
		/// use some annex specialities in common eurocode
		/// </summary>
		CracWidthNationalAnnex,

		/// <summary>
		/// Vestigal resistance
		/// </summary>
		VestigalResistance,

		/// <summary>
		/// dont exclude tendons from calculation model of cross-section
		/// </summary>
		NoTendonExclusion,

		/// <summary>
		/// High strength concrete for DIN NCI re 3.1.2 (6)
		/// </summary>
		HighStrengthConcrete,

		/// <summary>
		/// Inner perimeter for DIN NCI re 3.1.4 (5)
		/// </summary>
		InnerPerimeter,

		/// <summary>
		/// Limit for lever arm acc. to DIN NCI re 6.2.3 (1)
		/// </summary>
		LimitLeverArm,

		/// <summary>
		/// 6.8.4 (5) Pokud se použijí pravidla 6.8 pro vyhodnocení zbytkové životnosti existující konstrukce nebo pro
		/// posouzení potřeby jejího zesílení, pak pokud začala koroze, lze stanovit rozkmit napětí sníženým
		/// exponentem napětí k2 pro přímé a ohýbané pruty
		/// /// DIN EN 1992-1-1/NA:2011-01 April 2013 NDP re 6.8.4 (5)
		/// </summary>
		NA_K2Fatigue,

		/// <summary>
		/// NCI re 7.3.1 (5)
		///	In order to keep within the decompression limit, it is required that the concrete surrounding the tendon is in compression over a width of 100 mm
		///	or 1/10 of the depth of section (whichever is greater). Stresses are to be checked in state II.
		/// </summary>
		DecompressionLimit_3,

		/// <summary>
		/// table 6.3N for fatigue - parameters for reinforcement steel
		/// DIN EN 1992-2/NA 2013 NCI re 6.8.4, Table 6.3N
		/// </summary>
		NA_TableFatigue63N_1992_2,

		/// <summary>
		/// table 6.4N for fatigue - parameters for prestress steel
		/// DIN EN 1992-2/NA April 2013 NCI re 6.8.4, Table 6.4N
		/// </summary>
		NA_TableFatigue64N_1992_2,

		/// <summary>
		/// the bearing capacity of the reinforcement crossing the joint due to shear friction (third term of the equation) may be increased to become ρ fyd (1,2 μ sin α + cos α).
		/// </summary>
		IncreaseJointResistance,

		/// <summary>
		/// Equation 6.29 acc. to DIN NCI re 6.3.2 (4)
		/// </summary>
		Equation629,

		/// <summary>
		/// Equation 6.31 acc. to DIN NCI re 6.3.2 (5)
		/// </summary>
		Equation631,

		/// <summary>
		/// NDP zu 7.3.1 (105) Es gelten Tabelle 7.101DE
		/// </summary>
		NA_Table7_101DE_1992_2,

		/// <summary>
		/// NDP zu 7.3.1 (105) Es gelten Tabelle 7.102DE
		/// </summary>
		NA_Table7_102DE_1992_2,

		/// <summary>
		/// Under fatigue or dynamic loads, the values for c in 6.2.5 (1) should be halved.
		/// </summary>
		FatigueJointCohesion,

		/// <summary>
		/// Values in table 74N
		/// </summary>
		Table74N_1992_1_1,

		/// <summary>
		/// NDP zu 7.3.1 (105) Es gelten Tabelle 7.103DE
		/// </summary>
		NA_Table7_103DE_1992_2,

		/// <summary>
		/// maximum longitudinal spacing of bent-up bars
		/// </summary>
		MaxBentUpReinfDist,

		/// <summary>
		/// Minimal shear reinforcement percentage for slab
		/// </summary>
		MinShearReinfPercSlab,

		/// <summary>
		/// maximum longitudinal spacing of successive series of links for slab
		/// </summary>
		MaxShearReinfDistSlab,

		/// <summary>
		/// maximum transverse spacing of shear reinforcement for slab
		/// </summary>
		MaxShearReinfTransDistSlab,

		/// <summary>
		/// minimum shear reinf diameter EN2-2
		/// </summary>
		MinShearReinfDiamColumn_EN2_2,

		/// <summary>
		/// coeff ni1 for EN2-2
		/// </summary>
		CoeffNi1_EN2_2,

		/// <summary>
		/// minimum long reinf percentage for EN2-2
		/// </summary>
		MinLongReinfPercBeam_EN2_2,

		/// <summary>
		/// maximum long reinf percentage for EN2-2
		/// </summary>
		MaxLongReinfPercBeam_EN2_2,

		/// <summary>
		/// maximum main long reinf distance for EN2-2
		/// </summary>
		MaxMainReinfDistSlab_EN2_2,

		/// <summary>
		/// maximum transversal long reinf distance for EN2-2
		/// </summary>
		MaxTransReinfDistSlab_EN2_2,

		/// <summary>
		/// maximum horizontal long reinf distance for EN2-2
		/// </summary>
		MaxHorReinfDistWall_EN2_2,

		/// <summary>
		/// minimum long reinf percentage for EN2-2
		/// </summary>
		MinReinfPercDeepBeam_EN2_2,

		/// <summary>
		/// returns K5 coefficient according to DIN EN 1992-2/NA April 2013 NDP re 5.5 (4)
		/// </summary>
		/// <returns>k5 - coeffciennt</returns>
		NA_5_5_K5_fck_BiggerThan_50MPa,

		/// <summary>
		/// returns K5 coefficient according to DIN EN 1992-2/NA April 2013NDP re 5.5 (4)
		/// </summary>
		/// <returns>k5 - coeffciennt</returns>
		NA_5_5_K6_fck_BiggerThan_50MPa,

		/// <summary>
		/// is a coefficient which takes account of the bond properties of the bonded reinforcement
		/// </summary>
		NA_7_3_4_K1,

		/// <summary>
		/// is a coefficient which takes account of the distribution of strain
		/// </summary>
		NA_7_3_4_K2,

		/// <summary>
		/// coefficient
		/// </summary>
		NA_NN_112_Gamma_sd,

		/// <summary>
		/// coefficient witch reduce the tension strength
		/// </summary>
		CoefficientOfTensionConcrete,

		/// <summary>
		/// FindOnly2DPlaneDeformation
		/// </summary>
		//[Obsolete]
		FindOnly2DPlaneDeformation,

		/// <summary>
		/// Use simplified calculation model of cross-section (reinforcement bars in layers are substituted by rectangle polygon component)
		/// </summary>
		SimplifiedCssModel,

		/// <summary>
		/// BLR partial factor for concrete for ULS accidental design situation 2.4.2.4(1) Setup2Values, double, double
		/// </summary>
		NA_GammaC_BLR,

		/// <summary>
		/// BLR partial factor for reinforcement for ULS accidental design situation 2.4.2.4(1) Setup2Values, double, double
		/// </summary>
		NA_GammaS_BLR,

		/// <summary>
		/// partial factor for prestressed reinforcement for ULS accidental design situation 2.4.2.4(1) Setup2Values, double, double
		/// </summary>
		NA_GammaSP_BLR,

		/// <summary>
		/// precision of bridge load rating - check value
		/// </summary>
		BLRPRecisionCheckValue,

		/// <summary>
		/// set of SLS calculation
		/// 0 - both
		/// 1 - short-term
		/// 2 - long-term
		/// </summary>
		TypeSLSCalculation,

		/// <summary>
		/// No resistance of concrete in tension - plates
		/// </summary>
		NoResistanceOfConcreteInTension2D,

		/// <summary>
		/// number of parts on design member
		/// </summary>
		NumberPartsOfDM,

		/// <summary>
		/// calculate influence of shrinkage to stiffnesses
		/// </summary>
		CalculateShrinkage,

		/// <summary>
		/// crack width limitation
		/// </summary>
		NA_EN1992_3_CrackWidth_XA2_XA3_XF2_XF3_XF4,

		/// <summary>
		/// calculation of stress limitation k1
		/// </summary>
		CalculationOfStressLimitationK1,

		/// <summary>
		/// calculation of stress limitation k2
		/// </summary>
		CalculationOfStressLimitationK2,

		/// <summary>
		/// calculation of stress limitation k3
		/// </summary>
		CalculationOfStressLimitationK3,
	}

	/// <summary>
	/// Annex code
	/// </summary>
	public enum NationalAnnexCode
	{
		/// <summary>
		/// No national annex defined
		/// </summary>
		NoAnnex = 0,

		/// <summary>
		/// Czech national annex
		/// </summary>
		Czech,

		/// <summary>
		/// Slovak national annex
		/// </summary>
		Slovak,

		/// <summary>
		/// Austria national annex
		/// </summary>
		Austrian,

		/// <summary>
		/// Germany national annex
		/// </summary>
		German,

		/// <summary>
		/// Dutch national annex
		/// </summary>
		Dutch,

		/// <summary>
		/// Belgian national annex
		/// </summary>
		Belgian,

		/// <summary>
		/// France national annex
		/// </summary>
		French,

		/// <summary>
		///British national annex
		/// </summary>
		British,

		/// <summary>
		/// Singapore national annex
		/// </summary>
		Singapore,

		/// <summary>
		/// Polish national annex
		/// </summary>
		Polish,
	}

	/// <summary>
	/// Concrete setup Ec2
	/// </summary>
	public class ConcreteSetupEc2 : ConcreteSetup
	{
		/// <summary>
		/// Annex code
		/// </summary>
		public NationalAnnexCode Annex { get; set; }

		/// <summary>
		/// Nad strategy
		/// </summary>
		public NadStrategyConcrete Nad { get; set; }
	}
}