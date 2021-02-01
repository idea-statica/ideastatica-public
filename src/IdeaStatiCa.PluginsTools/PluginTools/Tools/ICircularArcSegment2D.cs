using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CI.Geometry2D
{
	public interface ICircularArcSegment2D : ISegment2D
	{
		Point Point { get; }
	}
}
