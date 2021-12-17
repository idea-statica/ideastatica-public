using IdeaStatiCa.RamToIdea.Factories;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMemberColumn : AbstractRamMember
	{
		public override int UID => _column.lUID;

		public override MemberType MemberType => MemberType.Column;

		protected override int Label => _column.lLabel;

		protected override double Rotation => _column.dOrientation;

		protected override EMATERIALTYPES MaterialType => _column.eMaterial;

		protected override int MaterialUID => _column.lMaterialID;

		protected override string CrossSectionName => _column.strSectionLabel;

		protected override int CrossSectionUID => _column.lSectionID;

		private readonly IColumn _column;

		public RamMemberColumn(IObjectFactory objectFactory, INodes nodes, IColumn column)
			: base(objectFactory, nodes)
		{
			_column = column;
		}

		protected override (SCoordinate, SCoordinate) GetStartEndCoordinates()
		{
			SCoordinate start = new SCoordinate();
			SCoordinate end = new SCoordinate();

			_column.GetEndCoordinates(ref start, ref end);

			return (start, end);
		}
	}
}