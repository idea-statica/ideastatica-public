namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Extended IIdeaModel by method related by connection
	/// </summary>
	public interface IIdeaConnectionModel : IIdeaModel
	{
		/// <summary>
		/// Process connected beams in connection
		/// </summary>
		void ProcessConnection(IIdeaConnectionPoint connectionPoint);
	}
}
