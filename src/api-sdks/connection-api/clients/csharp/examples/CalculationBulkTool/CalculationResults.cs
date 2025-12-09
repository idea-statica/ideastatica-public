using IdeaRS.OpenModel.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationBulkTool
{
	public class CalculationResults
	{
		public string? FileName { get; set; } // IdeaCon

		public List<Connection> ProjectItems { get; set; } = new List<Connection>();
	}

	public class Connection
	{
		public string? Name { get; set; }
		public string? AnalysisType { get; set; }
		public double? Analysis { get; set; }
		public double? Bolts { get; set; }
		public double? Welds { get; set; }
		public double? Plates { get; set; }
		public double? PreloadedBolts { get; set; }

		public bool Succes { get; set; }

		public List<BoltResults> BoltResults { get; set; } = new List<BoltResults> { };

	}

	public class BoltResults
	{
		public string? Item { get; set; }

		public double UtilizationInTension { get; set; } // Utt unityCheckTension

		public double UtilizationInShear { get; set; } // Uts unityCheckShearMax

		public double UtilizationInInteraction { get; set; } // Utts interactionTensionShear
	}

}
