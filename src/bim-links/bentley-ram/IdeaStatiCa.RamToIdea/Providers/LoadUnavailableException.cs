using System;

namespace IdeaStatiCa.RamToIdea.Providers
{
	internal class LoadUnavailableException : Exception
	{
		public LoadUnavailableException(int uid)
			: base($"Load case with UID {uid} is not available.")
		{

		}
	}
}
