using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Parameter;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Settings;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	public class ConConnection
	{
		//Need to choose to either use Id or Identifier for connection
		public int Id { get; set; }

		public string Identifier { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		//Related to connection?
		public ConAnalysisTypeEnum AnalysisType { get; set; }

		public ConLoadingOptions LoadOptions { get; set; }

		public int BearingMemberId { get; set; }

		public int AnalizedMemberId { get; set; }

		public bool IsCalculated { get; }

		//Decide whether to put this information in here or not.

		public IEnumerable<ConLoadEffect> LoadEffects { get; set; } = new List<ConLoadEffect> { };

		public IEnumerable<ConMember> Members { get; set; } = new List<ConMember> { };

		public IEnumerable<ConOperation> Operations { get; set; } = new List<ConOperation> { };

		public IEnumerable<IdeaParameter> Parameters { get; set; } = new List<IdeaParameter> { };
	}
}
