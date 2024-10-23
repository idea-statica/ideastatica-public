using SafFeaApi_MOCK;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafFeaBimLink
{
    internal class ModelClient
    {
		private FeaModelApiClient _feaModel;
		private readonly string _safFile;

		public ModelClient(FeaModelApiClient femModel)
		{
			_feaModel = femModel;
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
