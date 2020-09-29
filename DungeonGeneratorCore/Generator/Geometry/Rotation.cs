using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;


namespace DungeonGeneratorCore.Generator.Geometry
{

	public class Flip
    {
		public static Point FlipPointOverAxis (Point p, Point lineOrigin, Point lineVector)
        {
			p -= lineOrigin;
			var l = lineVector.Magnitude();
			var l0 = lineVector;
			var values = new double[] {
				l0.X *l0.X - l0.Y * l0.Y, 2 * l0.X * l0.Y, 2 * l0.X * l0.Y,  l0.Y *l0.Y - l0.X * l0.X
			};
			Console.WriteLine(JsonConvert.SerializeObject(values));
			Matrix<double> flipMatrix = Matrix<double>.Build.Dense(2, 2,values);
			Console.WriteLine(flipMatrix);
			flipMatrix.Multiply(1 / (l * l));
			Console.WriteLine(flipMatrix);
			Matrix<double> pointMatrix = Matrix<double>.Build.Dense(2, 1, new double[] { p.X, p.Y });
		
			var result = flipMatrix.Multiply(pointMatrix);
			var point = new Point((int)result[0, 0], (int)result[1, 0]) + lineOrigin;
			
			Console.WriteLine(point);
		 

			return point;
        }
    }
    public class Rotation
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
