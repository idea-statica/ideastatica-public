namespace IdeaStatiCa.Api.Connection.Model.Parameters
{
	/// <summary>
	/// The quantity a parameter's value stands for - the application's <i>Value type</i> column. It
	/// decides the unit the application displays the value in; values are always written in SI.
	/// <para>
	/// Every value type a parameter can be read back with is listed. A parameter created or updated
	/// over the API can have only the ones its parameter type can hold: <c>Float</c> the decimal
	/// quantities, <c>Int</c> <see cref="WholeNumber"/>, <c>Bool</c> <see cref="Boolean"/>,
	/// <c>String</c> <see cref="Text"/>, <c>Expression</c> any of those, and every type
	/// <see cref="Generic"/>.
	/// </para>
	/// </summary>
	public enum ConParameterValueType
	{
		/// <summary>No particular quantity. The value type of a parameter that is not given one.</summary>
		Generic,
		Text,
		WholeNumber,
		Boolean,
		BoltPosition,
		Vector,
		CrossSection,
		WeldMaterial,
		SteelMaterial,
		ConcreteMaterial,
		BoltAssembly,
		LengthStructure,
		LengthCrossSection,
		LengthComponent,
		Angle,
		Force,
		Moment,
		Stress,
		Temperature,
		RelativeHumidity,
		Mass,
		Weight,
		HeatTransferConvection,
		Area,
		Coeff,
		PlateThickness,
		WeldSize,
		Dropdown,
	}
}
