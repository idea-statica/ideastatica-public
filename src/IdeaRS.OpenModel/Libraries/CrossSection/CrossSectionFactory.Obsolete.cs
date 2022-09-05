using System;

namespace IdeaRS.OpenModel.CrossSection
{
	public static partial class CrossSectionFactory
	{
		[Obsolete("Use " + nameof(FillRolledI) + " instead")]
		public static void FillCssIarc(CrossSectionParameter css, double B, double H, double S, double T, double R2, double tapperF, double r1)
			=> FillRolledI(css, B, H, S, T, R2, tapperF, r1);

		[Obsolete("Use " + nameof(FillRolledChannel) + " instead")]
		public static void FillCssSteelChannel(CrossSectionParameter css, double B, double D, double tw, double tf, double rw, double rf, double taperF)
			=> FillRolledChannel(css, B, D, tw, tf, rw, rf, taperF);

		[Obsolete("Use " + nameof(FillRolledAngle) + " instead")]
		public static void FillCssSteelAngle(CrossSectionParameter css, double B, double D, double t, double rw, double r2, double C, bool mirrorZ = false)
			=> FillRolledAngle(css, B, D, t, rw, r2, C, mirrorZ);

		[Obsolete("Use " + nameof(FillRolledCHS) + " instead")]
		public static void FillCHSPar(CrossSectionParameter css, double D, double t)
			=> FillRolledCHS(css, D * 0.5, t);

		[Obsolete("Use " + nameof(FillRolledCHS) + " instead")]
		public static void FillCssSteelCircularHollow(CrossSectionParameter css, double r, double t)
			=> FillRolledCHS(css, r, t);

		[Obsolete("Use " + nameof(FillRolledRHS) + " instead")]
		public static void FillCssSteelRectangularHollow(CrossSectionParameter css, double D, double B, double t, double r1, double r2, double d)
			=> FillRolledRHS(css, D, B, t, r1, r2, d);

		[Obsolete("Use " + nameof(FillRolledCHS) + " instead")]
		public static void FillSteelTube(CrossSectionParameter css, double r, double t)
			=> FillRolledCHS(css, r, t);

		[Obsolete("Use " + nameof(FillRolledT) + " instead")]
		public static void FillCssTarc(CrossSectionParameter css, double B, double H, double Tw, double Tf, double R, double R1, double R2, double tapperF, double tapperW, bool mirrorY = false)
			=> FillRolledT(css, B, H, Tw, Tf, R, R1, R2, tapperF, tapperW, mirrorY);

		[Obsolete("Use " + nameof(FillRolledTFromI) + " instead")]
		public static void FillSteelTI(CrossSectionParameter css, string name, double H, bool mirrorY = false)
			=> FillRolledTFromI(css, name, H, mirrorY);

		[Obsolete("Use " + nameof(FillRolledChannel) + " instead")]
		public static void FillWeldedU(CrossSectionParameter css, double b, double hw, double tw, double tf, double rw, double rf, double taperF, bool mirrorZ = false)
			=> FillRolledChannel(css, b, hw, tw, tf, rw, rf, taperF, mirrorZ);

		[Obsolete("Use " + nameof(FillComposedDblUo) + " instead")]
		public static void FillShapeDblU(CrossSectionParameter css, double bt, double bb, double h, double tb, double tl, double tr, double dis)
			=> FillComposedDblUo(css, bt, bb, h, tb, tl, tr, dis);

		[Obsolete("Use " + nameof(FillComposedDblUc) + " instead")]
		public static void FillWelded2Uc(CrossSectionParameter css, string name, double distance)
			=> FillComposedDblUc(css, name, distance);

		[Obsolete("Use " + nameof(FillComposedDblLt) + " instead")]
		public static void FillWelded2Lt(CrossSectionParameter css, string name, double distance, bool shortLegUp = false, bool mirrorY = false)
			=> FillComposedDblLt(css, name, distance, shortLegUp, mirrorY);

		[Obsolete("Use " + nameof(FillComposedDblLu) + " instead")]
		public static void FillWelded2Lu(CrossSectionParameter css, string name, double distance, bool shortLegUp = false, bool mirrorY = false)
			=> FillComposedDblLu(css, name, distance, shortLegUp, mirrorY);

		[Obsolete("Use " + nameof(FillComposedDblLt) + " instead")]
		public static void FillShapeDblL(CrossSectionParameter css, double h, double b, double th, double sh, double dis, bool shortLegUp = false, bool mirrorY = false)
			=> FillComposedDblLt(css, h, b, th, sh, dis, shortLegUp, mirrorY);

		[Obsolete("Use " + nameof(FillComposedDblLu) + " instead")]
		public static void FillShapeDblLu(CrossSectionParameter css, double h, double b, double th, double sh, double dis, bool shortLegUp = false, bool mirrorY = false)
			=> FillComposedDblLu(css, h, b, th, sh, dis, shortLegUp, mirrorY);

		[Obsolete("Use " + nameof(FillWeldedBoxFlange) + " instead")]
		public static void FillBox2(CrossSectionParameter css, double bu, double bb, double hw, double b1, double tw, double tfu, double tfb)
			=> FillWeldedBoxFlange(css, bu, bb, hw, b1, tw, tfu, tfb);

		[Obsolete("Use " + nameof(FillWeldedBoxWeb) + " instead")]
		public static void FillBoxWeb(CrossSectionParameter css, double bu, double bb, double hw, double b1, double tw, double tfu, double tfb, bool mirrorY = false)
			=> FillWeldedBoxWeb(css, bu, bb, hw, b1, tw, tfu, tfb, mirrorY);

		[Obsolete("Use " + nameof(FillWeldedBoxDelta) + " instead")]
		public static void FillBoxWeb(CrossSectionParameter css, double bu, double bb, double hw, double b1, double h, double tw, double tfu, double tfb, double overlap, BoxDeltaAligment aligment, bool mirrorY = false)
			=> FillWeldedBoxDelta(css, bu, bb, hw, b1, h, tw, tfu, tfb, overlap, aligment, mirrorY);

		[Obsolete("Use " + nameof(FillWeldedTriangle) + " instead")]
		public static void FillTriangle(CrossSectionParameter css, double h, double w, double fTh, double webTh, double webD, bool mirrorY = false)
			=> FillWeldedTriangle(css, h, w, fTh, webTh, webD, mirrorY);
	}
}