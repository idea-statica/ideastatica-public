using IdeaStatiCa.Api.Connection.Model;

namespace NorsokChecker.Services.Norsok64
{
	/// <summary>
	/// Cross-section info for the joint topology (port of extract.py xs_map values).
	/// D/T in mm (as parsed from the CHS name), fy/fu in Pa (SI, straight from the material element).
	/// </summary>
	public sealed class JointSectionInfo
	{
		public string? Name { get; set; }
		public double? D { get; set; }          // mm
		public double? T { get; set; }          // mm

		/// <summary>
		/// The section's TYPE is tubular. Says nothing about whether D/T could be read — those are
		/// two different reasons to reject a member and they used to be merged into this one flag,
		/// which made the app report "not CHS (CHS 219.1/10)" about a section that plainly is one.
		/// Use <see cref="HasDimensions"/> for the geometry, or <see cref="RejectReason"/> for both.
		/// </summary>
		public bool IsCHS { get; set; }

		/// <summary>The cross-section type as the API named it, for the rejection message.</summary>
		public string? TypeName { get; set; }

		/// <summary>
		/// Set when the section name and the model disagree about D by more than 2 % — e.g.
		/// "PIPE127STD" is really Ø141.3 because 127 is the nominal size. Port of python's
		/// geom_note; the whole point of reading D/T from the model rather than the name.
		/// </summary>
		public string? GeomNote { get; set; }

		public double? Fy { get; set; }         // Pa
		public double? Fu { get; set; }         // Pa
		public string? MaterialName { get; set; }

		/// <summary>D and T are both known.</summary>
		public bool HasDimensions => D is > 0 && T is > 0;

		/// <summary>
		/// Why §6.4 cannot use this section, or null when it can. Two distinct answers, as the
		/// python reference gives (_section_reject): a section that is not tubular at all, and a
		/// tubular one whose dimensions could not be read.
		/// </summary>
		public string? RejectReason(string label)
		{
			string nm = string.IsNullOrEmpty(Name) ? "?" : Name!;
			if (!IsCHS)
				return $"{label}: {nm} is {TypeName ?? "an unknown type"} — NORSOK 6.4 applies to "
					+ "tubular (circular hollow) sections only.";
			if (!HasDimensions)
				return $"{label}: {nm} is tubular but its D/T are unknown — "
					+ (GeomNote ?? "dimensions could not be read from the model") + ".";
			return null;
		}

		/// <summary>
		/// CHS name → (D, T) mm; (null, null) if not parseable. Port of parse_chs, tolerant to the
		/// catalog conventions seen in real projects: 'CHS168.3/8.0' (slash sep, dot decimals),
		/// 'CHS457,16 - CHORD(CHS457,16)' (comma SEPARATOR + decorations), 'CHS168,3/8,0'
		/// (slash sep, comma decimals).
		/// </summary>
		public static (double? D, double? T) ParseChs(string? name)
		{
			if (string.IsNullOrEmpty(name) || !name.ToUpperInvariant().Contains("CHS"))
				return (null, null);
			var inv = System.Globalization.CultureInfo.InvariantCulture;
			// slash/x separator; decimals may be dot or comma
			var m = System.Text.RegularExpressions.Regex.Match(name,
				@"CHS\s*([0-9]+(?:[.,][0-9]+)?)\s*[/x×]\s*([0-9]+(?:[.,][0-9]+)?)",
				System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			if (m.Success)
				return (double.Parse(m.Groups[1].Value.Replace(',', '.'), inv),
						double.Parse(m.Groups[2].Value.Replace(',', '.'), inv));
			// comma as the D/T separator (numbers use dot decimals)
			m = System.Text.RegularExpressions.Regex.Match(name,
				@"CHS\s*([0-9]+(?:\.[0-9]+)?)\s*,\s*([0-9]+(?:\.[0-9]+)?)",
				System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			if (m.Success)
				return (double.Parse(m.Groups[1].Value, inv), double.Parse(m.Groups[2].Value, inv));
			return (null, null);
		}
	}

	/// <summary>
	/// One member's geometry + meta as the topology code consumes it — flattened from the REST
	/// member payload (extract.py reads the same fields from JSON). SI: origin/axes/offsets in metres.
	/// </summary>
	public sealed class JointMemberData
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public bool IsContinuous { get; set; }
		public bool IsBearing { get; set; }
		public ConMemberConnectedByEnum ConnectedBy { get; set; }
		public ConMemberForcesInEnum ForcesIn { get; set; } = ConMemberForcesInEnum.Node;
		/// <summary>model.x — distance along the axis for forcesIn=Position (m)</summary>
		public double ModelX { get; set; }
		public Vec3 Origin { get; set; }
		public Vec3 AxisX { get; set; }
		public Vec3 AxisY { get; set; }
		public Vec3 AxisZ { get; set; }
		public double OffsetEx { get; set; }
		public double OffsetEy { get; set; }
		public double OffsetEz { get; set; }
		public JointSectionInfo Section { get; set; } = new();

		/// <summary>Map a typed API member + its section into the topology input.</summary>
		public static JointMemberData FromConMember(ConMember m, JointSectionInfo section)
		{
			var p = m.Position;
			return new JointMemberData
			{
				Id = m.Id,
				Name = m.Name ?? $"Member {m.Id}",
				IsContinuous = m.IsContinuous,
				IsBearing = m.IsBearing,
				ConnectedBy = m.ConnectedBy,
				ForcesIn = m.Model?.ForcesIn ?? ConMemberForcesInEnum.Node,
				ModelX = m.Model?.X ?? 0.0,
				Origin = ToVec(p?.Origin),
				AxisX = ToVec(p?.AxisX),
				AxisY = ToVec(p?.AxisY),
				AxisZ = ToVec(p?.AxisZ),
				OffsetEx = p?.OffsetEx ?? 0.0,
				OffsetEy = p?.OffsetEy ?? 0.0,
				OffsetEz = p?.OffsetEz ?? 0.0,
				Section = section,
			};
		}

		private static Vec3 ToVec(IdeaRS.OpenModel.Geometry3D.Vector3D? v) =>
			v == null ? Vec3.Zero : new Vec3(v.X, v.Y, v.Z);
	}

