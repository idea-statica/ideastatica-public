

namespace SafFeaBimLink
{
	internal class ModelClient
	{
		private ISafDataSource _feaModel;
		private readonly string _safFile;

		public ModelClient(ISafDataSource feaModel)
		{
			_feaModel = feaModel;
			_safFile = Path.GetTempFileName();
		}

		/// <summary>
		/// Provides a saf file of a selection as provided from Checkbot.
		/// </summary>
		/// <param name="guids"></param>
		/// <returns></returns>
		public async Task<string> GetObjects(IEnumerable<Guid> guids)
		{
			_feaModel.ExportSAFFileofProvidedSelection(_safFile, guids);

			return _safFile;
		}

		/// <summary>
		/// Gets the active selection within the base program and exports it to a SAF file
		/// </summary>
		/// <param name="withSurroundings"></param>
		/// <returns></returns>
		public async Task<(string Path, IReadOnlyCollection<Guid> Selected)> GetSelection(bool withSurroundings = false)
		{
			IReadOnlyCollection<Guid> selected;

			_feaModel.ExportSAFFileofActiveSelection(_safFile, out selected);

			return (_safFile, selected);
		}
	}
}
