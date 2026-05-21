using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yjk.Helpers
{
	public class BimApiData
	{
		public List<Mapping> Mappings { get; set; }
	}

	public class Mapping
	{
		public int Item1 { get; set; }
		public string Item2 { get; set; }
	}
}
