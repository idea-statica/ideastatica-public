using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace IdeaStatiCa.IntermediateModel
{
	public class IRExportToXMLService : IIRExportToXMLService
	{
		readonly string VersionKey = "version";
		readonly string EncodingKey = "encoding";

		protected readonly IPluginLogger _logger;
		public IRExportToXMLService(IPluginLogger logger)
		{
			this._logger = logger;
		}

		/// <inheritdoc />
		public string ExportToXml(SModel irModel)
		{
			_logger.LogInformation("ExportToXml");

			XDeclaration xDeclaration = GetDeclaration(irModel);

			Stack<IXmlLineInfo> xmlItems = ProcessModelItems(irModel);

			//there should be left root item
			if (xmlItems.Count != 1)
			{
				throw new InvalidOperationException($"ExportToXml:ProcessModelItems failed model in not correct. Should be one root element instead of {xmlItems.Count}");
			}
			var xml = xmlItems.Pop();

			XDocument xDocument = new XDocument(xDeclaration, xml);

			_logger.LogInformation("ExportToXml XDocument into a string");
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

		private Stack<IXmlLineInfo> ProcessModelItems(SModel irModel)
		{
			_logger.LogInformation("ProcessMoStack<ISIntermediate>Items");

			var itemsTOProcess = new Stack<ISIntermediate>();
			itemsTOProcess.Push(irModel.RootItem);

			Stack<IXmlLineInfo> xmlItems = new Stack<IXmlLineInfo>();

			Dictionary<string, XNamespace> usedNamespaces = new Dictionary<string, XNamespace>();
			while (itemsTOProcess.Count > 0)
			{
				ISIntermediate item = itemsTOProcess.Pop();
				switch (item)
				{
					case SPrimitive primitive:
						{
							ProcessPrimitiveItem(xmlItems, primitive);
							break;
						}

					case SAttribute attribute:
						{
							ProcessAttributeItem(xmlItems, usedNamespaces, attribute);
							break;
						}

					case SList list:
						{
							ProcessListItem(itemsTOProcess, list);
							break;
						}

					case SObject sObject:
						{
							ProcessObjectItem(irModel, itemsTOProcess, xmlItems, usedNamespaces, sObject);
							break;
						}

					case EndOfObject _:
						{
							ProcessEndItem(xmlItems);

							break;
						}

					default:
						throw new InvalidOperationException($"Unknown object {item.GetType().FullName}");
				}
			}

			return xmlItems;
		}

		private void ProcessEndItem(Stack<IXmlLineInfo> xmlItems)
		{
			_logger.LogTrace("Process End Of Object");
			var closedItem = xmlItems.Pop();
			//assign closed element to parent

			if (xmlItems.Count > 0)
			{
				IXmlLineInfo parentXml = xmlItems.Peek();
				if (parentXml is XElement parentElementXml)
				{
					parentElementXml.Add(closedItem);
				}
				else
				{
					throw new InvalidOperationException($"Parent shod be XElement not {parentXml.GetType().FullName}");
				}
			}
			else
			{
				//return root item in to stack
				xmlItems.Push(closedItem);
			}
		}

		private void ProcessObjectItem(SModel irModel, Stack<ISIntermediate> itemsTOProcess, Stack<IXmlLineInfo> xmlItems, Dictionary<string, XNamespace> usedNamespaces, SObject sObject)
		{
			_logger.LogTrace($"Process object {sObject.GetElementName()}");
			//add information about closing tag
			itemsTOProcess.Push(new EndOfObject());

			// add to process all item Properties - reverse due to stack 
			foreach (var dictionaryItem in sObject.Properties.AsEnumerable().Reverse())
			{
				itemsTOProcess.Push(dictionaryItem.Value);
			}

			var newXmlItem = new XElement(sObject.TypeName);

			//for root element set name-spaces attributes
			if (xmlItems.Count == 0 && irModel.RootNameSpaces.Count > 0)
			{
				foreach (var namespaceItem in irModel.RootNameSpaces)
				{
					_logger.LogTrace($"Process object set root Namespace {namespaceItem.Value.NameSpace} Namespace value {namespaceItem.Value.Value}");
					XNamespace xNamespace = namespaceItem.Value.NameSpace;
					XNamespace @namespaceValue = namespaceItem.Value.Value;
					newXmlItem.Add(new XAttribute(xNamespace + namespaceItem.Value.LocalName, @namespaceValue.NamespaceName));
					usedNamespaces[namespaceItem.Value.LocalName] = namespaceValue;
				}
			}

			//create new xml element
			xmlItems.Push(newXmlItem);
		}

		private void ProcessListItem(Stack<ISIntermediate> itemsTOProcess, SList list)
		{
			_logger.LogTrace($"Process list {list.GetElementName()}");
			foreach (var listItem in list.AsEnumerable().Reverse())
			{
				itemsTOProcess.Push(listItem);
			}
		}

		private void ProcessAttributeItem(Stack<IXmlLineInfo> xmlItems, Dictionary<string, XNamespace> usedNamespaces, SAttribute attribute)
		{
			_logger.LogTrace($"Process attribute {attribute.GetElementValue()}");
			var parentXml = xmlItems.Peek();
			if (parentXml is XElement parentElementXml)
			{
				//for root item are attributes with definition of name-spaces already set
				if (usedNamespaces.TryGetValue(attribute.Prefix, out XNamespace ns))
				{
					parentElementXml.Add(new XAttribute(ns + attribute.LocalName, attribute.Value));
				}
			}
			else
			{
				throw new InvalidOperationException($"Parent should be XElement not {parentXml.GetType().FullName}");
			}
		}

		private void ProcessPrimitiveItem(Stack<IXmlLineInfo> xmlItems, SPrimitive primitive)
		{
			_logger.LogTrace($"Process primitive {primitive.GetElementValue()}");
			var parentXml = xmlItems.Peek();

			if (parentXml is XElement parentElementXml)
			{
				parentElementXml.Value = (!string.IsNullOrEmpty(primitive.Value) ? primitive.Value.ToString() : "");
			}
			else
			{
				throw new InvalidOperationException($"Parent should be XElement not {parentXml.GetType().FullName}");
			}
		}

		private XDeclaration GetDeclaration(SModel irModel)
		{
			_logger.LogInformation("GetDeclaration");
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
				_logger.LogInformation($"GetDeclaration version {version}, encoding {encoding}");
				xDeclaration = new XDeclaration(version, encoding, standalone);
			}
			else
			{
				_logger.LogInformation("GetDeclaration missing declaration");
			}

			return xDeclaration;
		}
	}
}
