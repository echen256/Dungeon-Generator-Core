using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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

		public static Point GetRotatedPointAroundPivot (double radian, Point p, double pivotX, double pivotY)
        {

			var x = p.X - pivotX;
			var y = p.Y - pivotY;
			var rotatedPointX  = (x * Math.Cos(radian) - y * Math.Sin(radian));
			var rotatedPointY =  (x * Math.Sin(radian) + y * Math.Cos(radian));

			rotatedPointX += pivotX;
			rotatedPointY += pivotY;
			return new Point((int)Math.Round(rotatedPointX), (int)Math.Round(rotatedPointY));
		}
	}
}
