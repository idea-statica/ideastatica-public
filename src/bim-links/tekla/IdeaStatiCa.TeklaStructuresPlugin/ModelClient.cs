using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using Picker = Tekla.Structures.Model.UI.Picker;
using TS = Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin
{
	public class ModelClient : IModelClient
	{
		private readonly TS.Model teklaModel;
		private readonly IPluginLogger plugInLogger;

		public ModelClient(TS.Model teklaModel, IPluginLogger plugInLogger)
		{
			this.teklaModel = teklaModel;
			this.plugInLogger = plugInLogger;
		}

		private TS.Model GetTeklaModel()
		{
			if (!teklaModel.GetConnectionStatus())
			{
				plugInLogger.LogInformation("GetTeklaModel Tekla is not running.");
				throw new ArgumentException("Tekla is not running. Press Enter.");
			}

			return teklaModel;
		}

		/// <summary>
		/// Get all members from Advance steel project
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TS.ModelObject> GetAllMembers()
		{
			plugInLogger.LogInformation("GetAllMembers");
			var model = GetTeklaModel();
			var beamsEnumerator = model.GetModelObjectSelector().GetAllObjectsWithType(TS.ModelObject.ModelObjectEnum.BEAM);
			var polyBeamsEnumerator = model.GetModelObjectSelector().GetAllObjectsWithType(TS.ModelObject.ModelObjectEnum.POLYBEAM);

			List<TS.ModelObject> members = new List<TS.ModelObject>();
			while (beamsEnumerator.MoveNext())
			{
				members.Add(beamsEnumerator.Current);
			}

			while (polyBeamsEnumerator.MoveNext())
			{
				members.Add(polyBeamsEnumerator.Current);
			}
			return members;
		}

		/// <summary>
		/// Get Item By Handler
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		public TS.Object GetItemByHandler(string handle)
		{
			plugInLogger.LogInformation($"GetItemByHandler handle {handle}.");
			var model = GetTeklaModel();

			var handlers = handle.Split(';');
			//skip construction 
			var itemHandle = handlers[0];
			if (handlers.Length > 1)
			{
				itemHandle = handlers[1];
			}

			var identifier = new Tekla.Structures.Identifier(itemHandle);
			plugInLogger.LogDebug($"GetItemByHandler readItem by identifier {identifier.ToString()}.");

			return model.SelectModelObject(identifier);
		}

		/// <summary>
		/// Get Parent item by Handler
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		public TS.Object GetParentItemByHandler(string handle)
		{
			plugInLogger.LogInformation($"GetParentItemByHandler by handle {handle}.");
			var handlers = handle.Split(';');

			if (handlers.Length > 1)
			{
				return GetItemByHandler(handlers[0]);
			}
			else
			{
				plugInLogger.LogDebug($"GetParentItemByHandler unknown handle {handle}.");
				return null;
			}
		}

		/// <summary>
		/// Get Project Path
		/// </summary>
		/// <returns></returns>
		public string GetProjectPath()
		{
			plugInLogger.LogInformation($"GetProjectPath.");
			var model = GetTeklaModel();
			var modelInfo = model.GetInfo();
			var projectPath = Path.Combine(modelInfo.ModelPath, modelInfo.ModelName);
			plugInLogger.LogDebug($"GetProjectPath found path {projectPath}.");
			if (HasWritePermissionOnDir(projectPath))
			{
				return projectPath;
			}
			else
			{
				plugInLogger.LogDebug($"GetProjectPath found path {projectPath} has not write permission.");
				return GetBackUpProjectPath(projectPath);
			}
		}

		/// <summary>
		/// Get Connection Point by user action
		/// </summary>
		/// <returns></returns>
		public Point GetConnectionPoint()
		{
			plugInLogger.LogInformation($"GetConnectionPoint.");
			GetTeklaModel();

			var picker = new Picker();
			var connectionPoint = picker.PickPoint(TeklaStructuresResources.Properties.Resources.SelectCoonecntionPoint);
			if (connectionPoint == null)
			{
				plugInLogger.LogDebug($"GetConnectionPoint - not selected point");
				throw new InvalidOperationException("Invalid Point - not selected point");
			}
			plugInLogger.LogDebug($"GetConnectionPoint - selected point {connectionPoint.X} {connectionPoint.Y} {connectionPoint.Z}");
			return connectionPoint;
		}

		/// <summary>
		/// User select beams
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TS.ModelObject> GetSelectBeams()
		{
			plugInLogger.LogInformation($"GetSelectBeams.");
			GetTeklaModel();
			var picker = new Picker();

			var selectedItems = new List<TS.ModelObject>();

			TS.ModelObjectEnumerator partsEnumerator;
			try
			{
				partsEnumerator = picker.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, TeklaStructuresResources.Properties.Resources.SelectBeams);
			}
			catch (ApplicationException ex)
			{
				plugInLogger.LogInformation($"GetSelectBeams - selection failed", ex);
				return selectedItems;
			}


			while (partsEnumerator.MoveNext())
			{
				if (partsEnumerator.Current is TS.Beam beam)
				{
					plugInLogger.LogDebug($"GetSelectBeams. selected beam id {beam.Identifier} name {beam.Name}");
					selectedItems.Add(partsEnumerator.Current);
				}
			}

			return selectedItems;
		}

		/// <summary>
		/// User select connection objects
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TS.ModelObject> GetSelectObjects()
		{
			plugInLogger.LogInformation($"GetSelectObjects");
			GetTeklaModel();
			var picker = new Picker();

			var selectedItems = new List<TS.ModelObject>();
			TS.ModelObjectEnumerator partsEnumerator = null;
			try
			{
				partsEnumerator = picker.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, TeklaStructuresResources.Properties.Resources.SelectParts);
			}
			catch (ApplicationException)
			{
				return selectedItems;
			}


			while (partsEnumerator.MoveNext())
			{
				if (partsEnumerator.Current is TS.Part)
				{
					selectedItems.Add(partsEnumerator.Current);
				}
			}
			return selectedItems;
		}

		/// <summary>
		/// User select bulk selection
		/// </summary>
		/// <returns></returns>
		public List<(Point, List<TS.ModelObject>, List<TS.ModelObject>)> GetBulkSelection(bool selectWholeModel = false)
		{
			plugInLogger.LogInformation("GetBulkSelection");
			List<(Point, List<TS.ModelObject>, List<TS.ModelObject>)> selections = new List<(Point, List<TS.ModelObject>, List<TS.ModelObject>)>();
			{
				var myModel = GetTeklaModel();

				TS.ModelObjectEnumerator partsEnumerator = null;


				if (selectWholeModel)
				{
					partsEnumerator = myModel.GetModelObjectSelector().GetAllObjects();
				}
				else
				{
					var picker = new Picker();
					try
					{
						partsEnumerator = picker.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, TeklaStructuresResources.Properties.Resources.CreateBulkSelection);
					}
					catch (ApplicationException ex)
					{
						plugInLogger.LogInformation("GetBulkSelection selection failed", ex);
					}
				}

				BIM.Common.SorterResult sortedJoints = BulkSelectionHelper.FindJoints(myModel, partsEnumerator);


				plugInLogger.LogInformation($"GetBulkSelection found joints {sortedJoints.Joints.Count}");
				foreach (var joint in sortedJoints.Joints)
				{
					plugInLogger.LogInformation($"GetBulkSelection joint {joint.Location.X} {joint.Location.Y} {joint.Location.Z}");
					List<TS.ModelObject> beams = new List<TS.ModelObject>();
					List<TS.ModelObject> parts = new List<TS.ModelObject>();


					var structuralMembers = joint.Members
					.Where(m => (m.Parent as TS.Part)?.Name != BulkSelectionHelper.HaunchMemberName)
					.Where(m => (m.Parent as TS.Part)?.Name != BulkSelectionHelper.TeklaAnchorRodName)
					.Where(m => (m.Parent as TS.Part)?.Name != BulkSelectionHelper.TeklaAnchorWasherName)
					.Where(m => (m.Parent as TS.Part)?.Name != BulkSelectionHelper.TeklaAnchorNutName);

					structuralMembers.ToList().ForEach(sm => beams.Add(sm.Parent as TS.ModelObject));

					plugInLogger.LogInformation($"GetBulkSelection joint number of members {beams.Count}");

					plugInLogger.LogInformation($"GetBulkSelection joint number of plates {joint.Plates.Count}");
					foreach (var plate in joint.Plates)
					{
						if (plate.Parent is TS.ModelObject tsObject)
						{
							parts.Add(tsObject);
						}
					}

					plugInLogger.LogInformation($"GetBulkSelection joint number of stiffening members {joint.StiffeningMembers.Count}");
					foreach (var stiffeningmember in joint.StiffeningMembers)
					{
						if (stiffeningmember.Parent is TS.ModelObject tsObject)
						{
							parts.Add(tsObject);
						}
					}

					plugInLogger.LogInformation($"GetBulkSelection joint number of fasteners {joint.Fasteners.Count}");
					foreach (var jointFastener in joint.Fasteners)
					{
						if (jointFastener.Parent is TS.ModelObject tsObject)
						{
							parts.Add(tsObject);
						}
					}

					plugInLogger.LogInformation($"GetBulkSelection joint number of welds {joint.Welds.Count}");
					foreach (var jointWeld in joint.Welds)
					{
						if (jointWeld.Parent is TS.ModelObject tsObject)
						{
							parts.Add(tsObject);
						}
					}

					selections?.Add(
						(
							new Point(joint.Location.X, joint.Location.Y, joint.Location.Z),
							beams,
							parts
						)
					);
				}
			}
			return selections;
		}

		/// <summary>
		/// Get point id
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public string GetPointId(Point point)
		{
			plugInLogger.LogInformation($"GetPointId {point.X.ToString("G", CultureInfo.InvariantCulture)}; {point.Y.ToString("G", CultureInfo.InvariantCulture)}; {point.Z.ToString("G", CultureInfo.InvariantCulture)}");

			return $"{point.X.ToString("G", CultureInfo.InvariantCulture)};{point.Y.ToString("G", CultureInfo.InvariantCulture)};{point.Z.ToString("G", CultureInfo.InvariantCulture)}";
		}

		/// <summary>
		/// get point from id
		/// </summary>
		/// <param name="nodeNo"></param>
		/// <returns></returns>
		public Point GetPoint3D(string nodeNo)
		{
			plugInLogger.LogDebug($"GetPoint3D for id {nodeNo}");

			var coords = nodeNo.Split(';');
			if (coords.Length == 3)
			{
				if (!double.TryParse(coords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
				{
					plugInLogger.LogInformation($"Not unknown coord X {coords[0]}");
					return null;
				}

				if (!double.TryParse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
				{
					plugInLogger.LogInformation($"Not unknown coord Y {coords[1]}");
					return null;
				}

				if (!double.TryParse(coords[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double z))
				{
					plugInLogger.LogInformation($"Not unknown coord Z {coords[2]}");
					return null;
				}

				return new Point(x, y, z);
			}
			else
			{
				plugInLogger.LogInformation($"Not unknown node X {nodeNo}");
				return null;
			}
		}

		/// <summary>
		/// Get Material from database
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Tekla.Structures.Catalogs.MaterialItem GetMaterial(string name)
		{
			plugInLogger.LogInformation($"GetMaterial {name}");
			var mat = new Tekla.Structures.Catalogs.MaterialItem();
			if (mat.Select(name))
			{

				return mat;
			}

			plugInLogger.LogInformation($"GetMaterial not found material {name}");
			return null;
		}

		/// <summary>
		/// Get CrossSection from database
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ProfileItem GetCrossSection(string name)
		{
			plugInLogger.LogInformation($"GetCrossSection {name}");
			var libItem = new LibraryProfileItem();
			if (!libItem.Select(name) || libItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
			{
				ParametricProfileItem paramProfileItem = new ParametricProfileItem();
				if (paramProfileItem.Select(name))
				{
					return paramProfileItem;
				}
				else
				{
					plugInLogger.LogInformation($"GetCrossSection not found {name}");
					return null;
				}
			}
			else
			{
				return libItem;
			}
		}

		/// <summary>
		/// Get Project Name
		/// </summary>
		/// <returns></returns>
		public string GetProjectName()
		{
			plugInLogger.LogDebug($"GetProjectName");
			return Path.GetFileName(GetProjectPath());
		}

		/// <summary>
		/// Check if has write permission
		/// </summary>
		/// <param name="FilePath"></param>
		/// <returns></returns>
		public static bool HasWritePermissionOnDir(string FilePath)
		{
			try
			{
				FileSystemSecurity security;
				if (File.Exists(FilePath))
				{
					security = File.GetAccessControl(FilePath);
				}
				else
				{
					security = Directory.GetAccessControl(Path.GetDirectoryName(FilePath));
				}
				var rules = security.GetAccessRules(true, true, typeof(NTAccount));

				var currentuser = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				bool result = false;
				foreach (FileSystemAccessRule rule in rules)
				{
					if (0 == (rule.FileSystemRights &
						(FileSystemRights.WriteData | FileSystemRights.Write)))
					{
						continue;
					}

					if (rule.IdentityReference.Value.StartsWith("S-1-"))
					{
						var sid = new SecurityIdentifier(rule.IdentityReference.Value);
						if (!currentuser.IsInRole(sid))
						{
							continue;
						}
					}
					else
					{
						if (!currentuser.IsInRole(rule.IdentityReference.Value))
						{
							continue;
						}
					}

					if (rule.AccessControlType == AccessControlType.Deny)
						return false;
					if (rule.AccessControlType == AccessControlType.Allow)
						result = true;
				}
				return result;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Get path to the user document folder for hashed project
		/// </summary>
		/// <param name="originalPath"></param>
		public string GetBackUpProjectPath(string originalPath)
		{
			plugInLogger.LogDebug("GetBackUpProjectPath");
			var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var backupPath = Path.Combine(docPath, "IdeaStatiCa Projects", GetHashString(originalPath), Path.GetFileName(originalPath));
			plugInLogger.LogInformation($"GetBackUpProjectPath path {backupPath}");
			return backupPath;
		}

		/// <summary>
		/// Calculate hash from string
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		protected static byte[] GetHash(string inputString)
		{
			using (HashAlgorithm algorithm = SHA256.Create())
				return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}

		/// <summary>
		/// Calculate hash from string
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		protected static string GetHashString(string inputString)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in GetHash(inputString))
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}
	}
}
