namespace IdeaRS.OpenModel.CrossSection
{
	/*
/// <summary>
/// CssParameterFatory
/// </summary>
public partial class CssParameterFatory
{
	public CrossSectionParameter AddCellBase(System.Double hOut1, System.Double hOut2, System.Double hOut21, System.Double hOut22, System.Double hOut3, System.Double hOut31, System.Double bOut1, System.Double bOut11, System.Double bOut12, System.Double bOut2, System.Double bOut21, System.Double bOut3, System.Double hIn1, System.Double hIn2, System.Double hIn21, System.Double hIn22, System.Double hIn3, System.Double hIn31, System.Double hIn4, System.Double hIn41, System.Double hIn42, System.Double hIn5, System.Double bIn1, System.Double bIn11, System.Double bIn12, System.Double bIn21, System.Double bIn3, System.Double bIn31, System.Double bIn32, System.Double bIn4)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "HOut1", Value = hOut1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HOut2", Value = hOut2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HOut21", Value = hOut21 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HOut22", Value = hOut22 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HOut3", Value = hOut3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HOut31", Value = hOut31 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BOut1", Value = bOut1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BOut11", Value = bOut11 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BOut12", Value = bOut12 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BOut2", Value = bOut2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BOut21", Value = bOut21 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BOut3", Value = bOut3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn1", Value = hIn1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn2", Value = hIn2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn21", Value = hIn21 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn22", Value = hIn22 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn3", Value = hIn3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn31", Value = hIn31 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn4", Value = hIn4 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn41", Value = hIn41 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn42", Value = hIn42 });
		ret.Parameters.Add(new ParameterDouble() { Name = "HIn5", Value = hIn5 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BIn1", Value = bIn1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BIn11", Value = bIn11 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BIn12", Value = bIn12 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BIn21", Value = bIn21 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BIn3", Value = bIn3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BIn31", Value = bIn31 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BIn32", Value = bIn32 });
		ret.Parameters.Add(new ParameterDouble() { Name = "BIn4", Value = bIn4 });
		return ret;
	}
	public CrossSectionParameter AddCellOne()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddCellTwo()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	/// <summary>
	///
	/// </summary>
	/// <param name="height"></param>
	/// <param name="width"></param>
	/// <param name="horizontal"></param>
	/// <param name="vertical"></param>
	/// <param name="th"></param>
	/// <returns></returns>
	public CrossSectionParameter AddShapeOctagon(System.Double height, System.Double width, System.Double horizontal, System.Double vertical, System.Double th)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "Horizontal", Value = horizontal });
		ret.Parameters.Add(new ParameterDouble() { Name = "Vertical", Value = vertical });
		ret.Parameters.Add(new ParameterDouble() { Name = "Th", Value = th });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeBox(System.Double bw, System.Double bw2, System.Double bw3, System.Double h, System.Double h2, System.Double c, System.Double c2, System.Double bc, System.Double htf, System.Double hf, System.Double hf2, System.Double o)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw2", Value = bw2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw3", Value = bw3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "H2", Value = h2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "C", Value = c });
		ret.Parameters.Add(new ParameterDouble() { Name = "C2", Value = c2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bc", Value = bc });
		ret.Parameters.Add(new ParameterDouble() { Name = "Htf", Value = htf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hf", Value = hf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hf2", Value = hf2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "O", Value = o });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeBox1(System.Double bw, System.Double bw2, System.Double bw3, System.Double h, System.Double h2, System.Double c, System.Double c2, System.Double bc, System.Double htf, System.Double htf2, System.Double hf, System.Double hf2, System.Double o)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw2", Value = bw2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw3", Value = bw3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "H2", Value = h2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "C", Value = c });
		ret.Parameters.Add(new ParameterDouble() { Name = "C2", Value = c2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bc", Value = bc });
		ret.Parameters.Add(new ParameterDouble() { Name = "Htf", Value = htf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Htf2", Value = htf2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hf", Value = hf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hf2", Value = hf2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "O", Value = o });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeIHaunchChamfer(System.Double bbf, System.Double hbf, System.Double hbfh, System.Double bw, System.Double h, System.Double htfh, System.Double htf, System.Double btf, System.Double bwh)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbf", Value = bbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf", Value = hbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbfh", Value = hbfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Htfh", Value = htfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "Htf", Value = htf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Btf", Value = btf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bwh", Value = bwh });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeIHaunchChamferAssym(System.Double bbf, System.Double hbf, System.Double hbfh, System.Double bw, System.Double bwh, System.Double h, System.Double htfh, System.Double htf, System.Double btfl, System.Double btfr, System.Double btf)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbf", Value = bbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf", Value = hbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbfh", Value = hbfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bwh", Value = bwh });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Htfh", Value = htfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "Htf", Value = htf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Btfl", Value = btfl });
		ret.Parameters.Add(new ParameterDouble() { Name = "Btfr", Value = btfr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Btf", Value = btf });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeIrevDegen(System.Double bbf, System.Double bw, System.Double hbf, System.Double hbfh, System.Double h, System.Double htfh, System.Double btf)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbf", Value = bbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf", Value = hbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbfh", Value = hbfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Htfh", Value = htfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "Btf", Value = btf });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeIrevDegenAdd(System.Double bbf, System.Double bw, System.Double hbf, System.Double hbfh, System.Double h2, System.Double htfh, System.Double h, System.Double hc, System.Double btf, System.Double l, System.Double r)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbf", Value = bbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf", Value = hbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbfh", Value = hbfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "H2", Value = h2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Htfh", Value = htfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hc", Value = hc });
		ret.Parameters.Add(new ParameterDouble() { Name = "Btf", Value = btf });
		ret.Parameters.Add(new ParameterDouble() { Name = "L", Value = l });
		ret.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeIZDegen(System.Double bn, System.Double bn3, System.Double hn, System.Double hc, System.Double l, System.Double hn2, System.Double hc2, System.Double hc3, System.Double r)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bn", Value = bn });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bn3", Value = bn3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hn", Value = hn });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hc", Value = hc });
		ret.Parameters.Add(new ParameterDouble() { Name = "L", Value = l });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hn2", Value = hn2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hc2", Value = hc2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hc3", Value = hc3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeLDegen(System.Double bw, System.Double h, System.Double h2, System.Double bw2, System.Double bw3, System.Double bla)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "H2", Value = h2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw2", Value = bw2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw3", Value = bw3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bla", Value = bla });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeRevU(System.Double h, System.Double b, System.Double tl, System.Double tr, System.Double tb)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tl", Value = tl });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tr", Value = tr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = tb });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeTrevDegen(System.Double bbf, System.Double bbfh, System.Double bw, System.Double hbf, System.Double hbfh, System.Double h, System.Double bc2)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbf", Value = bbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbfh", Value = bbfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf", Value = hbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbfh", Value = hbfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bc2", Value = bc2 });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeTrevDegenAdd(System.Double bw, System.Double hbfh, System.Double h, System.Double h3, System.Double bc, System.Double bc2)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbfh", Value = hbfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "H3", Value = h3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bc", Value = bc });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bc2", Value = bc2 });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeTrevChamferHaunchD(System.Double bbf, System.Double bbfh, System.Double bbfh2, System.Double hbf, System.Double hbf2, System.Double hbf3, System.Double bw, System.Double h)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbf", Value = bbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbfh", Value = bbfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbfh2", Value = bbfh2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf", Value = hbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf2", Value = hbf2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf3", Value = hbf3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeTrevChamferHaunchS(System.Double bbf, System.Double hbf, System.Double hbf2, System.Double hbf3, System.Double bw, System.Double bbfh2, System.Double h)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbf", Value = bbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf", Value = hbf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf2", Value = hbf2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hbf3", Value = hbf3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bbfh2", Value = bbfh2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		return ret;
	}
	public CrossSectionParameter AddBeamShapeZDegen(System.Double bn, System.Double hn, System.Double hc, System.Double l, System.Double hn2, System.Double hn3, System.Double hc2, System.Double r)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bn", Value = bn });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hn", Value = hn });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hc", Value = hc });
		ret.Parameters.Add(new ParameterDouble() { Name = "L", Value = l });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hn2", Value = hn2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hn3", Value = hn3 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Hc2", Value = hc2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
		return ret;
	}
	public CrossSectionParameter AddRectangle2D(System.Double width, System.Double height, System.Double offsetY, System.Double offsetZ)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		ret.Parameters.Add(new ParameterDouble() { Name = "OffsetY", Value = offsetY });
		ret.Parameters.Add(new ParameterDouble() { Name = "OffsetZ", Value = offsetZ });
		return ret;
	}
	public CrossSectionParameter AddRectangle2DCompressionMember()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddRectangle2DHollow(System.Double b, System.Double h, System.Double tb, System.Double tt, System.Double tl, System.Double tr, System.Double zt, System.Double yt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = tb });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = tt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tl", Value = tl });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tr", Value = tr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Yt", Value = yt });
		return ret;
	}
	public CrossSectionParameter AddRectangle2DSlab()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddRectangleTwoWaySlab()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddRectOnTrapezoidSheet(System.Double width, System.Double height, System.Double sheetHeight, System.Double sheetWidthA, System.Double sheetWidthB, System.Double sheetWidthBTot)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		ret.Parameters.Add(new ParameterDouble() { Name = "SheetHeight", Value = sheetHeight });
		ret.Parameters.Add(new ParameterDouble() { Name = "SheetWidthA", Value = sheetWidthA });
		ret.Parameters.Add(new ParameterDouble() { Name = "SheetWidthB", Value = sheetWidthB });
		ret.Parameters.Add(new ParameterDouble() { Name = "SheetWidthBTot", Value = sheetWidthBTot });
		return ret;
	}
	public CrossSectionParameter AddRectOnTrapezoidSheetNeg()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeAnnulusSector(System.Double radius, System.Double thickness, System.Double angle1, System.Double angle2)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = radius });
		ret.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = thickness });
		ret.Parameters.Add(new ParameterDouble() { Name = "Angle1", Value = angle1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Angle2", Value = angle2 });
		return ret;
	}
	public CrossSectionParameter AddShapeI()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeIBase(System.Double h, System.Double bh, System.Double bs, System.Double ts, System.Double th, System.Double tw, System.Double tfh, System.Double bfh)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bh", Value = bh });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bs", Value = bs });
		ret.Parameters.Add(new ParameterDouble() { Name = "Ts", Value = ts });
		ret.Parameters.Add(new ParameterDouble() { Name = "Th", Value = th });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = tw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tfh", Value = tfh });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bfh", Value = bfh });
		return ret;
	}
	public CrossSectionParameter AddShapeL(System.Double b, System.Double th, System.Double sh, System.Double h)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
		ret.Parameters.Add(new ParameterDouble() { Name = "Th", Value = th });
		ret.Parameters.Add(new ParameterDouble() { Name = "Sh", Value = sh });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		return ret;
	}
	public CrossSectionParameter AddShapeLl(System.Double b, System.Double th, System.Double sh, System.Double h)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
		ret.Parameters.Add(new ParameterDouble() { Name = "Th", Value = th });
		ret.Parameters.Add(new ParameterDouble() { Name = "Sh", Value = sh });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		return ret;
	}
	public CrossSectionParameter AddShapeO(System.Double diameter, System.Double radius)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Diameter", Value = diameter });
		ret.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = radius });
		return ret;
	}
	public CrossSectionParameter AddShapeOHollow(System.Double diameter, System.Double radius, System.Double thickness)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Diameter", Value = diameter });
		ret.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = radius });
		ret.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = thickness });
		return ret;
	}
	public CrossSectionParameter AddShapeOval()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeS(System.Double bt, System.Double bb, System.Double bw, System.Double h, System.Double tt, System.Double tb, System.Double zt, System.Double yt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bt", Value = bt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bb", Value = bb });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = tt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = tb });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Yt", Value = yt });
		return ret;
	}
	public CrossSectionParameter AddShapeT()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeT1()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeT2()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeTAssym(System.Double bfl, System.Double bfr, System.Double bfll, System.Double bfrr, System.Double tw, System.Double tfr, System.Double tfl, System.Double h)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bfl", Value = bfl });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bfr", Value = bfr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bfll", Value = bfll });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bfrr", Value = bfrr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = tw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tfr", Value = tfr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tfl", Value = tfl });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		return ret;
	}
	public CrossSectionParameter AddShapeTAssymRev(System.Double bfl, System.Double bfr, System.Double tw, System.Double tfr, System.Double tfl, System.Double h)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bfl", Value = bfl });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bfr", Value = bfr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = tw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tfr", Value = tfr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tfl", Value = tfl });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		return ret;
	}
	public CrossSectionParameter AddShapeTBase(System.Double height, System.Double width, System.Double topFlangeWidth, System.Double wallWidth, System.Double wallHaunch, System.Double topFlangeHaunch, System.Double zt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = topFlangeWidth });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = wallWidth });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallHaunch", Value = wallHaunch });
		ret.Parameters.Add(new ParameterDouble() { Name = "TopFlangeHaunch", Value = topFlangeHaunch });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		return ret;
	}
	public CrossSectionParameter AddShapeTChamfer1()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeTChamfer2()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeTChamferBase(System.Double height, System.Double width, System.Double topFlangeWidth, System.Double wallWidth, System.Double wallHaunch1, System.Double wallHaunch2, System.Double topFlangeHaunch1, System.Double topFlangeHaunch2, System.Double zt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = topFlangeWidth });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = wallWidth });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallHaunch1", Value = wallHaunch1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallHaunch2", Value = wallHaunch2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "TopFlangeHaunch1", Value = topFlangeHaunch1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "TopFlangeHaunch2", Value = topFlangeHaunch2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		return ret;
	}
	public CrossSectionParameter AddShapeTrev()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeTrev1()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeTrev2()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeTrevBase(System.Double height, System.Double width, System.Double topFlangeWidth, System.Double wallWidth, System.Double wallHaunch, System.Double topFlangeHaunch, System.Double zt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = topFlangeWidth });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = wallWidth });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallHaunch", Value = wallHaunch });
		ret.Parameters.Add(new ParameterDouble() { Name = "TopFlangeHaunch", Value = topFlangeHaunch });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		return ret;
	}
	public CrossSectionParameter AddShapeTT()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeTT1()
	{
		var ret = new CrossSectionParameter();
		return ret;
	}
	public CrossSectionParameter AddShapeTTBase(System.Double height, System.Double width, System.Double topFlangeWidth, System.Double wallWidth, System.Double wallHaunch1, System.Double wallHaunch2, System.Double topFlangeHaunch, System.Double wallSpace, System.Double zt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "TopFlangeWidth", Value = topFlangeWidth });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallWidth", Value = wallWidth });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallHaunch1", Value = wallHaunch1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallHaunch2", Value = wallHaunch2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "TopFlangeHaunch", Value = topFlangeHaunch });
		ret.Parameters.Add(new ParameterDouble() { Name = "WallSpace", Value = wallSpace });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		return ret;
	}
	public CrossSectionParameter AddShapeU(System.Double b, System.Double h, System.Double tb, System.Double tl, System.Double tr, System.Double zt, System.Double yt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = tb });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tl", Value = tl });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tr", Value = tr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Yt", Value = yt });
		return ret;
	}
	public CrossSectionParameter AddShapeZ(System.Double bt, System.Double bb, System.Double bw, System.Double h, System.Double tt, System.Double tb, System.Double zt, System.Double yt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Bt", Value = bt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bb", Value = bb });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bw", Value = bw });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tt", Value = tt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tb", Value = tb });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Yt", Value = yt });
		return ret;
	}
	public CrossSectionParameter AddCssSteelAngle(System.Double rw, System.Double depth, System.Double width, System.Double thickness, System.Double toeRadius, System.Double centroidPosition)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Rw", Value = rw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Depth", Value = depth });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = thickness });
		ret.Parameters.Add(new ParameterDouble() { Name = "ToeRadius", Value = toeRadius });
		ret.Parameters.Add(new ParameterDouble() { Name = "CentroidPosition", Value = centroidPosition });
		return ret;
	}
	public CrossSectionParameter AddCssSteelCircularHollow(System.Double radius, System.Double thickness)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = radius });
		ret.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = thickness });
		return ret;
	}
	public CrossSectionParameter AddCssSteelChannel(System.Double rw, System.Double depth, System.Double width, System.Double tf, System.Double tw, System.Double bucklingDepth, System.Double flangeTaper, System.Double centroidPosition, System.Double rf)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Rw", Value = rw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Depth", Value = depth });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tf", Value = tf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = tw });
		ret.Parameters.Add(new ParameterDouble() { Name = "BucklingDepth", Value = bucklingDepth });
		ret.Parameters.Add(new ParameterDouble() { Name = "FlangeTaper", Value = flangeTaper });
		ret.Parameters.Add(new ParameterDouble() { Name = "CentroidPosition", Value = centroidPosition });
		ret.Parameters.Add(new ParameterDouble() { Name = "Rf", Value = rf });
		return ret;
	}
	public CrossSectionParameter AddCssSteelRectangularHollow(System.Double depth, System.Double width, System.Double thickness, System.Double innerRadius, System.Double outerRadius, System.Double webBucklingDepth)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Depth", Value = depth });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = thickness });
		ret.Parameters.Add(new ParameterDouble() { Name = "InnerRadius", Value = innerRadius });
		ret.Parameters.Add(new ParameterDouble() { Name = "OuterRadius", Value = outerRadius });
		ret.Parameters.Add(new ParameterDouble() { Name = "WebBucklingDepth", Value = webBucklingDepth });
		return ret;
	}
	public CrossSectionParameter AddCssSteelShapeIrec(System.Double arc, System.Double height, System.Double width, System.Double flangeThickness, System.Double webThickness, System.Double r1, System.Double tapperF)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Arc", Value = arc });
		ret.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "FlangeThickness", Value = flangeThickness });
		ret.Parameters.Add(new ParameterDouble() { Name = "WebThickness", Value = webThickness });
		ret.Parameters.Add(new ParameterDouble() { Name = "R1", Value = r1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "TapperF", Value = tapperF });
		return ret;
	}
	public CrossSectionParameter AddCssSteelShapeThinWall(System.Double radius, System.Double thickness)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "Radius", Value = radius });
		ret.Parameters.Add(new ParameterDouble() { Name = "Thickness", Value = thickness });
		return ret;
	}
	public CrossSectionParameter AddCssSteelShapeTrec(System.Double r, System.Double r1, System.Double r2, System.Double height, System.Double width, System.Double tf, System.Double tw, System.Double tapperf, System.Double tapperw)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "R", Value = r });
		ret.Parameters.Add(new ParameterDouble() { Name = "R1", Value = r1 });
		ret.Parameters.Add(new ParameterDouble() { Name = "R2", Value = r2 });
		ret.Parameters.Add(new ParameterDouble() { Name = "Height", Value = height });
		ret.Parameters.Add(new ParameterDouble() { Name = "Width", Value = width });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tf", Value = tf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tw", Value = tw });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tapperf", Value = tapperf });
		ret.Parameters.Add(new ParameterDouble() { Name = "Tapperw", Value = tapperw });
		return ret;
	}
	public CrossSectionParameter AddTrapezoid1(System.Double h, System.Double bt, System.Double bb, System.Double zt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bt", Value = bt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Bb", Value = bb });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		return ret;
	}
	public CrossSectionParameter AddTrapezoid3(System.Double b, System.Double h, System.Double btl, System.Double btr, System.Double yt, System.Double zt)
	{
		var ret = new CrossSectionParameter();
		ret.Parameters.Add(new ParameterDouble() { Name = "B", Value = b });
		ret.Parameters.Add(new ParameterDouble() { Name = "H", Value = h });
		ret.Parameters.Add(new ParameterDouble() { Name = "Btl", Value = btl });
		ret.Parameters.Add(new ParameterDouble() { Name = "Btr", Value = btr });
		ret.Parameters.Add(new ParameterDouble() { Name = "Yt", Value = yt });
		ret.Parameters.Add(new ParameterDouble() { Name = "Zt", Value = zt });
		return ret;
	}
}
	 * */
}