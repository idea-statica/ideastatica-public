using IdeaStatiCa.IntermediateModel.IRModel;
using System.Xml;

namespace IdeaStatiCa.IntermediateModel
{
	// Implementation of the intermediate service
	public class XmlParsingService : IXmlParsingIRService
	{
		public SModel ParseXml(string xmlContent)
		{
			var processItemStack = new Stack<SObject>();
			var model = new SModel();

			using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlContent)))
			{
				while (reader.MoveToNextAttribute() || reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							ProcessStartElement(reader, processItemStack, model);
							break;
						case XmlNodeType.Text:
							ProcessText(reader, processItemStack);
							break;
						case XmlNodeType.Attribute:
							ProcessAttribute(reader, processItemStack);
							break;
						case XmlNodeType.EndElement:
							ProcessEndElement(processItemStack);
							break;
					}
				}
			}

			return model;
		}

		private void ProcessStartElement(XmlReader reader, Stack<SObject> processItemStack, SModel model)
		{
			var element = new SObject { TypeName = reader.Name };

			//if its not root element assign element to the parent
			if (processItemStack.Count > 0)
			{
				var parent = processItemStack.Peek();
				AddToParent(element, parent);
			}
			//new parent
			processItemStack.Push(element);

			if (model.RootItem == null)
			{
				model.RootItem = element;
			}
		}

		private void AddToParent(SObject element, SObject parent)
		{
			if (parent.Properties.ContainsKey(element.TypeName))
			{
				if (parent.Properties[element.TypeName] is SList sList)
				{
					sList.Items.Add(element);
				}
			}
			else
			{
				// set reference as potential collection it will be fixed on the end of parent element
				parent.Properties[element.TypeName] = new SList { Items = new List<ISIntermediate> { element } };
			}
		}

		private void ProcessText(XmlReader reader, Stack<SObject> processItemStack)
		{
			if (processItemStack.Count > 0)
			{
				processItemStack.Peek().Properties["Value"] = new SPrimitive { Value = reader.Value };
			}
		}

		private void ProcessAttribute(XmlReader reader, Stack<SObject> processItemStack)
		{
			if (processItemStack.Count > 0)
			{
				processItemStack.Peek().Properties[reader.Name] = new SAttribute { Key = reader.Name, Value = reader.Value };
			}
		}

		private void ProcessEndElement(Stack<SObject> processItemStack)
		{
			if (processItemStack.Count > 0)
			{
				var filled = processItemStack.Pop();
				FixReferences(filled);
			}
		}

		private void FixReferences(SObject element)
		{
			var properties = new Dictionary<string, ISIntermediate>();

			foreach (var item in element.Properties)
			{
				if (item.Value is SList sList && sList.Items.Count == 1)
				{
					properties[item.Key] = sList.Items.First();
				}
				else
				{
					properties[item.Key] = item.Value;
				}
			}

			element.Properties = properties;
		}
	}
}
