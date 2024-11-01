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
		public string GetModelDirectory()
		{
			//Return a dumby file path to the model directory.
			//Assume model is saved on the desktop.

			string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			string folderPath = Path.Combine(desktopPath, "EXAMPLE_FeaModelLocation");

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			return folderPath;
		}

		/// <summary>
		/// Get the Name of the Active Fea Model. This is used to create the Checkbot FileName.
		/// </summary>
		/// <returns></returns>
		public string GetModelName()
		{
			//We will create a dumby model name.

			return "steel_truss";
		}

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
		/// <returns>The filepath of the saved SAF file</returns>
		public string ExportSAFFileofActiveSelection(string safSavePath, out IReadOnlyCollection<Guid> selectedElementGuids)
		{
			//REQUIREMENT
			//FEA APPLICATION NEEDS TO CREATE A SAF FILE BASED ON THE CURRENTLY SELECTED MEMBERS AND POINTS

			//DUBY OUTPUT: List of selected elements in the current FEA application 
			List<Guid> Guids = new List<Guid>();
			//Add a dumby guid so the program detects atleast one item to Import to bypass selection check.
			Guids.Add(Guid.NewGuid());
			selectedElementGuids = new ReadOnlyCollection<Guid>(Guids);


			//FOR NOW WE WILL COPY A SAMPLE SAF FILE TO THE GENERATED SAF FILE LOCATION 
			string sourceFilePath = "Inputs\\SAF_steel_truss_first_import.xlsx";   

			File.Copy(sourceFilePath, safSavePath, true);

			return safSavePath;
		}


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
		/// <param name="selectedElementGuids"></param>
		/// <returns></returns>
		public string ExportSAFFileofProvidedSelection(string safSavePath, IEnumerable<Guid> providedElementGuids)
		{

			//FEA APPLICATION NEEDS TO CREATE A SAF FILE BASED ON THE PROVDIED GUIDS OF MEMBERS AND POINTS

			//FOR NOW WE WILL COPY A MODIFIED VERSION OF THE SAMPLE SAF FILE TO THE GENERATED SAF FILE LOCATION 
			string sourceFilePath = "Inputs\\SAF_steel_truss_sync.xlsx";

			File.Copy(sourceFilePath, safSavePath, true);

			return safSavePath;
		}
	}
}
