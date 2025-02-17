﻿using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utils
{
	internal static class MemberHelper
	{

		/// <summary>
		/// For L shape members adjust rotation [diferent convention for Tekla and Idea]
		/// </summary>
		/// <param name="beam"></param>
		/// <param name="member"> </param>
		public static void AdjustAngleMember(Part beam, Member1D member)
		{
			if (IsAngleMember(beam))
			{
				member.MirrorY = true;
			}
		}

		/// <summary>
		/// Is member L shape
		/// </summary>
		/// <param name="beam"></param>
		public static bool IsAngleMember(Part beam)
		{
			string strProfName = beam.Profile.ProfileString;

			LibraryProfileItem profileItem = new LibraryProfileItem();
			profileItem.Select(strProfName);

			if (profileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_L)
			{
				return true;
			}
			else if (profileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
			{
				ParametricProfileItem paramProfileItem = new ParametricProfileItem();
				if (paramProfileItem.Select(strProfName) && paramProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_L)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Get part length by begin and end of centerline
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public static double GetPartLength(Part part)
		{
			var centLine = part.GetCenterLine(false);

			Point begNode = centLine[0] as Point;
			Point endNode = centLine[centLine.Count - 1] as Point;
			var vector = new Vector(endNode - begNode);

			var memberLen = vector.GetLength();
			return memberLen;
		}
	}
}
