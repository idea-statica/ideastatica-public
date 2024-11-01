using SafFeaBimLink;
using System.Collections.ObjectModel;

namespace SafFeaApi_MOCK
{
	/// <summary>
	/// The list of required methods that a FEA App API client is required to enable creation of a full SAF based BIM link with IDEA StatiCA Checkbot. 
	/// </summary>
	/// <remarks>
	/// These methods need to be able to be called from Checkbot therefore should be publicly avaliable.
	/// </remarks>
	public class FeaModelApiClient : ISafDataSource
	{
		/// <inheritdoc cref="ISafDataSource.GetModelDirectory"/>
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

		/// <inheritdoc cref="ISafDataSource.GetModelName"/>"/>
		public string GetModelName()
		{
			//We will create a dumby model name.

			return "steel_truss";
		}

		/// <inheritdoc cref="ISafDataSource.ExportSAFFileofActiveSelection(string, out IReadOnlyCollection{Guid})"/>
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


		/// <inheritdoc cref="ISafDataSource.ExportSAFFileofProvidedSelection(string, IEnumerable{Guid})"/>
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
