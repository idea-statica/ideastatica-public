﻿using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Provides conversion between BimApi and IOM.
	/// </summary>
	public interface IBimImporter
	{
		/// <summary>
		/// Imports connections into IOM.
		/// </summary>
		/// <returns>ModelBIM object.</returns>
		/// <exception cref="ConstraintException">Thrown when some constrain imposed on BimApi data is broken.</exception>
		ModelBIM ImportConnections();

		/// <summary>
		/// Imports members into IOM.
		/// </summary>
		/// <returns>ModelBIM object.</returns>
		/// <exception cref="ConstraintException">Thrown when some constrain imposed on BimApi data is broken.</exception>
		ModelBIM ImportMembers();

		/// <summary>
		/// Reimports previously imported objects. 
		/// </summary>
		/// <param name="selected">List of objects to reimport.</param>
		/// <returns>List of ModelBIM objects.</returns>
		List<ModelBIM> ImportSelected(List<BIMItemsGroup> selected);
	}
}