using IdeaStatiCa.IntermediateModel.IRModel;
using System.Xml;
using System.Xml.Linq;

namespace IdeaStatiCa.IntermediateModel
{
	public class IRExportToXMLService : IIRExportToXMLService
	{
		readonly string VersionKey = "version";
		readonly string EncodingKey = "encoding";

		public string ExportToXml(SModel irModel)
		{

			XDeclaration xDeclaration = GetDeclaration(irModel);

			Stack<IXmlLineInfo> xmlItems = ProcessModelItems(irModel);

			//there should be left root item
			var xml = xmlItems.Pop();

			XDocument xDocument = new XDocument(xDeclaration, xml);

			// Save XDocument into a string using XmlWriter
			string xmlString;
			using (StringWriter stringWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
				{
					xDocument.Save(xmlWriter);
				}
				xmlString = stringWriter.ToString();
			}

			return xmlString;
		}

		private static Stack<IXmlLineInfo> ProcessModelItems(SModel irModel)
		{
			var itemsTOProcess = new Stack<ISIntermediate>();
			itemsTOProcess.Push(irModel.RootItem);

			Stack<IXmlLineInfo> xmlItems = new Stack<IXmlLineInfo>();

			Dictionary<string, XNamespace> usedNamespaces = new Dictionary<string, XNamespace>();
			while (itemsTOProcess.TryPop(out ISIntermediate item))
			{
				switch (item)
				{
					case SPrimitive primitive:
						{
							var parentXml = xmlItems.Peek();

							if (parentXml is XElement parentElementXml)
							{
								parentElementXml.Value = primitive.Value.ToString();
							}
							else
							{
								throw new Exception($"Parent shod be XElement not {parentXml.GetType().FullName}");
							}

							break;
						}

					case SAttribute attribute:
						{
							var parentXml = xmlItems.Peek();
							if (parentXml is XElement parentElementXml)
							{
								//for root item are attributes with definition of namespaces already set
								if (usedNamespaces.TryGetValue(attribute.Prefix, out XNamespace ns))
								{
									parentElementXml.Add(new XAttribute(ns + attribute.LocalName, attribute.Value));
								}
							}
							else
							{
								throw new Exception($"Parent shod be XElement not {parentXml.GetType().FullName}");
							}

							break;
						}

					case SList list:
						{
							foreach (var listItem in list.AsEnumerable().Reverse())
							{
								itemsTOProcess.Push(listItem);
							}

							break;
						}

					case SObject sObject:
						{
							//add information about closing tag
							itemsTOProcess.Push(new EndOfObject());

							// add to process all item Properties - reverse due to stack 
							foreach (var dictionaryItem in sObject.Properties.AsEnumerable().Reverse())
							{
								itemsTOProcess.Push(dictionaryItem.Value);
							}

							var newXmlItem = new XElement(sObject.TypeName);

							//for root element set namespaces atributes
							if (xmlItems.Count == 0 && irModel.RootNameSpaces.Count > 0)
							{
								foreach (var namespaceItem in irModel.RootNameSpaces)
								{
									XNamespace xNamespace = namespaceItem.Value.NameSpace;
									XNamespace @namespaceValue = namespaceItem.Value.Value;
									newXmlItem.Add(new XAttribute(xNamespace + namespaceItem.Value.LocalName, @namespaceValue.NamespaceName));
									usedNamespaces[namespaceItem.Value.LocalName] = namespaceValue;
								}

							}

							//create new xml element
							xmlItems.Push(newXmlItem);
							break;
						}

					case EndOfObject:
						{

							var closedItem = xmlItems.Pop();
							//assign closed element to parent
							if (xmlItems.TryPeek(out IXmlLineInfo parentXml))
							{
								if (parentXml is XElement parentElementXml)
								{
									parentElementXml.Add(closedItem);
								}
								else
								{
									throw new Exception($"Parent shod be XElement not {parentXml.GetType().FullName}");
								}
							}
							else
							{
								//return root item in to stack
								xmlItems.Push(closedItem);
							}

							break;
						}

					default:
						throw new Exception($"Unkonw object {item.GetType().FullName}");
				}
			}

			return xmlItems;
		}

		private XDeclaration GetDeclaration(SModel irModel)
		{
			XDeclaration xDeclaration = null;
			if (irModel.ModelDeclaration is SObject modelDeclaration)
			{
				string version = string.Empty;
				string encoding = string.Empty;
				string standalone = string.Empty;


				if (modelDeclaration.Properties.ContainsKey(VersionKey) && modelDeclaration.Properties[VersionKey] is SAttribute versionAttribute)
				{
					version = versionAttribute.Value;
				}

				if (modelDeclaration.Properties.ContainsKey(EncodingKey) && modelDeclaration.Properties[EncodingKey] is SAttribute encodingAttribute)
				{
					encoding = encodingAttribute.Value;
				}

				xDeclaration = new XDeclaration(version, encoding, standalone);
			}

			return xDeclaration;
		}

		private class EndOfObject : ISIntermediate
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
}
