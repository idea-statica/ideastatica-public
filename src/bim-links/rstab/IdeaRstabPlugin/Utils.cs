using Dlubal.RSTAB8;
using IdeaRstabPlugin.Factories;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;

namespace IdeaRstabPlugin
{
	internal static class Utils
	{
		private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab");

		/// <summary>
		/// Parses RSTAB object list. It is a comma separated list of number (e.g. 1,5,3,4), list of ranges (e.g. 1-3,7-10),
		/// or combination of both (e.g. 1,3,5-10,15).
		/// </summary>
		/// <param name="objects">Comma separated list</param>
		/// <returns>Numbers parsed from list with ranges expanded.</returns>
		public static IEnumerable<int> ParseObjectList(string objects)
		{
			if (string.IsNullOrEmpty(objects))
			{
				yield break;
			}

			foreach (string interval in objects.Split(','))
			{
				string[] parts = interval.Trim().Split('-');
				if (parts.Length == 1)
				{
					if (int.TryParse(parts[0], out int number))
					{
						yield return number;
					}
				}
				else if (parts.Length == 2)
				{
					if (int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
					{
						for (int i = start; i <= end; i++)
						{
							yield return i;
						}
					}
				}
			}
		}

		/// <summary>
		/// Parses RSTAB material TextID, e.g. NameID|S 235@TypeID|STEEL@NormID|DIN EN 1993-1-1-10.
		/// </summary>
		/// <param name="textId">Text ID string</param>
		/// <param name="name">Name of material (TypeID field)</param>
		/// <param name="type">Type of material (NormID field)</param>
		/// <param name="norm">Norm (last field in TextID)</param>
		/// <exception cref="ArgumentException">Throws if <paramref name="textId"/> is not in correct format.</exception>
		public static void ParseMaterialTextID(string textId, out string name, out string type, out string norm)
		{
			string[] parts = textId.Split('|');
			if (parts.Length != 4)
			{
				throw new ArgumentException("Unknown TextID format.", nameof(textId));
			}

			(string typeIdValue, string typeIdKey) = ParseMaterialTextIDInner(parts[1]);
			if (typeIdKey != "TypeID")
			{
				throw new ArgumentException("Unknown TextID format. Did not find 'TypeID'.", nameof(textId));
			}

			(string normIdValue, string normIdKey) = ParseMaterialTextIDInner(parts[2]);
			if (normIdKey != "NormID")
			{
				throw new ArgumentException("Unknown TextID format. Did not find 'NormID'.", nameof(textId));
			}

			name = typeIdValue;
			type = normIdValue;
			norm = parts[3];
		}

		private static (string, string) ParseMaterialTextIDInner(string input)
		{
			int pos = input.IndexOf('@');
			if (pos == -1)
			{
				throw new ArgumentException("Unknown TextID format. Expected character '@' not found.", nameof(input));
			}

			return (input.Substring(0, pos), input.Substring(pos + 1));
		}

		public static double GetEulerXAxisAngle(PlaneType planeType, IdeaVector3D a, IdeaVector3D b, IdeaVector3D c)
		{
			if (planeType != PlaneType.PlaneXY && planeType != PlaneType.PlaneXZ)
			{
				throw new ArgumentException("Only PlaneXY and PlaneXZ are supported.", nameof(planeType));
			}

			UnitVector3D originVector;
			if (planeType == PlaneType.PlaneXY)
			{
				originVector = UnitVector3D.Create(0, 0, 1);
			}
			else
			{
				originVector = UnitVector3D.Create(0, 1, 0);
			}

			Plane plane = Plane.FromPoints(Vector2Point(a), Vector2Point(b), Vector2Point(c));
			return originVector.SignedAngleTo(plane.Normal, UnitVector3D.Create(1, 0, 0)).Radians;
		}

		private static MathNet.Spatial.Euclidean.Point3D Vector2Point(IdeaVector3D vec)
		{
			return new MathNet.Spatial.Euclidean.Point3D(vec.X, vec.Y, vec.Z);
		}

		/// <summary>
		/// Converts RSTAB <see cref="Rotation"/> object into a angle in radians.  
		/// </summary>
		/// <param name="rotation">Rotation</param>
		/// <param name="objectFactory">IObjectFactory instance, 
		/// used to obtain <see cref="IIdeaNode"/> instance from <see cref="Rotation.HelpNodeNo"/></param>
		/// <param name="node1">An another node for rotation by a plane</param>
		/// <param name="node2">An another node for rotation by a plane</param>
		/// <returns>Angle in radians</returns>
		public static double ConvertRotation(Rotation rotation, IObjectFactory objectFactory, IIdeaNode node1, IIdeaNode node2)
		{
			switch (rotation.Type)
			{
				case RotationType.Angle:
					return rotation.Angle;

				case RotationType.NoneRotation:
					return 0.0;

				case RotationType.HelpNode:
					return Utils.GetEulerXAxisAngle(
						rotation.Plane,
						node1.Vector,
						node2.Vector,
						objectFactory.GetNode(rotation.HelpNodeNo).Vector
					);

				default:
					_logger.LogWarning($"Unsupported rotation type {rotation.Type}, using no rotation");
					return 0.0;
			}
		}
	}
}