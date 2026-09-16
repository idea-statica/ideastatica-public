namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Concrete anchor type. ThreadedRod / GeneralAnchor are post-installed; HookedAnchor /
	/// HeadedStud / WasherPlate* / Reinforcement are cast-in-place.
	/// </summary>
	public enum ConAnchorType
	{
		ThreadedRod,
		GeneralAnchor,
		HookedAnchor,
		HeadedStud,
		WasherPlate,
		Reinforcement,
	}
}
