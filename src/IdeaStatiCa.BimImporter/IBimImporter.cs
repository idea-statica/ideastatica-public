using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Provides conversion between BimApi and IOM.
	/// </summary>
	public interface IBimImporter
	{
		/// <summary>
		/// Imports specified objects into IOM.
		/// </summary>
		/// <param name="objects">Objects to import.</param>
		/// <returns>ModelBIM object.</returns>
		/// <param name="countryCode"></param>
		ModelBIM Import(IEnumerable<IIdeaObject> objects, CountryCode countryCode);

		/// <summary>
		/// Import single connections into IOM. Typycaly complicated connection from cad
		/// </summary>
		/// <returns>ModelBIM object.</returns>
		/// <exception cref="ConstraintException">Thrown when some constrain imposed on BimApi data is broken.</exception>
		ModelBIM ImportSingleConnection(CountryCode countryCode);

		/// <summary>
		/// Imports connections into IOM.
		/// </summary>
		/// <param name="countryCode"></param>
		/// <returns>ModelBIM object.</returns>
		/// <exception cref="ConstraintException">Thrown when some constrain imposed on BimApi data is broken.</exception>
		ModelBIM ImportConnections(CountryCode countryCode);

		/// <summary>
		/// Import whole model into IOM.
		/// </summary>
		/// <param name="countryCode"></param>
		/// <returns>ModelBIM object.</returns>
		/// <exception cref="ConstraintException">Thrown when some constrain imposed on BimApi data is broken.</exception>
		ModelBIM ImportWholeModel(CountryCode countryCode);

		/// <summary>
		/// Imports members into IOM.
		/// </summary>
		/// <param name="countryCode"></param>
		/// <returns>ModelBIM object.</returns>
		/// <exception cref="ConstraintException">Thrown when some constrain imposed on BimApi data is broken.</exception>
		ModelBIM ImportMembers(CountryCode countryCode);

		/// <summary>
		/// Imports the given members into IOM. Reads no selection from the model and adds none of the members
		/// framing into their end nodes, so the result holds exactly the members passed in.
		/// </summary>
		/// <remarks>
		/// For a caller that already holds the members: they are read from the BIM application once.
		/// <see cref="ImportSelected"/> addresses items by IOM id and resolves each one back into an object,
		/// which reads every entity a second time.
		/// <para>
		/// The members are given the model's leading IOM ids, ahead of the geometry they reference. A member
		/// that is null is skipped, which is what an unresolvable entity arrives as. Details are not supported
		/// here — a <see cref="RequestedItemsType.Substructure"/> group can also carry them, this cannot.
		/// </para>
		/// </remarks>
		/// <param name="members">Members to import.</param>
		/// <param name="countryCode"></param>
		/// <returns>ModelBIM object.</returns>
		/// <exception cref="System.ArgumentNullException">Throws when <paramref name="members"/> is null.</exception>
		ModelBIM ImportSubstructure(IEnumerable<IIdeaMember1D> members, CountryCode countryCode);

		/// <summary>
		/// Reimports previously imported objects.
		/// </summary>
		/// <param name="selected">List of objects to reimport.</param>
		/// <param name="countryCode"></param>
		/// <returns>List of ModelBIM objects.</returns>
		List<ModelBIM> ImportSelected(List<BIMItemsGroup> selected, CountryCode countryCode);

		/// <summary>
		/// Imports Members2D into IOM.
		/// </summary>
		/// <param name="countryCode"></param>
		/// <returns>ModelBIM object.</returns>
		/// <exception cref="ConstraintException">Thrown when some constrain imposed on BimApi data is broken.</exception>
		ModelBIM ImportMembers2D(CountryCode countryCode);
	}
}