	/// <summary>Per-brace geometry/meta (port of extract.py braces_meta entries).</summary>
	public sealed class BraceMeta
	{
		public string Name { get; set; } = "";
		public JointSectionInfo Section { get; set; } = new();
		public double ThetaDeg { get; set; }
		public double? Beta { get; set; }
		public double CoplanarDevDeg { get; set; }
		public double OopOffsetM { get; set; }
		public double EccAlongM { get; set; }
		public bool IsCHS { get; set; }
		/// <summary>+1 = +ey chord face, -1 = -ey face</summary>
		public int Side { get; set; } = 1;
	}

	/// <summary>Toe-to-toe gap between two same-face braces (port of compute_gaps rows).</summary>
	public sealed class BraceGap
	{
		public string A { get; set; } = "";
		public string B { get; set; } = "";
		public double GapM { get; set; }
		public int Side { get; set; }           // +1 / -1
		public bool Adjacent { get; set; }

		/// <summary>
		/// False when a diameter the gap depends on (the chord's, or either brace's) was not known,
		/// so GapM is arithmetic on a missing value rather than a measurement.
		///
		/// This exists because the alternative is a confident wrong answer. A missing D used to
		/// default to 0, which shortens the foot to nothing and moves the landing point, and the
		/// toe-to-toe subtraction then comes out NEGATIVE — reported as "feet overlap, out of 6.4
		/// gap rules". Measured 2026-08-27 on CON1 of test_cs against service 26.1, whose IOM export
		/// returns no model at all: all five braces are physically clear of each other (real gaps
		/// +1.5 to +47 mm) and the app rejected the joint for overlapping. A conclusion about
		/// geometry drawn from the absence of geometry is worse than admitting the gap is unknown.
		/// </summary>
		public bool Known { get; set; } = true;
	}

	/// <summary>
	/// One load effect's node-equilibrium residual: the sum of every member's force and moment
	/// about the node, which must be ~0 for loads that balance.
	///
	/// This is the self-check that the forces, axes, levers and eccentricities were read correctly
	/// — a large residual means the reading is wrong, not that the structure is. Small values are
	/// normal because the stored input is rounded.
	/// </summary>
	public sealed class NodeEquilibriumRow
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public Vec3 SumF { get; set; }          // N
		public Vec3 SumM { get; set; }          // N·m
		public bool Ok { get; set; }
	}

	/// <summary>Assumption-gate verdict (port of classify_assumptions result).</summary>
	public sealed class TopologyVerdict
	{
		public string Status { get; set; } = "OK";   // OK | WARNING | ERROR
		public List<string> Errors { get; set; } = new();
		public List<string> Warnings { get; set; } = new();
	}

	/// <summary>One brace's section forces resolved into its joint sub-plane (SI: N, N·m).</summary>
	public sealed class BraceForceRow
	{
		public string Name { get; set; } = "";
		public double NSd { get; set; }         // + tension
		public double Vip { get; set; }
		public double Vop { get; set; }
		public double Mip { get; set; }
		public double Mop { get; set; }
		public double Mtor { get; set; }
		public double SubNormalDot { get; set; }
		public int Side { get; set; } = 1;
	}

