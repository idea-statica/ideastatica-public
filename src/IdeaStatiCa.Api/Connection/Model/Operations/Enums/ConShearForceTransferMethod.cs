namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// How shear force is transferred between the base plate and the concrete block.
	/// </summary>
	public enum ConShearForceTransferMethod
	{
		Friction,
		ShearLug,
		AnchorBending,
		Sliding,
	}
}
