using System;

namespace IdeaStatiCa.BimImporter
{
	public class ConstraintException : Exception
	{
		public ConstraintException() : base()
		{
		}

		public ConstraintException(string message) : base(message)
		{
		}
	}
}