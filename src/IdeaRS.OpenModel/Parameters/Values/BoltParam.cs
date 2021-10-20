using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using System.Diagnostics;

namespace IdeaRS.OpenModel.Parameters
{
	/// <summary>
	/// Position, to which is input related to.
	/// </summary>
	public enum PositionRelatedTo
	{
		/// <summary>
		/// Position is related to any axis.
		/// </summary>
		Axis,

		/// <summary>
		/// Position is related to top of steel.
		/// </summary>
		TopOfSteel,

		/// <summary>
		/// Position is related to the related profile (cross-section) bounding box.
		/// In this case is Rows and Cols position are on the positive axes (top, right),
		/// RowsNegative and ColsNegative are on the negative axes.
		/// </summary>
		Profile,

		/// <summary>
		/// Position is related to the left side of main plate.
		/// </summary>
		LeftSide,

		/// <summary>
		/// Position is related to the right side of main plate.
		/// </summary>
		RightSide,
	}

	/// <summary>
	/// Type of polar input
	/// </summary>
	public enum PolarInputType
	{
		/// <summary>
		/// Polar input defined by count in circle.
		/// </summary>
		ByCount,

		/// <summary>
		/// Polar input defined by angles.
		/// </summary>
		ByAngle,
	}

	/// <summary>
	/// Defines a type of fasteners grid.
	/// </summary>
	public enum ConnectorGridType
	{
		/// <summary>
		/// Regular type of grid.
		/// </summary>
		Regular = 0,

		/// <summary>
		/// First row is shifted.
		/// </summary>
		FirstShiftedFull = 1 << 0,

		/// <summary>
		/// Second row is shifted
		/// </summary>
		SecondShiftedFull = 1 << 1,

		/// <summary>
		/// First row is shifted, second row is short.
		/// </summary>
		FirstShiftedShort = 1 << 2,

		/// <summary>
		/// Second row is shifted, first is short.
		/// </summary>
		SecondShiftedShort = 1 << 3,

		/// <summary>
		/// First shifted.
		/// </summary>
		FirstShifted = FirstShiftedFull | FirstShiftedShort,

		/// <summary>
		/// Second shifted.
		/// </summary>
		SecondShifted = SecondShiftedFull | SecondShiftedShort,

		/// <summary>
		/// Rows are full.
		/// </summary>
		Full = FirstShiftedFull | SecondShiftedFull,

		/// <summary>
		/// Rows are short.
		/// </summary>
		Short = FirstShiftedShort | SecondShiftedShort,
	}

	/// <summary>
	/// Defines a transfer of shear force in bolts.
	/// </summary>
	public enum BoltShearType : int
	{
		/// <summary>
		/// TODO DRA 1
		/// </summary>
		Bearing,

		/// <summary>
		/// TODO DRA 2
		/// </summary>
		Interaction,

		/// <summary>
		/// TODO DRA 3
		/// </summary>
		Friction
	}

	/// <summary>
	/// Specifies pair of value and their count.
	/// </summary>
	[DebuggerDisplay("{Value} * {Count}")]
	public sealed class ValueCount
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public double Value { get; set; }

		/// <summary>
		/// Gets or sets the repeating of value.
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Creates a new instance with specified value.
		/// </summary>
		/// <param name="value">The value to set.</param>
		/// <returns>A new instance of ValueCount.</returns>
		public static ValueCount Create(double value)
		{
			return new ValueCount { Value = value, Count = 1 };
		}

