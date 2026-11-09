using CadExampleApi;
using System;
using System.Globalization;

namespace BimApiLinkCadExample.Conversion
{
	/// <summary>
	/// A node has no id of its own in the CAD model, so it is identified by its coordinates: "x;y;z".
	/// </summary>
	/// <remarks>
	/// The format must stay the same as the one <c>ConnectionIdentifier</c> uses for the node of a connection.
	/// Otherwise the connection node and the member end at the same point become two different nodes.
	/// </remarks>
	internal static class NodeId
	{
		/// <summary>
		/// The id of the node at the point.
		/// </summary>
		public static string Of(CadPoint3D point)
			=> string.Join(";", Format(point.X), Format(point.Y), Format(point.Z));

		/// <summary>
		/// The point a node id stands for.
		/// </summary>
		public static CadPoint3D ToPoint(string nodeId)
		{
			string[] coordinates = nodeId.Split(';');
			if (coordinates.Length != 3)
			{
				throw new FormatException($"'{nodeId}' is not a node id.");
			}

			return new CadPoint3D(Parse(coordinates[0]), Parse(coordinates[1]), Parse(coordinates[2]));
		}

		/// <summary>
		/// Writes one coordinate the same way <c>ConnectionIdentifier</c> does.
		/// </summary>
		private static string Format(double coordinate) => coordinate.ToString("G", CultureInfo.InvariantCulture);

		/// <summary>
		/// Reads one coordinate written by <see cref="Format"/>.
		/// </summary>
		private static double Parse(string coordinate) => double.Parse(coordinate, NumberStyles.Float, CultureInfo.InvariantCulture);
	}
}
