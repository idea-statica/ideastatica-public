using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.8.2 — Axial Compression and Bending (Eq. 6.27–6.28)
	/// </summary>
	public static class CompressionBendingCheck
	{
		public const double E_steel = 2.1e5; // MPa

		/// <summary>Stability check — Equation (6.27)</summary>
		public static NorsokFormulaResult EvaluateStability(
			double N_Sd_kN, double N_c_Rd_kN,
			double M_y_Sd, double M_z_Sd, double M_Rd,
			double Cmy, double Cmz,
			double N_Ey_kN, double N_Ez_kN)
		{
			double axialTerm = N_c_Rd_kN > 0 ? N_Sd_kN / N_c_Rd_kN : 0;

			double ampY = N_Ey_kN > 0 ? 1.0 - N_Sd_kN / N_Ey_kN : 1.0;
			double ampZ = N_Ez_kN > 0 ? 1.0 - N_Sd_kN / N_Ez_kN : 1.0;
			double My_amp = ampY != 0 ? Cmy * M_y_Sd / ampY : 0;
			double Mz_amp = ampZ != 0 ? Cmz * M_z_Sd / ampZ : 0;
			double momentTerm = M_Rd > 0 ? Math.Sqrt(My_amp * My_amp + Mz_amp * Mz_amp) / M_Rd : 0;

			double interaction = axialTerm + momentTerm;

			return new NorsokFormulaResult
			{
				Section = "6.3.8.2",
				Equation = "6.27",
				Title = "Compression + Bending (Stability)",
				CheckExpression = "N_Sd/N_c,Rd + √[(Cmy·My/(1-N/NEy))² + (Cmz·Mz/(1-N/NEz))²] / M_Rd ≤ 1.0",
				Formula = "Interaction = N_Sd/N_c,Rd + 1/M_Rd · √[(Cmy·My,Sd/(1-N_Sd/N_Ey))² + (Cmz·Mz,Sd/(1-N_Sd/N_Ez))²]",
				FormulaSubstituted = $"= {N_Sd_kN:F1}/{N_c_Rd_kN:F1} + √[({Cmy:F2}×{Math.Abs(M_y_Sd):F1}/{ampY:F3})² + ({Cmz:F2}×{Math.Abs(M_z_Sd):F1}/{ampZ:F3})²]/{M_Rd:F1} = {axialTerm:F4} + {momentTerm:F4} = {interaction:F4}",
				Demand = interaction,
				Capacity = 1.0,
				Utilization = interaction,
				Passed = interaction <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "design compressive force", Value = N_Sd_kN, Unit = "kN" },
					new() { Symbol = "N_c,Rd", Description = "design compressive resistance (§6.3.3)", Value = N_c_Rd_kN, Unit = "kN" },
					new() { Symbol = "M_y,Sd", Description = "in-plane bending moment", Value = M_y_Sd, Unit = "kNm" },
					new() { Symbol = "M_z,Sd", Description = "out-of-plane bending moment", Value = M_z_Sd, Unit = "kNm" },
					new() { Symbol = "M_Rd", Description = "design bending resistance (§6.3.4)", Value = M_Rd, Unit = "kNm" },
					new() { Symbol = "C_my", Description = "moment reduction factor y-axis (Table 6-2)", Value = Cmy, Unit = "-" },
					new() { Symbol = "C_mz", Description = "moment reduction factor z-axis (Table 6-2)", Value = Cmz, Unit = "-" },
					new() { Symbol = "N_Ey", Description = "Euler buckling load y-axis (Eq. 6.29)", Value = N_Ey_kN, Unit = "kN" },
					new() { Symbol = "N_Ez", Description = "Euler buckling load z-axis (Eq. 6.30)", Value = N_Ez_kN, Unit = "kN" },
				}
			};
		}

		/// <summary>Cross-section check — Equation (6.28)</summary>
		public static NorsokFormulaResult EvaluateCrossSection(
			double N_Sd_kN, double N_cl_Rd_kN,
			double M_y_Sd, double M_z_Sd, double M_Rd)
		{
			double axialTerm = N_cl_Rd_kN > 0 ? N_Sd_kN / N_cl_Rd_kN : 0;
			double M_res = Math.Sqrt(M_y_Sd * M_y_Sd + M_z_Sd * M_z_Sd);
			double momentTerm = M_Rd > 0 ? M_res / M_Rd : 0;
			double interaction = axialTerm + momentTerm;

			return new NorsokFormulaResult
			{
				Section = "6.3.8.2",
				Equation = "6.28",
				Title = "Compression + Bending (Cross-section)",
				CheckExpression = "N_Sd / N_cl,Rd + √(M²_y,Sd + M²_z,Sd) / M_Rd ≤ 1.0",
				Formula = "Interaction = N_Sd/N_cl,Rd + √(M²_y + M²_z)/M_Rd",
				FormulaSubstituted = $"= {N_Sd_kN:F1}/{N_cl_Rd_kN:F1} + √({Math.Abs(M_y_Sd):F1}² + {Math.Abs(M_z_Sd):F1}²)/{M_Rd:F1} = {axialTerm:F4} + {momentTerm:F4} = {interaction:F4}",
				Demand = interaction,
				Capacity = 1.0,
				Utilization = interaction,
				Passed = interaction <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "design compressive force", Value = N_Sd_kN, Unit = "kN" },
					new() { Symbol = "N_cl,Rd", Description = "local buckling resistance = f_cl·A/γ_M", Value = N_cl_Rd_kN, Unit = "kN" },
					new() { Symbol = "M_y,Sd", Description = "in-plane bending moment", Value = M_y_Sd, Unit = "kNm" },
					new() { Symbol = "M_z,Sd", Description = "out-of-plane bending moment", Value = M_z_Sd, Unit = "kNm" },
					new() { Symbol = "M_Rd", Description = "design bending resistance (§6.3.4)", Value = M_Rd, Unit = "kNm" },
				}
			};
		}

		public static double EulerBucklingLoad(double A, double k, double l, double i)
		{
			double kl_i = k * l / i;
			double N_E_N = Math.PI * Math.PI * E_steel * A / (kl_i * kl_i);
			return N_E_N / 1000.0; // kN
		}
	}
}
