namespace IdeaRS.OpenModel.CrossSection
{
	public static partial class CrossSectionFactory
	{
		public static void FillCssIarc(CrossSectionParameter css, double B, double H, double S, double T, double R2, double tapperF, double r1)
		{
			FillRolledI(css, B, H, S, T, R2, tapperF, r1);
		}

		public static void FillCssSteelChannel(CrossSectionParameter css, double B, double D, double tw, double tf, double rw, double rf, double taperF)
		{
			FillRolledChannel(css, B, D, tw, tf, rw, rf, taperF);
		}

		public static void FillCssSteelAngle(CrossSectionParameter css, double B, double D, double t, double rw, double r2, double C, bool mirrorZ = false)
		{
			FillRolledAngle(css, B, D, t, rw, r2, C, mirrorZ);
		}

		public static void FillCHSPar(CrossSectionParameter css, double D, double t)
		{
			FillSteelCHS(css, D * 0.5, t);
		}

		public static void FillCssSteelCircularHollow(CrossSectionParameter css, double r, double t)
		{
			FillSteelCHS(css, r, t);
		}

		public static void FillCssSteelRectangularHollow(CrossSectionParameter css, double D, double B, double t, double r1, double r2, double d)
		{
			FillSteelRHS(css, D, B, t, r1, r2, d);
		}

		public static void FillSteelTube(CrossSectionParameter css, double r, double t)
		{
			FillSteelCHS(css, r, t);
		}

		public static void FillCssTarc(CrossSectionParameter css, double B, double H, double Tw, double Tf, double R, double R1, double R2, double tapperF, double tapperW, bool mirrorY = false)
		{
			FillRolledT(css, B, H, Tw, Tf, R, R1, R2, tapperF, tapperW, mirrorY);
		}

		public static void FillSteelTI(CrossSectionParameter css, string name, double H, bool mirrorY = false)
		{
			FillSteelTFromI(css, name, H, mirrorY);
		}

		public static void FillWeldedU(CrossSectionParameter css, double b, double hw, double tw, double tf, double rw, double rf, double taperF, bool mirrorZ = false)
		{
			FillRolledChannel(css, b, hw, tw, tf, rw, rf, taperF, mirrorZ);
		}
	}
}