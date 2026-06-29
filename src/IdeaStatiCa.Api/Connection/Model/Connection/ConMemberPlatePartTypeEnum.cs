namespace IdeaStatiCa.Api.Connection.Model
{
	public enum ConMemberPlatePartTypeEnum : int
	{
		NotSpecified = 0,
		TopFlange = 1,
		BottomFlange = 2,
		Web = 4,
		BasePlate = 8,
		EndPlate = 16,
		PlateWidener = 32,
		Stiffener = 64,
		Rib = 128,
		GussetPlate = 256,
		FinPlate = 512,
		Flange = 1024,
		CssArcSegment = 2048,
		IsStub = 4096,
		Splice = 8192,
		TonguePlate = 16384,
		LidPlate = 32768,
		GeneralPlate = 65536,
		Doubler = GeneralPlate * 2,
		EndPlateOnFlanges = Doubler * 2,
		BackingPlate = EndPlateOnFlanges * 2,
		InsertedPlate = BackingPlate * 2,
		IsNegative = InsertedPlate * 2,
		BothFlanges = 3,
		AllCssParts = 7
	}
}
