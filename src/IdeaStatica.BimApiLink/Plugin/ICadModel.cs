using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.Plugin
{
	public interface ICadModel : IModel
	{
		CadUserSelection GetUserSelection();

		IEnumerable<CadUserSelection> GetUserSelections();

		IEnumerable<CadUserSelection> GetSelectionOfWholeModel();

	}
}
