namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Discriminator for <see cref="Operations.ConWeldData"/> polymorphism. Drives JSON
	/// (de)serialization into the right concrete subtype.
	/// </summary>
	public enum ConWeldDataKind
	{
		/// <summary>Plain continuous weld — base <see cref="Operations.ConWeldData"/>.</summary>
		Continuous,

		/// <summary>Intermittent weld with begin/end offsets and length/gap pattern.</summary>
		Intermittent,
	}
}
