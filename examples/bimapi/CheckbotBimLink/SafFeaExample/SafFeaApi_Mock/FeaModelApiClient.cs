using System.Collections.ObjectModel;

namespace SafFeaApi_MOCK
{
	/// <summary>
	/// The list of required methods that a FEA App API client is required to enable creation of a full SAF based BIM link with IDEA StatiCA Checkbot. 
	/// </summary>
	/// <remarks>
	/// These methods need to be able to be called from Checkbot therefore should be publicly avaliable.
	/// </remarks>
	public class FeaModelApiClient
	{
		/// <summary>
		/// //Method that needs to provide the endpoint filepath of the project so that a checkbot project folder can be created in the same directory.
		/// </summary>
		/// <returns></returns>
		public string GetModelFilePath()
		{
			return "c:/ideatest.str";
		}

		/// <summary>
		/// Get the Name of the Active Fea Model. This is used to create the Checkbot FileName.
		/// </summary>
		/// <returns></returns>
		public string GetModelName()
		{
			return "ideatest";
		}

		/// <summary>
		/// A method which can export and save the SAF file from the actively selected items in Fem-Design.
		/// </summary>
		/// <remarks>
		/// Here we are using Guids for selection of objects in the Fea Model. 
		/// This method should be can be modified to suit your needs and use any type of Unique Identifier which is used in the Fea Program.
		/// SAF file should include any information required to build the model, including Points, Members, Cross-sections, Materials 
		/// and 1D Member results of the current active Member and Point selection.
		/// </remarks>
		/// <returns>The filepath of the saved SAF file</returns>
		public string ExportSAFFileofActiveSelection(string safSavePath, out IReadOnlyCollection<Guid> selectedElementGuids)
		{
			List<Guid> Guids = new List<Guid>();

			//FIX
			Guids.Add(new Guid("b334ca71 - c82b - 4445 - 9771 - ad83b1e9361e"));
			Guids.Add(new Guid("91aae2e3 - 715b - 4da3 - 8527 - a6fd6e5f2a38"));
			Guids.Add(new Guid("7a35a940 - e1fd - 4ce8 - af65 - 7b20f9f619e9"));

			ReadOnlyCollection<Guid> readOnlyDinosaurs = new ReadOnlyCollection<Guid>(Guids);

			selectedElementGuids = readOnlyDinosaurs;


			return "d:/ideatest.xlsx";
		}

		/// <summary>
		/// A method which can export a selection of elements in the given model based on a given list of Guids in the model.
		/// This is used for syncing an updated model within Checkbot
		/// </summary>
		/// <remarks>
		/// To enable syncing in Checkbot. Checkbot needs to ask the Fea app for a SAF file of all the current members in Checkbot. 
		/// This is similar to the <see cref="ExportSAFFileofActiveSelection(string, out IReadOnlyCollection{Guid})"/>
		/// </remarks>
		/// <param name="safSavePath"></param>
		/// <param name="selectedElementGuids"></param>
		/// <returns></returns>
		public string ExportSAFFileofProvidedSelection(string safSavePath, IEnumerable<Guid> selectedElementGuids)
		{
			return "d:/ideatest.xlsx";
		}
	}
}