	/// <summary>Chord nominal stresses at one brace footprint, NORSOK Qf convention (Pa).</summary>
	public sealed class ChordStressRow
	{
		public string Name { get; set; } = "";
		public double SigmaA { get; set; }      // + tension
		public double SigmaMy { get; set; }     // + compression in footprint
		public double SigmaMz { get; set; }
		public double A { get; set; }
		public double I { get; set; }
		public double R { get; set; }
		public double NChord { get; set; }
		public double MipChord { get; set; }
		public double MopChord { get; set; }
		public int Side { get; set; }
	}

	/// <summary>One K balancing pairing of the classifier (port of classify_kyx K_components).</summary>
	public sealed class KComponent
	{
		public string Partner { get; set; } = "";
		public double? GapM { get; set; }       // null = no gap data (falls back to 0 downstream)
		public double Q { get; set; }           // transverse force taken by this pairing [N]
		public double Frac { get; set; }        // fraction of |q_b|
	}

	/// <summary>K/Y/X classification of one brace for one load effect (port of classify_kyx rows).</summary>
	public sealed class KyxClass
	{
		public string Name { get; set; } = "";
		public double NSd { get; set; }
		public double MipSd { get; set; }
		public double MopSd { get; set; }
		public double QTrans { get; set; }
		public double FrK { get; set; }
		public double FrX { get; set; }
		public double FrY { get; set; }
		public List<KComponent> KComponents { get; set; } = new();
		public string Note { get; set; } = "";
	}

	/// <summary>Per-load-effect wrapper used by forces / stresses / classification / checks.</summary>
	public sealed class PerLoadEffect<T>
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public List<T> Rows { get; set; } = new();
	}

	/// <summary>One brace's §6.4 check result within one load effect (port of joint_checks rows).</summary>
	public sealed class JointCheckRow
	{
		public string Name { get; set; } = "";
		public bool Skipped { get; set; }
		public string? Reason { get; set; }
		public double Util { get; set; }
		public bool Passed { get; set; }
		public bool ChordOverstressed { get; set; }
		public bool NoAxialClassification { get; set; }
		public bool WithinRange { get; set; }
		public double NRdWeighted { get; set; }  // N
		public double MRdIp { get; set; }        // N·m
		public double MRdOp { get; set; }        // N·m
		public string DomClass { get; set; } = "K";
		/// <summary>The full engine result (per-class breakdown, K terms, Qf, validity...).</summary>
		public JointResult64? Engine { get; set; }
		/// <summary>The engine input (echo for the report).</summary>
		public Joint64Input? Inputs { get; set; }
		/// <summary>Chord-stress derivation for the report.</summary>
		public ChordStressRow? ChordStress { get; set; }
		/// <summary>Classification driving the weighted resistance.</summary>
		public KyxClass? Classification { get; set; }

		/// <summary>
		/// Stamped on the winner of <see cref="JointEnvelope.Pick"/>: the load effect this brace's
		/// governing check came from. The id is the stable key — load-effect names are user-editable
		/// and not guaranteed unique — while the name is what the user reads.
		/// </summary>
		public int GovLeId { get; set; }
		public string? GovLeName { get; set; }
	}

	/// <summary>
	/// Full topology + per-LE analysis of one connection (port of build_connection's payload,
	/// minus the purely visual members2d/3d blocks).
	/// </summary>
	public sealed class JointTopology
	{
		public JointMemberData? Chord { get; set; }
		public List<JointMemberData> Braces { get; set; } = new();          // all non-chord members
		public List<JointMemberData> GapBraces { get; set; } = new();       // ended members only
		public List<BraceMeta> BracesMeta { get; set; } = new();
		public List<BraceGap> Gaps { get; set; } = new();
		public TopologyVerdict Verdict { get; set; } = new();
		public bool Coplanar { get; set; } = true;
		public List<string> PlaneOutliers { get; set; } = new();
		public List<string> EvalOutliers { get; set; } = new();
		public string? PlaneFitBasis { get; set; }
		public string? PlaneWarn { get; set; }
		public double PlaneSpread { get; set; }
		public Vec3 Ex { get; set; }             // chord axis (unit)
		public Vec3 Ey { get; set; }             // in-plane axis (unit)
		public Vec3 NPlane { get; set; }         // joint-plane normal (unit)

		public List<PerLoadEffect<BraceForceRow>> BraceForces { get; set; } = new();
		public List<PerLoadEffect<ChordStressRow>> ChordStresses { get; set; } = new();
		public List<PerLoadEffect<KyxClass>> Classification { get; set; } = new();

		/// <summary>
		/// Node-equilibrium residual per load effect. One row per LE, in the same order as
		/// <see cref="JointChecks"/>.
		/// </summary>
		public List<NodeEquilibriumRow> Equilibrium { get; set; } = new();
		public List<PerLoadEffect<JointCheckRow>> JointChecks { get; set; } = new();
	}
}
