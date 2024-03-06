using System;
using System.Collections.Generic;

namespace IdeaStatiCa.IntermediateModel.IRModel
{
	//This object is used only for exporting to the XML file 
	class EndOfObject : ISIntermediate
	{
		/// <inheritdoc />
		public void AddElementProperty(ISIntermediate property)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public IEnumerable<ISIntermediate> GetElement(Queue<string> filter)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public string GetElementName()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public ISIntermediate TakeElementProperty(string filter)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void ChangeElementPropertyName(string name, string newName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void ChangeElementValue(string newValue)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public string GetElementValue(string property = null)
		{
			throw new NotImplementedException();
		}
	}
}
