using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CI.GiCL2D
{
	[System.Reflection.Obfuscation(Feature = "renaming")]
	public struct Rect2D
	{
		private Rect myRc;

		//
		// Summary:
		//     Initializes a new instance of the IdaRs.GiCL2D.Rect2D structure that is of
		//     the specified size and is located at (0,0).
		//
		// Parameters:
		//   size:
		//     A System.Windows.Size structure that specifies the width and height of the
		//     rectangle.
		public Rect2D(Size size)
		{
			myRc = new Rect(new Point(0, 0), size);
		}

		//
		// Summary:
		//     Initializes a new instance of the IdaRs.GiCL2D.Rect2D structure that is of
		//     the specified rectangle System.Windows.Rect.
		//
		// Parameters:
		//   size:
		//     A System.Windows.Size structure that specifies the width and height of the
		//     rectangle.
		public Rect2D(Rect rect)
		{
			myRc = rect;
		}

		//
		// Summary:
		//     Initializes a new instance of the IdaRs.GiCL2D.Rect2D structure that is exactly
		//     large enough to contain the two specified points.
		//
		// Parameters:
		//   point1:
		//     The first point that the new rectangle must contain.
		//
		//   point2:
		//     The second point that the new rectangle must contain.
		public Rect2D(Point point1, Point point2)
		{
			myRc = new Rect(point1, point2);
		}

		//
		// Summary:
		//     Initializes a new instance of the IdaRs.GiCL2D.Rect2D structure that has
		//     the specified bottom-left corner location and the specified width and height.
		//
		// Parameters:
		//   location:
		//     A point that specifies the location of the bottom-left corner of the rectangle.
		//
		//   size:
		//     A System.Windows.Size structure that specifies the width and height of the
		//     rectangle.
		public Rect2D(Point location, Size size)
		{
			myRc = new Rect(location, size);
		}

		//
		// Summary:
		//     Initializes a new instance of the IdaRs.GiCL2D.Rect2D structure that is exactly
		//     large enough to contain the specified point and the sum of the specified
		//     point and the specified vector.
		//
		// Parameters:
		//   point:
		//     The first point the rectangle must contain.
		//
		//   vector:
		//     The amount to offset the specified point. The resulting rectangle will be
		//     exactly large enough to contain both points.
		public Rect2D(Point point, Vector vector)
		{
			myRc = new Rect(point, vector);
		}

		//
		// Summary:
		//     Initializes a new instance of the IdaRs.GiCL2D.Rect2D structure that has
		//     the specified x-coordinate, y-coordinate, width, and height.
		//
		// Parameters:
		//   x:
		//     The x-coordinate of the left-bottom corner of the rectangle.
		//
		//   y:
		//     The y-coordinate of the left-bottom corner of the rectangle.
		//
		//   width:
		//     The width of the rectangle.
		//
		//   height:
		//     The height of the rectangle.
		public Rect2D(double x, double y, double width, double height)
		{
			myRc = new Rect(x, y, width, height);
		}

		// Summary:
		//     Compares two rectangles for inequality.
		//
		// Parameters:
		//   rect1:
		//     The first rectangle to compare.
		//
		//   rect2:
		//     The second rectangle to compare.
		//
		// Returns:
		//     true if the rectangles do not have the same IdaRs.GiCL2D.Rect2D.Location
		//     and IdaRs.GiCL2D.Rect2D.Size values; otherwise, false.
		public static bool operator !=(Rect2D rect1, Rect2D rect2)
		{
			return rect1.myRc != rect2.myRc;
		}

		//
		// Summary:
		//     Compares two rectangles for exact equality.
		//
		// Parameters:
		//   rect1:
		//     The first rectangle to compare.
		//
		//   rect2:
		//     The second rectangle to compare.
		//
		// Returns:
		//     true if the rectangles have the same IdaRs.GiCL2D.Rect2D.Location and IdaRs.GiCL2D.Rect2D.Size
		//     values; otherwise, false.
		public static bool operator ==(Rect2D rect1, Rect2D rect2)
		{
			return rect1.myRc == rect2.myRc;
		}

		public static implicit operator Rect(Rect2D src)
		{
			return src.myRc;
		}

		public static implicit operator Rect2D(Rect src)
		{
			return new Rect2D(src);
		}

		//

		// Summary:
		//     Gets the y-axis value of the bottom of the rectangle.
		//
		// Returns:
		//     The y-axis value of the bottom of the rectangle. If the rectangle is empty,
		//     the value is System.Double.NegativeInfinity .
		public double Bottom { get { return myRc.Top; } }

		//
		// Summary:
		//     Gets the position of the bottom-left corner of the rectangle
		//
		// Returns:
		//     The position of the bottom-left corner of the rectangle.
		public Point LeftBottom { get { return myRc.TopLeft; } }

		//
		// Summary:
		//     Gets the position of the bottom-right corner of the rectangle.
		//
		// Returns:
		//     The position of the bottom-right corner of the rectangle.
		public Point RightBottom { get { return myRc.TopRight; } }

		//
		// Summary:
		//     Gets a special value that represents a rectangle with no position or area.
		//
		// Returns:
		//     The empty rectangle, which has IdaRs.GiCL2D.Rect2D.X and IdaRs.GiCL2D.Rect2D.Y
		//     property values of System.Double.PositiveInfinity, and has IdaRs.GiCL2D.Rect2D.Width
		//     and IdaRs.GiCL2D.Rect2D.Height property values of System.Double.NegativeInfinity.
		public static Rect2D Empty { get { return new Rect2D(Rect.Empty); } }

		//
		// Summary:
		//     Gets or sets the height of the rectangle.
		//
		// Returns:
		//     A positive number that represents the height of the rectangle. The default
		//     is 0.
		public double Height { get { return myRc.Height; } set { myRc.Height = value; } }

		//
		// Summary:
		//     Gets a value that indicates whether the rectangle is the IdaRs.GiCL2D.Rect2D.Empty
		//     rectangle.
		//
		// Returns:
		//     true if the rectangle is the IdaRs.GiCL2D.Rect2D.Empty rectangle; otherwise,
		//     false.
		public bool IsEmpty { get { return myRc.IsEmpty; } }

		//
		// Summary:
		//     Gets the x-axis value of the left side of the rectangle.
		//
		// Returns:
		//     The x-axis value of the left side of the rectangle.
		public double Left { get { return myRc.Left; } }

		//
		// Summary:
		//     Gets or sets the position of the bottom-left corner of the rectangle.
		//
		// Returns:
		//     The position of the bottom-left corner of the rectangle. The default is (0,
		//     0).
		public Point Location { get { return myRc.Location; } set { myRc.Location = value; } }

		//
		// Summary:
		//     Gets the x-axis value of the right side of the rectangle.
		//
		// Returns:
		//     The x-axis value of the right side of the rectangle.
		public double Right { get { return myRc.Right; } }

		//
		// Summary:
		//     Gets or sets the width and height of the rectangle.
		//
		// Returns:
		//     A System.Windows.Size structure that specifies the width and height of the
		//     rectangle.
		public Size Size { get { return myRc.Size; } set { myRc.Size = value; } }

		//
		// Summary:
		//     Gets the y-axis position of the top of the rectangle.
		//
		// Returns:
		//     The y-axis position of the top of the rectangle.
		public double Top { get { return myRc.Bottom; } }

		//
		// Summary:
		//     Gets the position of the top-left corner of the rectangle.
		//
		// Returns:
		//     The position of the top-left corner of the rectangle.
		public Point LeftTop { get { return myRc.BottomLeft; } }

		//
		// Summary:
		//     Gets the position of the top-right corner of the rectangle.
		//
		// Returns:
		//     The position of the top-right corner of the rectangle.
		public Point RightTop { get { return myRc.BottomRight; } }

		//
		// Summary:
		//     Gets or sets the width of the rectangle.
		//
		// Returns:
		//     A positive number that represents the width of the rectangle. The default
		//     is 0.
		public double Width { get { return myRc.Width; } set { myRc.Width = value; } }

		//
		// Summary:
		//     Gets or sets the x-axis value of the left side of the rectangle.
		//
		// Returns:
		//     The x-axis value of the left side of the rectangle.
		public double X { get { return myRc.X; } set { myRc.X = value; } }

		//
		// Summary:
		//     Gets or sets the y-axis value of the bottom side of the rectangle.
		//
		// Returns:
		//     The y-axis value of the bottom side of the rectangle.
		public double Y { get { return myRc.Y; } set { myRc.Y = value; } }

		// Summary:
		//     Indicates whether the rectangle contains the specified point.
		//
		// Parameters:
		//   point:
		//     The point to check.
		//
		// Returns:
		//     true if the rectangle contains the specified point; otherwise, false.
		public bool Contains(Point point)
		{
			return myRc.Contains(point);
		}

		//
		// Summary:
		//     Indicates whether the rectangle contains the specified rectangle.
		//
		// Parameters:
		//   rect:
		//     The rectangle to check.
		//
		// Returns:
		//     true if rect is entirely contained by the rectangle; otherwise, false.
		public bool Contains(Rect2D rect)
		{
			return myRc.Contains(rect.myRc);
		}

		//
		// Summary:
		//     Indicates whether the rectangle contains the specified x-coordinate and y-coordinate.
		//
		// Parameters:
		//   x:
		//     The x-coordinate of the point to check.
		//
		//   y:
		//     The y-coordinate of the point to check.
		//
		// Returns:
		//     true if (x, y) is contained by the rectangle; otherwise, false.
		public bool Contains(double x, double y)
		{
			return myRc.Contains(x, y);
		}

		//
		// Summary:
		//     Indicates whether the specified object is equal to the current rectangle.
		//
		// Parameters:
		//   o:
		//     The object to compare to the current rectangle.
		//
		// Returns:
		//     true if o is a IdaRs.GiCL2D.Rect2D and has the same IdaRs.GiCL2D.Rect2D.Location
		//     and IdaRs.GiCL2D.Rect2D.Size values as the current rectangle; otherwise,
		//     false.
		public override bool Equals(object o)
		{
			if (o is Rect2D)
			{
				return myRc.Equals(((Rect2D)o).myRc);
			}
			if (o is Rect)
			{
				return myRc.Equals(o);
			}
			return false;
		}

		//
		// Summary:
		//     Indicates whether the specified rectangle is equal to the current rectangle.
		//
		// Parameters:
		//   value:
		//     The rectangle to compare to the current rectangle.
		//
		// Returns:
		//     true if the specified rectangle has the same IdaRs.GiCL2D.Rect2D.Location
		//     and IdaRs.GiCL2D.Rect2D.Size values as the current rectangle; otherwise,
		//     false.
		public bool Equals(Rect2D value)
		{
			return myRc.Equals(value.myRc);
		}

		//
		// Summary:
		//     Indicates whether the specified rectangles are equal.
		//
		// Parameters:
		//   rect1:
		//     The first rectangle to compare.
		//
		//   rect2:
		//     The second rectangle to compare.
		//
		// Returns:
		//     true if the rectangles have the same IdaRs.GiCL2D.Rect2D.Location and IdaRs.GiCL2D.Rect2D.Size
		//     values; otherwise, false.
		public static bool Equals(Rect2D rect1, Rect2D rect2)
		{
			return Rect.Equals(rect1.myRc, rect2.myRc);
		}

		//
		// Summary:
		//     Creates a hash code for the rectangle.
		//
		// Returns:
		//     A hash code for the current IdaRs.GiCL2D.Rect2D structure.
		public override int GetHashCode()
		{
			return myRc.GetHashCode();
		}

		//
		// Summary:
		//     Expands the rectangle by using the specified System.Windows.Size, in all
		//     directions.
		//
		// Parameters:
		//   size:
		//     Specifies the amount to expand the rectangle. The System.Windows.Size structure's
		//     System.Windows.Size.Width property specifies the amount to increase the rectangle's
		//     IdaRs.GiCL2D.Rect2D.Left and IdaRs.GiCL2D.Rect2D.Right properties. The System.Windows.Size
		//     structure's System.Windows.Size.Height property specifies the amount to increase
		//     the rectangle's IdaRs.GiCL2D.Rect2D.Top and IdaRs.GiCL2D.Rect2D.Bottom properties.
		public void Inflate(Size size)
		{
			myRc.Inflate(size);
		}

		//
		// Summary:
		//     Expands or shrinks the rectangle by using the specified width and height
		//     amounts, in all directions.
		//
		// Parameters:
		//   width:
		//     The amount by which to expand or shrink the left and right sides of the rectangle.
		//
		//   height:
		//     The amount by which to expand or shrink the top and bottom sides of the rectangle.
		public void Inflate(double width, double height)
		{
			myRc.Inflate(width, height);
		}

		//
		// Summary:
		//     Returns the rectangle that results from expanding the specified rectangle
		//     by the specified System.Windows.Size, in all directions.
		//
		// Parameters:
		//   rect:
		//     The IdaRs.GiCL2D.Rect2D structure to modify.
		//
		//   size:
		//     Specifies the amount to expand the rectangle. The System.Windows.Size structure's
		//     System.Windows.Size.Width property specifies the amount to increase the rectangle's
		//     IdaRs.GiCL2D.Rect2D.Left and IdaRs.GiCL2D.Rect2D.Right properties. The System.Windows.Size
		//     structure's System.Windows.Size.Height property specifies the amount to increase
		//     the rectangle's IdaRs.GiCL2D.Rect2D.Top and IdaRs.GiCL2D.Rect2D.Bottom properties.
		//
		// Returns:
		//     The resulting rectangle.
		public static Rect2D Inflate(Rect2D rect, Size size)
		{
			return new Rect2D(Rect.Inflate(rect.myRc, size));
		}

		//
		// Summary:
		//     Creates a rectangle that results from expanding or shrinking the specified
		//     rectangle by the specified width and height amounts, in all directions.
		//
		// Parameters:
		//   rect:
		//     The IdaRs.GiCL2D.Rect2D structure to modify.
		//
		//   width:
		//     The amount by which to expand or shrink the left and right sides of the rectangle.
		//
		//   height:
		//     The amount by which to expand or shrink the top and bottom sides of the rectangle.
		//
		// Returns:
		//     The resulting rectangle.
		public static Rect2D Inflate(Rect2D rect, double width, double height)
		{
			return new Rect2D(Rect.Inflate(rect.myRc, width, height));
		}

		//
		// Summary:
		//     Finds the intersection of the current rectangle and the specified rectangle,
		//     and stores the result as the current rectangle.
		//
		// Parameters:
		//   rect:
		//     The rectangle to intersect with the current rectangle.
		public void Intersect(Rect2D rect)
		{
			myRc.Intersect(rect.myRc);
		}

		//
		// Summary:
		//     Returns the intersection of the specified rectangles.
		//
		// Parameters:
		//   rect1:
		//     The first rectangle to compare.
		//
		//   rect2:
		//     The second rectangle to compare.
		//
		// Returns:
		//     The intersection of the two rectangles, or IdaRs.GiCL2D.Rect2D.Empty if no
		//     intersection exists.
		public static Rect2D Intersect(Rect2D rect1, Rect2D rect2)
		{
			return new Rect2D(Rect.Intersect(rect1.myRc, rect2.myRc));
		}

		//
		// Summary:
		//     Indicates whether the specified rectangle intersects with the current rectangle.
		//
		// Parameters:
		//   rect:
		//     The rectangle to check.
		//
		// Returns:
		//     true if the specified rectangle intersects with the current rectangle; otherwise,
		//     false.
		public bool IntersectsWith(Rect2D rect)
		{
			return myRc.IntersectsWith(rect.myRc);
		}

		//
		// Summary:
		//     Moves the rectangle by the specified vector.
		//
		// Parameters:
		//   offsetVector:
		//     A vector that specifies the horizontal and vertical amounts to move the rectangle.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     This method is called on the IdaRs.GiCL2D.Rect2D.Empty rectangle.
		public void Offset(Vector offsetVector)
		{
			myRc.Offset(offsetVector);
		}

		//
		// Summary:
		//     Moves the rectangle by the specified horizontal and vertical amounts.
		//
		// Parameters:
		//   offsetX:
		//     The amount to move the rectangle horizontally.
		//
		//   offsetY:
		//     The amount to move the rectangle vertically.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     This method is called on the IdaRs.GiCL2D.Rect2D.Empty rectangle.
		public void Offset(double offsetX, double offsetY)
		{
			myRc.Offset(offsetX, offsetY);
		}

		//
		// Summary:
		//     Returns a rectangle that is offset from the specified rectangle by using
		//     the specified vector.
		//
		// Parameters:
		//   rect:
		//     The original rectangle.
		//
		//   offsetVector:
		//     A vector that specifies the horizontal and vertical offsets for the new rectangle.
		//
		// Returns:
		//     The resulting rectangle.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     rect is IdaRs.GiCL2D.Rect2D.Empty.
		public static Rect2D Offset(Rect2D rect, Vector offsetVector)
		{
			return new Rect2D(Rect.Offset(rect.myRc, offsetVector));
		}

		//
		// Summary:
		//     Returns a rectangle that is offset from the specified rectangle by using
		//     the specified horizontal and vertical amounts.
		//
		// Parameters:
		//   rect:
		//     The rectangle to move.
		//
		//   offsetX:
		//     The horizontal offset for the new rectangle.
		//
		//   offsetY:
		//     The vertical offset for the new rectangle.
		//
		// Returns:
		//     The resulting rectangle.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     rect is IdaRs.GiCL2D.Rect2D.Empty.
		public static Rect2D Offset(Rect2D rect, double offsetX, double offsetY)
		{
			return new Rect2D(Rect.Offset(rect.myRc, offsetX, offsetY));
		}

		//
		// Summary:
		//     Creates a new rectangle from the specified string representation.
		//
		// Parameters:
		//   source:
		//     The string representation of the rectangle, in the form "x, y, width, height".
		//
		// Returns:
		//     The resulting rectangle.
		public static Rect2D Parse(string source)
		{
			return new Rect2D(Rect.Parse(source));
		}

		//
		// Summary:
		//     Multiplies the size of the current rectangle by the specified x and y values.
		//
		// Parameters:
		//   scaleX:
		//     The scale factor in the x-direction.
		//
		//   scaleY:
		//     The scale factor in the y-direction.
		public void Scale(double scaleX, double scaleY)
		{
			myRc.Scale(scaleX, scaleY);
		}

		//
		// Summary:
		//     Returns a string representation of the rectangle.
		//
		// Returns:
		//     A string representation of the current rectangle. The string has the following
		//     form: "IdaRs.GiCL2D.Rect2D.X,IdaRs.GiCL2D.Rect2D.Y,IdaRs.GiCL2D.Rect2D.Width,IdaRs.GiCL2D.Rect2D.Height".
		public override string ToString()
		{
			return myRc.ToString();
		}

		//
		// Summary:
		//     Returns a string representation of the rectangle by using the specified format
		//     provider.
		//
		// Parameters:
		//   provider:
		//     Culture-specific formatting information.
		//
		// Returns:
		//     A string representation of the current rectangle that is determined by the
		//     specified format provider.
		public string ToString(IFormatProvider provider)
		{
			return myRc.ToString(provider);
		}

		//
		// Summary:
		//     Transforms the rectangle by applying the specified matrix.
		//
		// Parameters:
		//   matrix:
		//     A matrix that specifies the transformation to apply.
		public void Transform(Matrix matrix)
		{
			myRc.Transform(matrix);
		}

		//
		// Summary:
		//     Returns the rectangle that results from applying the specified matrix to
		//     the specified rectangle.
		//
		// Parameters:
		//   rect:
		//     A rectangle that is the basis for the transformation.
		//
		//   matrix:
		//     A matrix that specifies the transformation to apply.
		//
		// Returns:
		//     The rectangle that results from the operation.
		public static Rect2D Transform(Rect2D rect, Matrix matrix)
		{
			return new Rect2D(Rect.Transform(rect.myRc, matrix));
		}

		//
		// Summary:
		//     Expands the current rectangle exactly enough to contain the specified point.
		//
		// Parameters:
		//   point:
		//     The point to include.
		public void Union(Point point)
		{
			myRc.Union(point);
		}

		//
		// Summary:
		//     Expands the current rectangle exactly enough to contain the specified rectangle.
		//
		// Parameters:
		//   rect:
		//     The rectangle to include.
		public void Union(Rect2D rect)
		{
			myRc.Union(rect.myRc);
		}

		//
		// Summary:
		//     Creates a rectangle that is exactly large enough to include the specified
		//     rectangle and the specified point.
		//
		// Parameters:
		//   rect:
		//     The rectangle to include.
		//
		//   point:
		//     The point to include.
		//
		// Returns:
		//     A rectangle that is exactly large enough to contain the specified rectangle
		//     and the specified point.
		public static Rect2D Union(Rect2D rect, Point point)
		{
			return new Rect2D(Rect.Union(rect.myRc, point));
		}

		//
		// Summary:
		//     Creates a rectangle that is exactly large enough to contain the two specified
		//     rectangles.
		//
		// Parameters:
		//   rect1:
		//     The first rectangle to include.
		//
		//   rect2:
		//     The second rectangle to include.
		//
		// Returns:
		//     The resulting rectangle.
		public static Rect2D Union(Rect2D rect1, Rect2D rect2)
		{
			return new Rect2D(Rect.Union(rect1.myRc, rect2.myRc));
		}

		public Point Center
		{
			get
			{
				return new Point(X + (Width / 2), Y + (Height / 2));
			}
			set
			{
				X = value.X - (Width / 2);
				Y = value.Y - (Height / 2);
			}
		}

		public double MaxSize
		{
			get
			{
				if (IsEmpty) return 0;
				if (Height > Width) return Height;
				else return Width;
			}
		}

		public List<Point> MakeListPoint()
		{
			return new List<Point>() { LeftBottom, RightBottom, RightTop, LeftTop };
		}

		public double MinSize
		{
			get
			{
				if (IsEmpty) return 0;
				if (Height < Width) return Height;
				else return Width;
			}
		}

		public static Rect2D operator *(Rect2D src, double scale)
		{
			return new Rect2D(src.X * scale, src.Y * scale, src.Width * scale, src.Height * scale);
		}

		public void Move(double dx, double dy)
		{
			if (!myRc.IsEmpty)
			{
				myRc.X = myRc.X + dx;
				myRc.Y = myRc.Y + dy;
			}
		}
	}
}