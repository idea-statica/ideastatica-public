using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.Tools
{
	/// <summary>
	/// Helpers shared by the 3.3.x connection-entity steps: recursive lookup of intermediate
	/// objects and conversion of a material property between a name string and a ReferenceElement.
	/// </summary>
	internal static class ConnectionRefTool
	{
		/// <summary>
		/// Recursively collect every <see cref="SObject"/> with the given element/type name
		/// anywhere below <paramref name="node"/> (PlateData/WeldData appear under several parents).
		/// </summary>
		public static IEnumerable<SObject> FindAll(ISIntermediate node, string typeName)
		{
			switch (node)
			{
				case SObject o:
					if (o.TypeName == typeName)
					{
						yield return o;
					}
					foreach (var child in o.Properties.Values)
					{
						foreach (var r in FindAll(child, typeName))
						{
							yield return r;
						}
					}
					break;
				case SList l:
					foreach (var item in l.Items)
					{
						foreach (var r in FindAll(item, typeName))
						{
							yield return r;
						}
					}
					break;
			}
		}

		/// <summary>
		/// Map material Name -> Id for a top-level material list (e.g. "MatSteel", "MatConcrete", "MatWelding").
		/// </summary>
		public static Dictionary<string, string> BuildNameToId(ISIntermediate openModel, string matListName)
		{
			var map = new Dictionary<string, string>();
			foreach (var mat in openModel.GetElements($"{matListName};{matListName}").OfType<SObject>())
			{
				var name = mat.TryGetElementValue("Name");
				var id = mat.TryGetElementValue("Id");
				if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(id))
				{
					map[name] = id;
				}
			}
			return map;
		}

		/// <summary>
		/// Map material Id -> Name for a top-level material list.
		/// </summary>
		public static Dictionary<string, string> BuildIdToName(ISIntermediate openModel, string matListName)
		{
			var map = new Dictionary<string, string>();
			foreach (var mat in openModel.GetElements($"{matListName};{matListName}").OfType<SObject>())
			{
				var name = mat.TryGetElementValue("Name");
				var id = mat.TryGetElementValue("Id");
				if (!string.IsNullOrEmpty(id))
				{
					map[id] = name ?? string.Empty;
				}
			}
			return map;
		}

		/// <summary>
		/// Convert a name-string material property (e.g. &lt;Material&gt;S355&lt;/Material&gt;) into a
		/// ReferenceElement (TypeName + Id). A missing/unresolved material leaves the property absent.
		/// </summary>
		public static void StringToReference(SObject owner, string property, string refType, Dictionary<string, string> nameToId, IPluginLogger logger)
		{
			var name = owner.TryGetElementValue(property);
			owner.RemoveElementProperty(property);
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			if (!nameToId.TryGetValue(name, out var id))
			{
				logger.LogTrace($"Material '{name}' not found in {refType}; reference left empty");
				return;
			}
			var refObj = owner.CreateElementProperty(property);
			IRIOMTool.CreateIOMReferenceElement(refObj, refType, id);
		}

		/// <summary>
		/// Convert a ReferenceElement material property back into a name string (downgrade).
		/// </summary>
		public static void ReferenceToString(SObject owner, string property, Dictionary<string, string> idToName, IPluginLogger logger)
		{
			var id = GetReferenceId(owner, property);
			owner.RemoveElementProperty(property);
			var name = string.Empty;
			if (!string.IsNullOrEmpty(id) && idToName.TryGetValue(id, out var resolved))
			{
				name = resolved;
			}
			owner.CreateElementProperty(property).ChangeElementValue(name);
		}

		/// <summary>
		/// Read the Id of a ReferenceElement stored in <paramref name="property"/> of <paramref name="owner"/>.
		/// </summary>
		public static string GetReferenceId(SObject owner, string property)
		{
			if (owner.Properties.TryGetValue(property, out var refElem) && refElem is SObject refObj)
			{
				return refObj.TryGetElementValue("Id");
			}
			return null;
		}

		/// <summary>
		/// Read the TypeName of a ReferenceElement stored in <paramref name="property"/> of <paramref name="owner"/>.
		/// </summary>
		public static string GetReferenceTypeName(SObject owner, string property)
		{
			if (owner.Properties.TryGetValue(property, out var refElem) && refElem is SObject refObj)
			{
				return refObj.TryGetElementValue("TypeName");
			}
			return null;
		}

		private const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

		/// <summary>
		/// Add a library-loaded material element (e.g. MatConcrete) into a top-level material list,
		/// so a name-only material referenced by a connection entity resolves to a real material
		/// instead of being dropped. The caller supplies a list-unique <paramref name="id"/> and the
		/// concrete .NET type via <paramref name="xsiType"/> (e.g. "MatConcreteEc2").
		/// </summary>
		public static void MaterializeMaterial(ISIntermediate openModel, string matListName, string xsiType, string name, string id)
		{
			var mat = new SObject() { TypeName = matListName };
			mat.Properties["xsi:type"] = new SAttribute
			{
				Prefix = "xsi",
				LocalName = "type",
				Value = xsiType,
				NameSpace = XsiNamespace,
			};
			mat.CreateElementProperty("Id").ChangeElementValue(id);
			mat.CreateElementProperty("Name").ChangeElementValue(name);
			// Resolved from the material library by Name at consume time (as exporters emit materials).
			mat.CreateElementProperty("LoadFromLibrary").ChangeElementValue("true");
			AddToList(openModel, matListName, matListName, mat);
		}

		/// <summary>
		/// Add <paramref name="item"/> into a parser-shaped list: a wrapper SObject named
		/// <paramref name="listName"/> whose items are keyed by <paramref name="itemType"/>
		/// (an SList for many, a single SObject for one, absent when empty).
		/// </summary>
		public static void AddToList(ISIntermediate parent, string listName, string itemType, SObject item)
		{
			if (!(parent is SObject owner))
			{
				return;
			}

			SObject wrapper;
			if (owner.Properties.TryGetValue(listName, out var value))
			{
				wrapper = value as SObject ?? ((value as SList)?.First() as SObject);
			}
			else
			{
				wrapper = owner.CreateElementProperty(listName);
			}

			if (wrapper == null)
			{
				return;
			}

			if (wrapper.Properties.TryGetValue(itemType, out var inner))
			{
				if (inner is SList list)
				{
					list.Add(item);
				}
				else
				{
					wrapper.Properties[itemType] = new SList(inner, item);
				}
			}
			else
			{
				wrapper.Properties[itemType] = new SList(item);
			}
		}
	}
}
