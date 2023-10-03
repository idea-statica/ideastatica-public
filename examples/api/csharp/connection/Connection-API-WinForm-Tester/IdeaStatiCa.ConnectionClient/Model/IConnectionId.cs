using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Model
{
	public interface IConnectionId
	{
		string Name
		{
			get;
		}

		string ConnectionId
		{
			get;
		}
	}
}
