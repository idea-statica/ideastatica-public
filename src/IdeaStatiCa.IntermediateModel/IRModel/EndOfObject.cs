namespace IdeaStatiCa.IntermediateModel.IRModel
{
	//This object is used only for exporting to the XML file 
	class EndOfObject : ISIntermediate
	{
		public void AddElementProperty(ISIntermediate property)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ISIntermediate> GetElement(Queue<string> filter)
		{
			throw new NotImplementedException();
		}

		public string GetElementName()
		{
			throw new NotImplementedException();
		}

		public ISIntermediate TakeElementProperty(string filter)
		{
			throw new NotImplementedException();
		}

		public void ChangeElementPropertyName(string name, string newName)
		{
			throw new NotImplementedException();
		}

		public void ChangeElementValue(string newValue)
		{
			throw new NotImplementedException();
		}

		public string GetElementValue(string property = null)
		{
			throw new NotImplementedException();
		}
	}
}
