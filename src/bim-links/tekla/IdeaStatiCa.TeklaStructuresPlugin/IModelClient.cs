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
		List<(Point, List<ModelObject>, List<ModelObject>)> GetBulkSelection(bool selectWholeModel = false);

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
	}

}