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
	}
}
