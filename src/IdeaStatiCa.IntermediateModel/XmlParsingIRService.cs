﻿using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace IdeaStatiCa.IntermediateModel
{
	// Implementation of the intermediate service
	public class XmlParsingService : IXmlParsingIRService
	{
		protected readonly IPluginLogger _logger;
		public XmlParsingService(IPluginLogger logger)
		{
			this._logger = logger;
		}

		/// <inheritdoc />
		public SModel ParseXml(Stream streamContent)
		{
			using (XmlReader reader = XmlReader.Create(streamContent, GetReaderSettings()))
			{
				return ParseModel(reader);
			}
		}

		/// <inheritdoc />
		public SModel ParseXml(string xmlContent)
		{

			using (StringReader stringReader = new StringReader(xmlContent))
			{
				// Try skipping utf-16 BOM for backwards compatibility
				int ch = stringReader.Peek();
				if (ch == 0xfeff || ch == 0xffef)
				{
					stringReader.Read();
				}

				using (XmlReader reader = XmlReader.Create(stringReader, GetReaderSettings()))
				{
					return ParseModel(reader);
				}
			}
		}

		private SModel ParseModel(XmlReader reader)
		{
			var processItemStack = new Stack<SObject>();
			var model = new SModel();


			while (reader.MoveToNextAttribute() || reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						ProcessStartElement(reader, processItemStack, model);
						break;
					case XmlNodeType.Text:
						ProcessPrimitiveValue(reader, processItemStack);
						break;
					case XmlNodeType.Attribute:
						ProcessAttribute(reader, processItemStack, model);
						break;
					case XmlNodeType.EndElement:
						ProcessEndElement(processItemStack);
						break;
					case XmlNodeType.XmlDeclaration:
						ProcessDeclaration(reader, model);
						break;
				}
			}
			return model;
		}

		private static XmlReaderSettings GetReaderSettings()
		{
			return new XmlReaderSettings()
			{
				IgnoreWhitespace = true,
				CheckCharacters = false
			};
		}

		private void ProcessDeclaration(XmlReader reader, SModel model)
		{
			_logger.LogDebug("ProcessDeclaration TypeName {reader.Name}");
			var element = new SObject { TypeName = reader.Name };
			model.ModelDeclaration = element;

		}

		private void ProcessStartElement(XmlReader reader, Stack<SObject> processItemStack, SModel model)
		{
			_logger.LogDebug("ProcessStartElement TypeName {reader.Name}");
			var element = new SObject { TypeName = reader.Name };

			//if its not root element assign element to the parent
			if (processItemStack.Count > 0)
			{
				var parent = processItemStack.Peek();
				AddToParent(element, parent);
			}

			//if its not empty element
			if (!reader.IsEmptyElement)
			{
				_logger.LogDebug("ProcessStartElement add to fuhrer proses");
				//new parent
				processItemStack.Push(element);
			}

			//if its root item set as model root
			if (model.RootItem == null)
			{
				_logger.LogDebug("ProcessStartElement set as root item");
				model.RootItem = element;
			}
		}

		private void AddToParent(SObject element, SObject parent)
		{
			_logger.LogDebug($"AddToParent add element {element.GetElementName()} in to {parent.GetElementName()}");

			if (parent.Properties.ContainsKey(element.GetElementName()))
			{
				if (parent.Properties[element.GetElementName()] is SList sList)
				{
					sList.Add(element);
				}
			}
			else
			{
				// set reference as potential collection it will be fixed on the end of parent element
				parent.Properties[element.GetElementName()] = new SList(element);
			}
		}

		private void ProcessPrimitiveValue(XmlReader reader, Stack<SObject> processItemStack)
		{
			_logger.LogDebug($"ProcessPrimitiveValue value {reader.Value}");

			if (processItemStack.Count > 0)
			{
				var primitive = new SPrimitive { Value = reader.Value };
				processItemStack.Peek().Properties[primitive.GetElementName()] = primitive;
			}
			else
			{
				_logger.LogError($"ProcessPrimitiveValue parent not exits for primitive value {reader.Value}");
				throw new InvalidOperationException($"ProcessPrimitiveValue parent not exits for primitive value {reader.Value}");
			}
		}

		private void ProcessAttribute(XmlReader reader, Stack<SObject> processItemStack, SModel model)
		{
			_logger.LogDebug($"ProcessAttribute LocalName = {reader.LocalName}, Prefix = {reader.Prefix}, Value = {reader.Value}, NameSpace = {reader.NamespaceURI}");
			var attribute = new SAttribute { LocalName = reader.LocalName, Prefix = reader.Prefix, Value = reader.Value, NameSpace = reader.NamespaceURI };

			//test if its root namespace or standard element
			if (processItemStack.Count > 0)
			{
				processItemStack.Peek().Properties[reader.Name] = attribute;

				if (!model.RootNameSpaces.ContainsKey(reader.Prefix))
				{
					model.RootNameSpaces.Add(reader.LocalName, attribute);
				}
				else
				{
					_logger.LogDebug($"ProcessAttribute root attribute {reader.Name} exist");
				}
			}
			else if (model.ModelDeclaration is SObject declaration)
			{
				declaration.Properties[reader.Name] = attribute;
			}
		}

		private void ProcessEndElement(Stack<SObject> processItemStack)
		{
			_logger.LogDebug("ProcessEndElement");
			if (processItemStack.Count > 0)
			{
				var filled = processItemStack.Pop();
				_logger.LogDebug($"ProcessEndElement {filled.GetElementName()}");
				FixReferences(filled);
			}
		}

		/// <summary>
		/// After close element check if its real list or list need to be removed
		/// </summary>
		/// <param name="element"></param>
		private void FixReferences(SObject element)
		{
			_logger.LogDebug($"FixReferences {element.GetElementName()}");
			var properties = new Dictionary<string, ISIntermediate>();

			foreach (var item in element.Properties)
			{
				if (item.Value is SList sList && sList.Count == 1)
				{
					_logger.LogDebug($"FixReferences remove list reference");
					properties[item.Key] = sList.First();
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
