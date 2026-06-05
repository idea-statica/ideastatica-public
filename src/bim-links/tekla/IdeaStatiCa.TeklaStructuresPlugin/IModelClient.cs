using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin
{
	/// <summary>
	/// Interface responsible for reading data from tekla model
	/// </summary>
	public interface IModelClient
	{
		/// <summary>
		/// Read object by string id
		/// </summary>
		/// <param name="handle">string representing Identifier GUID</param>
		/// <returns></returns>
		Object GetItemByHandler(string handle);

		/// <summary>
		/// Create string representation of point
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		string GetPointId(Point point);

		/// <summary>
		/// From string representation of point restore instance of point
		/// </summary>
		/// <param name="nodeNo"></param>
		/// <returns></returns>
		Point GetPoint3D(string nodeNo);

		/// <summary>
		/// Read crosssection by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		ProfileItem GetCrossSection(string name);

		/// <summary>
		/// Read material by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		MaterialItem GetMaterial(string name);

		/// <summary>
		/// Get Tekla project path
		/// </summary>
		/// <returns></returns>
		string GetProjectPath();

		/// <summary>
		/// Get User selection by bulk selection
		/// </summary>
		/// <returns></returns>
		List<(Point, List<ModelObject>, List<ModelObject>)> GetBulkSelection(bool selectWholeModel = false, IProgressMessaging progressMessaging = null, IdeaStatiCa.BIM.Common.SorterSettings sorterSettings = null);

		/// <summary>
		/// Get user selection of connection point
		/// </summary>
		/// <returns></returns>
		Point GetConnectionPoint();

		/// <summary>
		/// Get Project Name
		/// </summary>
		/// <returns></returns>
		string GetProjectName();

		/// <summary>
		/// Get all members
		/// </summary>
		/// <returns></returns>
		IEnumerable<ModelObject> GetAllMembers();

		/// <summary>
		/// Get user selection of beams
		/// </summary>
		/// <returns></returns>
		IEnumerable<ModelObject> GetSelectBeams();

		/// <summary>
		/// Get user selection of connection objects
		/// </summary>
		/// <returns></returns>
		IEnumerable<ModelObject> GetSelectObjects();

		/// <summary>
		/// Cache Created Object
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="createdObject"></param>
		void CacheCreatedObject(IIdentifier identifier, IIdeaObject createdObject);

		/// <summary>
		/// Get Cached Object
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		IIdeaObject GetCachedObject(IIdentifier identifier);

		/// <summary>
		/// Clear cached items
		/// </summary>
		void ClearCache();

		/// <summary>
		/// Register a GUID set for a connection point identified by its node key (X;Y;Z).
		/// Called from GetCadUserSelection before import starts.
		/// </summary>
		void RegisterConnectionGuids(string nodeKey, HashSet<string> guids);

		/// <summary>
		/// Set the current connection GUID set by looking up the node key registered earlier.
		/// Called from ConnectionImporter.Create at the start of each CP import.
		/// </summary>
		void SetCurrentConnectionGuidsByKey(string nodeKey);

		/// <summary>
		/// Set GUIDs of all objects (beams, plates, fasteners) that belong to the currently processed connection point.
		/// BoltGridImporter uses this to skip ConnectedParts that were cached for a different connection.
		/// </summary>
		void SetCurrentConnectionGuids(HashSet<string> guids);

		/// <summary>
		/// Returns true if the given GUID belongs to the currently processed connection point.
		/// Returns true when no connection context has been set (backwards-compatible default).
		/// </summary>
		bool IsInCurrentConnection(string guid);

		/// <summary>
		/// Returns true if guid appears in ANY registered connection GUID set.
		/// Parts that return false were not picked up by the sorter and can be adopted
		/// by the current connection if a bolt there references them.
		/// </summary>
		bool IsRegisteredInAnyConnection(string guid);

		/// <summary>
		/// Returns true if partGuid belongs to the same connection as boltGridGuid.
		/// Prevents ConnectedParts from leaking across connections when importers run in parallel.
		/// Returns true when no mapping exists (backwards-compatible default).
		/// </summary>
		bool IsInSameConnectionAs(string boltGridGuid, string partGuid);
	}

}