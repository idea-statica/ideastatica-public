namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Which end of a stiffening member is kept when the member is cut.
	/// Maps to the new-model <c>ConnectedBy</c> enum. Only meaningful when the cut target
	/// is a stiffening member.
	/// </summary>
	public enum ConRemainingPart
	{
		Begin,
		End,
	}
}
