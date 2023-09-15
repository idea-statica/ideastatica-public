using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BimApiLinkCadExample.CadExampleApi;

namespace BimApiLinkCadExample.Utils
{
	internal static class PointTranslator
	{
		/// <summary>
		/// Get point id. This allows us to get a string identifier for the point object based on its location.
		/// </summary>
		/// <param name="point"></param>
		public static string GetPointId(CadPoint3D point)
		{
			return $"{point.X.ToString("G", CultureInfo.InvariantCulture)};{point.Y.ToString("G", CultureInfo.InvariantCulture)};{point.Z.ToString("G", CultureInfo.InvariantCulture)}";
		}


		/// <summary>
		/// get point from id
		/// </summary>
		/// <param name="nodeNo"></param>
		/// <returns></returns>
		public static CadPoint3D GetPoint3D(string nodeNo)
		{
			var coords = nodeNo.Split(';');
			if (coords.Length == 3)
			{
				if (!double.TryParse(coords[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x))
				{
					//ideaLogger.LogInformation($"Not unknown coord X {coords[0]}");
					return null;
				}

				if (!double.TryParse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
				{
					//ideaLogger.LogInformation($"Not unknown coord Y {coords[1]}");
					return null;
				}

				if (!double.TryParse(coords[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double z))
				{
					//ideaLogger.LogInformation($"Not unknown coord Z {coords[2]}");
					return null;
				}

				return new CadPoint3D((double)x, (double)y, (double)z);
			}
			else
			{
				//ideaLogger.LogInformation($"Not unknown node X {nodeNo}");
				return null;
			}
		}

	}
}
