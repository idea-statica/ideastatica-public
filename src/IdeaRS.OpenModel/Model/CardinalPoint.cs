using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaRS.OpenModel.Model
{
	public enum InsertionPoints
	{
		/// <summary>
		/// Center of Gravity.
		/// </summary>
		CenterOfGravity,

		/// <summary>
		/// The upper of the two points located at the intersection of the Z axis and the outer rectangle.
		/// </summary>
		Top,

		/// <summary>
		/// The lower of the two points located at the intersection of the Z axis and the outer rectangle.
		/// </summary>
		Bottom,

		/// <summary>
		/// Left of two points located at the intersection of the Y axis and the outer rectangle.
		/// </summary>
		Left,

		/// <summary>
		/// Right of two points located at the intersection of the Y axis and the outer rectangle.
		/// </summary>
		Right,

		/// <summary>
		/// Upper right corner of the outer rectangle.
		/// </summary>
		TopRight,

		/// <summary>
		/// Upper left corner of the outer rectangle.
		/// </summary>
		TopLeft,

		/// <summary>
		/// Lower right corner of the outer rectangle.
		/// </summary>
		BottomRight,

		/// <summary>
		/// Lower left corner of the outer rectangle.
		/// </summary>
		BottomLeft,

		/// <summary>
		/// Point lying on the intersection of the centers of the upper and lower sides and the centers of the left and right sides of the outer rectangle.
		/// </summary>
		Center,

		/// <summary>
		/// Center of the top side of the outer rectangle.
		/// </summary>
		CenterTop,

		/// <summary>
		/// Center of the bottom side of the outer rectangle.
		/// </summary>
		CenterBottom,

		/// <summary>
		/// Center of the left side of the outer rectangle.
		/// </summary>
		CenterLeft,

		/// <summary>
		/// Center of the right side of the outer rectangle.
		/// </summary>
		CenterRight,
	}
}
