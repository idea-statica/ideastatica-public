using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IdeaRS.OpenModel.CrossSection
{
	[OpenModelClass("IdeaRS.WsLibCssService.Gcss,CI.CrossSection")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class GcssComponent : OpenObject
	{
		public string Name { get; set; }
		public CrossSectionParameter CrossSection { get; set; }
		public int Id { get; set; }
		public int InsertPointID { get; set; }
		public double OffsetY { get; set; }
		public double OffsetZ { get; set; }
		public double Rotation { get; set; }
		public double InsertionPointRotation { get; set; }
		public bool MirrorY { get; set; }
		public bool MirrorZ { get; set; }
		public double MeshSize { get; set; }
		public int ZeroPointID { get; set; }
		public int PrevComponentID { get; set; }
		public int NextComponentID { get; set; }
		public int RelatedComponentID { get; set; }
		public bool CustomMeshSize { get; set; }
	}
}
