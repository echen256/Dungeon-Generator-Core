using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGeneratorCore.Generator.Geometry
{
    class Rotation
    {
		public static Point GetRotatedPoint(double radian, Point p)
		{ 
			var rotatedPoint = new Point(
					(int)Math.Round(p.X * Math.Cos(radian) - p.Y * Math.Sin(radian)),
					(int)Math.Round(p.X * Math.Sin(radian) + p.Y * Math.Cos(radian))
				);
			return rotatedPoint;

		}
	}
}
