using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.8.2 — Axial Compression and Bending (Equations 6.27–6.28)
	///
	/// Stability check (Eq. 6.27):
	///   N_Sd/N_c,Rd + 1/M_Rd · √[(C_my·M_y/(1-N/N_Ey))² + (C_mz·M_z/(1-N/N_Ez))²] ≤ 1.0
	///
	/// Cross-section check (Eq. 6.28):
	///   N_Sd/N_cl,Rd + √(M²_y + M²_z)/M_Rd ≤ 1.0
	/// </summary>
	public static class CompressionBendingCheck
	{
		public const double E_steel = 2.1e5; // [MPa]

		/// <summary>
		/// Stability check — Equation (6.27)
		/// </summary>
		public static NorsokFormulaResult EvaluateStability(
			double N_Sd_kN, double N_c_Rd_kN,
			double M_y_Sd, double M_z_Sd, double M_Rd,
			double Cmy, double Cmz,
			double N_Ey_kN, double N_Ez_kN)
		{
			double axialTerm = N_Sd_kN / N_c_Rd_kN;

			// Amplified moment terms
			double My_amp = Cmy * M_y_Sd / (1.0 - N_Sd_kN / N_Ey_kN);
			double Mz_amp = Cmz * M_z_Sd / (1.0 - N_Sd_kN / N_Ez_kN);
			double momentTerm = Math.Sqrt(My_amp * My_amp + Mz_amp * Mz_amp) / M_Rd;

			double interaction = axialTerm + momentTerm;

			return new NorsokFormulaResult
			{
				Section = "6.3.8.2",
				Equation = "6.27",
				Title = "Compression + Bending (Stability)",
				CheckExpression = "N_Sd/N_c,Rd + √[(C_my·M_y/(1-N/N_Ey))² + (C_mz·M_z/(1-N/N_Ez))²]/M_Rd ≤ 1.0",
				Demand = interaction,
				Capacity = 1.0,
				Utilization = interaction,
				Passed = interaction <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "Design compressive force", Value = N_Sd_kN, Unit = "kN" },
					new() { Symbol = "N_c,Rd", Description = "Design compressive resistance (§6.3.3)", Value = N_c_Rd_kN, Unit = "kN" },
					new() { Symbol = "M_y,Sd", Description = "In-plane bending moment", Value = M_y_Sd, Unit = "kNm" },
					new() { Symbol = "M_z,Sd", Description = "Out-of-plane bending moment", Value = M_z_Sd, Unit = "kNm" },
					new() { Symbol = "M_Rd", Description = "Design bending resistance", Value = M_Rd, Unit = "kNm" },
					new() { Symbol = "C_my", Description = "Moment reduction factor (y-axis)", Value = Cmy, Unit = "-" },
					new() { Symbol = "C_mz", Description = "Moment reduction factor (z-axis)", Value = Cmz, Unit = "-" },
					new() { Symbol = "N_Ey", Description = "Euler buckling strength (y-axis)", Value = N_Ey_kN, Unit = "kN" },
					new() { Symbol = "N_Ez", Description = "Euler buckling strength (z-axis)", Value = N_Ez_kN, Unit = "kN" },
					new() { Symbol = "Axial term", Description = "N_Sd / N_c,Rd", Value = axialTerm, Unit = "-" },
					new() { Symbol = "Moment term", Description = "Amplified bending utilization", Value = momentTerm, Unit = "-" },
					new() { Symbol = "Interaction", Description = "Combined utilization (Eq. 6.27)", Value = interaction, Unit = "-" },
				}
			};
		}

		/// <summary>
		/// Cross-section check — Equation (6.28)
		/// </summary>
		public static NorsokFormulaResult EvaluateCrossSection(
			double N_Sd_kN, double N_cl_Rd_kN,
			double M_y_Sd, double M_z_Sd, double M_Rd)
		{
			double axialTerm = N_Sd_kN / N_cl_Rd_kN;
			double momentTerm = Math.Sqrt(M_y_Sd * M_y_Sd + M_z_Sd * M_z_Sd) / M_Rd;
			double interaction = axialTerm + momentTerm;

			return new NorsokFormulaResult
			{
				Section = "6.3.8.2",
				Equation = "6.28",
				Title = "Compression + Bending (Cross-section)",
				CheckExpression = "N_Sd/N_cl,Rd + √(M²_y + M²_z)/M_Rd ≤ 1.0",
				Demand = interaction,
				Capacity = 1.0,
				Utilization = interaction,
				Passed = interaction <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "Design compressive force", Value = N_Sd_kN, Unit = "kN" },
					new() { Symbol = "N_cl,Rd", Description = "Local buckling resistance", Value = N_cl_Rd_kN, Unit = "kN" },
					new() { Symbol = "M_y,Sd", Description = "In-plane bending moment", Value = M_y_Sd, Unit = "kNm" },
					new() { Symbol = "M_z,Sd", Description = "Out-of-plane bending moment", Value = M_z_Sd, Unit = "kNm" },
					new() { Symbol = "M_Rd", Description = "Design bending resistance", Value = M_Rd, Unit = "kNm" },
					new() { Symbol = "Interaction", Description = "Combined utilization (Eq. 6.28)", Value = interaction, Unit = "-" },
				}
			};
		}

		/// <summary>
		/// Compute Euler buckling load N_E [kN] per Equations (6.29)/(6.30)
		/// </summary>
		public static double EulerBucklingLoad(double A, double k, double l, double i)
		{
			double kl_i = k * l / i;
			double N_E_N = Math.PI * Math.PI * E_steel * A / (kl_i * kl_i);
			return N_E_N / 1000.0; // kN
		}
	}
}
