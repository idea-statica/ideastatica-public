namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represents named and identifiable BIM object.
	/// <para>
	/// Names of the objects are general strings and there are no guarantees about their content. The names can be null strings, empty
	/// or any generic text value, event duplicated across the model, if the original BIM software allows that.
	/// </para>
	/// <para>
	/// Identifiers of the objects are strings that are guaranteed to be:
	/// <list type="bullet">
	///		<item>globally unique within the scope of the current model.</item>
	///		<item>constant when the model is changed.</item>
	/// </list>
	/// are qua
	/// </para>
	/// </summary>
	public interface IIdeaObject
	{
		/// <summary>
		/// Identification of the object. The ids of the objects are guaranteed to be unique withing
		/// the scope of the model and constant. See <see cref="IIdeaObject"/> interface comment for more details.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Name of the object, or null if the object does not have a name. The object name is not guaranteed to be defined or unique.
		/// </summary>
		string Name { get; }
	}
}