using System;

namespace IdeaStatiCa.BimImporter
{
    /// <summary>
    /// Exception thrown when some contrain on data returned from BimApi is broken.
    /// </summary>
    public class ConstraintException : Exception
    {
        public ConstraintException(string message) : base(message)
        {
        }
    }
}