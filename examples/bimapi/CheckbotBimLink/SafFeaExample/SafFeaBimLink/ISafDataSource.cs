namespace SafFeaBimLink
{
	public interface ISafDataSource
	{
		/// <summary>
		/// //Method that needs to provide the endpoint filepath of the project so that a checkbot project folder can be created in the same directory.
		/// </summary>
		/// <returns></returns>
		string GetModelDirectory();


		/// <summary>
		/// Get the Name of the Active Fea Model. This is used to create the Checkbot FileName.
		/// </summary>
		/// <returns></returns>
		string GetModelName();

		/// <summary>
		/// A method which can export and save the SAF file from the actively selected items in the FEA Application.
		/// </summary>
		/// <remarks>
		/// Here we are using Guids for selection of objects in the Fea Model. 
		/// This method should be modified to suit your needs and can use any type (int, string, etc) as the unique identifier which is used in the Fea Program.
		/// SAF file should include any information required to build the model, including Points, Members, Cross-sections, Materials 
		/// and 1D Member results of the current active Member and Point selection.
		/// SAF requires that UId's are exported with the objects. It is important that these Uid's are persistant with each export of the same object.
		/// </remarks>
		/// <param name="safSavePath"></param>
		/// <param name="selectedElementGuids"></param>
		/// <returns>The filepath of the saved SAF file</returns>
		string ExportSAFFileofActiveSelection(string safSavePath, out IReadOnlyCollection<Guid> selectedElementGuids);

		/// <summary>
		/// A method which can export a selection of elements in the given model based on a given list of Guids in the model.
		/// This is used for syncing an updated model within Checkbot
		/// </summary>
		/// <remarks>
		/// To enable syncing in Checkbot. Checkbot needs to ask the Fea app for a SAF file of all the current members in Checkbot. 
		/// This is similar to the <see cref="ExportSAFFileofActiveSelection(string, out IReadOnlyCollection{Guid})"/>
		/// SAF requires that UId's are exported with the objects. It is important that these Uid's are persistant with each export of the same object. We map these Uids with the Unique indentifier in the program.
		/// </remarks>
		/// <param name="safSavePath"></param>
		/// <param name="providedElementGuids"></param>
		/// <returns></returns>
		string ExportSAFFileofProvidedSelection(string safSavePath, IEnumerable<Guid> providedElementGuids);
	}
}