		/// <summary>
		/// Creates a new instance with value and their repeating.
		/// </summary>
		/// <param name="value">The value to set.</param>
		/// <param name="count">The number of repeating.</param>
		/// <returns>A new instance of ValueCount.</returns>
		public static ValueCount Create(double value, int count)
		{
			return new ValueCount { Value = value, Count = count };
		}
	}

	/// <summary>
	/// Defines used input of groups of numbers.
	/// </summary>
	public sealed class NumberGroups : List<List<ValueCount>>
	{
		/// <summary>
		/// Creates new instance with list of ValueCount .
		/// </summary>
		/// <param name="list">The list of ValueCount objects.</param>
		/// <returns>A new instance of group.</returns>
		public static NumberGroups Create(List<ValueCount> list)
		{
			return new NumberGroups { list };
		}

		/// <summary>
		/// Creates new instance with varibale count of bolts based on * operator
		/// </summary>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static NumberGroups Create(params string[] positions)
		{
			int countOfParams = positions.Length;
			var numberGroups = new NumberGroups { new List<ValueCount>()};

			for (int i=0; i<countOfParams; i++)
			{
				var position = positions[i];
				if (position.Contains("*"))
				{
					string[] result = position.Split('*');
					if (result.Length == 2)
					{
						bool valA = double.TryParse(result[0], out double a);
						bool valB = int.TryParse(result[1], out int b);
						if (valA && valB)
						{
							var valueCount = ValueCount.Create(a, b);
							numberGroups[0].Add(valueCount);
						}
					}
				}
				else
				{
					bool valA = double.TryParse(position, out double a);
					if (valA)
					{
						var valueCount = ValueCount.Create(a);
						numberGroups[0].Add(valueCount);
					}
				}

			}

			return numberGroups;

		}

		/// <summary>
		/// Creates new instance with one group with one value.
		/// </summary>
		/// <param name="value">The value to set to group.</param>
		/// <returns>A new instance of group.</returns>
		public static NumberGroups Create(double value)
		{
			return new NumberGroups { new List<ValueCount> { ValueCount.Create(value) } };
		}

		/// <summary>
		/// Creates a new instance of one group with several values (defined by count).
		/// </summary>
		/// <param name="value">The input value.</param>
		/// <param name="count">The count, defined value repeating.</param>
		/// <returns>A new instance of group.</returns>
		public static NumberGroups Create(double value, int count)
		{
			return new NumberGroups { new List<ValueCount> { ValueCount.Create(value, count) } };
		}

		/// <summary>
		/// Creates new instance with one group with specified sequence of numbers.
		/// </summary>
		/// <param name="positions">The sequence of numbers to add to group.</param>
		/// <returns>A new instance of group.</returns>
		public static NumberGroups Create(IEnumerable<double> positions)
		{
			return new NumberGroups { positions.Select(v => ValueCount.Create(v)).ToList() };
		}

		/// <summary>
		/// Creates new instance with one group with specified sequence of numbers.
		/// </summary>
		/// <param name="positions">The sequence of numbers to add to group.</param>
		/// <returns>A new instance of group.</returns>
		public static NumberGroups Create(params double[] positions)
		{
			return Create((IEnumerable<double>)positions);
		}

		/// <summary>
		/// Creates new instance with groups with with one value in group.
		/// </summary>
		/// <param name="positions">The sequence of numbers to create groups.</param>
		/// <returns>A new instance of groups.</returns>
		public static NumberGroups CreateGroups(IEnumerable<double> positions)
		{
			var ngs = new NumberGroups();
			ngs.AddRange(positions.Select(p => new List<ValueCount> { ValueCount.Create(p) } ));
			return ngs;
		}

		/// <summary>
		/// Creates new instance with groups with with one value in group.
		/// </summary>
		/// <param name="positions">The sequence of numbers to create groups.</param>
		/// <returns>A new instance of groups.</returns>
		public static NumberGroups CreateGroups(params double[] positions)
		{
			return CreateGroups((IEnumerable<double>)positions);
		}
	}

	/// <summary>
	/// Parameter which represts the record of the bolt in IDEA MPRL
	/// </summary>
	[DataContract]
	[Serializable]
	public class BoltParam : MprlRecord
	{
		/// <summary>
		/// Gets or sets the coordinate system type.
		/// </summary>
		[DataMember]
		public CoordinateSystemMethod CoordinateSystem { get; set; }

		#region Positions

		/// <summary>
		/// Gets or sets the fasteners positions {X, Y}.
		/// </summary>
		[DataMember]
		public List<IdeaRS.OpenModel.Geometry2D.Point> Positions { get; set; }

		#endregion

		#region Cartesian

		/// <summary>
		/// Gets or sets the rows of a fasteners positions.
		/// Absolute positions of groups, in group are relative positions.
		/// </summary>
		[DataMember]
		public NumberGroups Rows { get; set; }

		/// <summary>
		/// Gets or sets the columns of fasteners positions.
		/// </summary>
		[DataMember]
		public NumberGroups Cols { get; set; }

		/// <summary>
		/// Gets or sets the rows of a negative fasteners positions in case of RowsPosition equals to Profile.
		/// Absolute positions of groups, in group are relative positions.
		/// </summary>
		[DataMember]
		public NumberGroups RowsNegative { get; set; }

		/// <summary>
		/// Gets or sets the rows of a negative fasteners positions in case of RowsPosition equals to Profile.
		/// Absolute positions of groups, in group are relative positions.
		/// </summary>
		[DataMember]
		public NumberGroups ColsNegative { get; set; }

		/// <summary>
		/// Gets or sets the type of symmetry of rows positions.
		/// </summary>
		[DataMember]
		public Symmetry RowsSymmetry { get; set; }

		/// <summary>
		/// Gets or sets the type of symmetry of columns positions.
		/// </summary>
		[DataMember]
		public Symmetry ColsSymmetry { get; set; }

		/// <summary>
		/// Gets or sets the position, to which are bolt rows realted to.
		/// </summary>
		[DataMember]
		public PositionRelatedTo RowsPosition { get; set; }

		/// <summary>
		/// Gets or sets the position, to which are bolt columns realted to.
		/// </summary>
		[DataMember]
		public PositionRelatedTo ColsPosition { get; set; }

		/// <summary>
		/// Gets or sets the type of rows - regular or shifted.
		/// </summary>
		[DataMember]
		public ConnectorGridType RowsGridType { get; set; }

		/// <summary>
		/// Gets or sets the type of cols - regular or shifted.
		/// </summary>
		[DataMember]
		public ConnectorGridType ColsGridType { get; set; }

		#endregion Cartesian

		#region Polar

		/// <summary>
		/// Gets or sets the type, that spefifies polar input.
		/// </summary>
		[DataMember]
		public PolarInputType PolarInput { get; set; }

		/// <summary>
		/// Gets or sets the count of bolts in the circle (polar coordinate system).
		/// </summary>
		[DataMember]
		public IList<int> Counts { get; set; }

		/// <summary>
		/// Gets or sets the radii of bolts (polar coordinate system).
		/// </summary>
		[DataMember]
		public NumberGroups Radii { get; set; }

		/// <summary>
		/// Gets or sets the groups and positions for angles.
		/// </summary>
		[DataMember]
		public NumberGroups Angles { get; set; }

		/// <summary>
		/// Gets or sets the position, to which are bolt radii realted to (polar coordinate system).
		/// </summary>
		[DataMember]
		public PositionRelatedTo PolarPosition { get; set; }

		#endregion Polar

		/// <summary>
		/// Indicates, whether a shear plane is in the thread of a bolt.
		/// </summary>
		[DataMember]
		public bool ShearInThread { get; set; }

		/// <summary>
		/// Indicates type of shear transfer
		/// </summary>
		[DataMember]
		public BoltShearType BoltInteraction { get; set; }
	}
}