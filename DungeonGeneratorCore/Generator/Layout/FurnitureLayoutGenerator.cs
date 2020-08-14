using Dungeon_Generator_Core.Generator;
using System;
using System.Collections.Generic;
using System.Text;
using Dungeon_Generator_Core.Geometry;
using System.Linq;

using System.Data;
using System.Diagnostics;
using NetTopologySuite.Utilities;

namespace Dungeon_Generator_Core.Layout
{

	using Point = Dungeon_Generator_Core.Geometry.Point;
	public class FurnitureLayoutGenerator
    {
		Random random = new Random();
        public List<IProp> generateRoomLayout (Room room, List<IProp> props, int money)
        {
			List<IProp> placedProps = new List<IProp>();
			List<Point> loopPoints = new List<Point>( room.getUsableInnerPoints());
			var maxCycles = 30;
			var cycles = 0;

			while (loopPoints.Count > 0 && cycles < maxCycles)
			{
				if (loopPoints.Count == 0) return placedProps;
				props = props.OrderBy((a) => { return random.NextDouble(); }).ToList();
				loopPoints = loopPoints.OrderBy((a) => {
					return random.NextDouble();
				}).ToList();

				var p = loopPoints[0];
				cycles++;
				var validPropPositions = new List<PossiblePropPositions>();
				
				for (var j = 0; j < props.Count / 2; j++)
				{
					 multipleArrayFill(p, loopPoints, props[j], room, validPropPositions);	
				}

				if (validPropPositions.Count != 0) {
					var distributionFactor = 1;
					validPropPositions = validPropPositions.OrderByDescending(
					a => { return a.GetValue(distributionFactor); } ).ToList();
					var selectedPosition = validPropPositions[0];
					var positions = selectedPosition.possiblePositions; 
					positions.ForEach((point) => {
						drawProp(point,selectedPosition.prop, loopPoints, placedProps);
					}); 
				}

			 
			}
            return placedProps;
        }
		public void multipleArrayFill   (Point point, List<Point> points, IProp prop, Room room, List<PossiblePropPositions> validPropPositions)  {
			for (var angle = 0; angle < 4; angle += 1)
			{
				var radian = Math.PI / 2 * angle;

				var tempProp = prop.GetRotatedProp(radian);

				Directions.directions.ForEach((dir) => {  
					validPropPositions.Add(arrayFill(point, points, tempProp, dir, room));	 
				});
			}	 
		}

		public bool checkIfPropFits   (Point point, List<Point> points, IProp prop, Room room)  {
			for (var i = 0; i < prop.Width(); i++)
			{
				for (var j = 0; j < prop.Height(); j++)
				{
					var p2 = new Point(point.X + i, point.Y + j); 

					if (!points.Contains(p2))
					{
						return false;
					}

				}
			}
			if (! prop.WallHugger()) return false;
			return checkWallHuggerCondition(point, room, prop) == prop.WallHugger();
		}

		public bool checkWallHuggerCondition  (Point point, Room room, IProp prop)  {
			var edgePoints = new List<Point>();
			for (var i = 0; i < prop.Width(); i++)
            {
				for (var j = 0; j < prop.Height(); j++)
                {
					edgePoints.Add(new Point(i, j)  + point + prop.Direction());
                }
            }

			return edgePoints.Intersect(room.edgePoints).Count() > 0;
		}

		public void drawProp  (Point point, IProp prop, List<Point> points,  List<IProp> placedProps )  {
			var offset = random.Next(1, 3);
			var width = prop.Width();
			var height = prop.Height(); 
			for (var i = -offset; i < width + offset; i++)
			{
				for (var j = -offset; j < height + offset; j++)
				{
					var p2 = new Point(point.X + i, point.Y + j);
					
					points.Remove(p2);
					 
				}
			}
			placedProps.Add(prop.GetPropAtPosition(point));
		}

		public PossiblePropPositions arrayFill(Point point, List<Point> points, IProp prop, Point direction, Room room)
		{
			var maxCount = random.Next(1, 5);
			var maxArea = random.Next(5, 9);
			var possiblePropPositions = new List<Point>();
			var propArea = prop.Width() * prop.Height();
			var currentArea = 0;

			while (currentArea < maxArea && possiblePropPositions.Count < maxCount)
			{
				if (checkIfPropFits(point, points, prop, room))
				{
					possiblePropPositions.Add(point);
					point = new Point(point.X + direction.X * prop.Width(), point.Y + direction.Y * prop.Height());
					currentArea += propArea;
				}
				else
				{
					break;
				}
			}
			return new PossiblePropPositions(prop, possiblePropPositions);
		}



	}

	public interface IProp
    {
		int Cost();
		string Identifier();
		bool WallHugger();
		int Width();
		int Height();

		Point Direction();

		IProp GetRotatedProp(double radian);

		Point Position();

		IProp GetPropAtPosition(Point p);
	}

	public class Prop : IProp
    {
		public int cost;
		public string color;
		public bool wallHugger;
		public int width;
		public int height;
		Point direction;
		Point position;
	
		public IProp GetRotatedProp(double radian)
		{ 
			var newSizeX = (int)Math.Abs(Math.Round( Width() * Math.Cos(radian) - Height() * Math.Sin(radian)));
			var newSizeY = (int)Math.Abs(Math.Round( Width() * Math.Sin(radian) +  Height() * Math.Cos(radian)));
			var rotatedDirection = new Point(
					(int)Math.Round(direction.X * Math.Cos(radian) - direction.Y * Math.Sin(radian)),
					(int)Math.Round(direction.X * Math.Sin(radian) + direction.Y * Math.Cos(radian))
				);
			return new Prop(newSizeX, newSizeY,  Cost(),  Identifier(),  WallHugger(),rotatedDirection, position);

		}

		public  int Cost()
        {
			return cost;
        }
		public string Identifier()
        {
			return color;
        }
		public bool WallHugger() {
			return wallHugger;
		}
		public int Width() { return width;  }
		public int Height() { return height; }

		public Point Direction()
        {
			return direction;
        }

		public Point Position()
        {
			return position;
        }

		public IProp GetPropAtPosition(Point p)
        {
			return new Prop(width, height, cost, color, wallHugger, direction, p);
        }
		public Prop (int width, int height, int cost,string color, bool wallHugger, Point direction, Point position)
        {
			this.width = width;
			this.height = height;
			this.cost = cost;
			this.color = color;
			this.wallHugger = wallHugger;
			this.direction = direction;
			this.position = position;
        }
    }

	public class PossiblePropPositions
	{
		public IProp prop;
		public List<Point> possiblePositions; 

		public PossiblePropPositions(IProp prop, List<Point> positions )
		{
			this.prop = prop;
			this.possiblePositions = positions; 
		}

		public double GetValue(double distributionFactor)
        {
			return prop.Cost()  * possiblePositions.Count * distributionFactor  ;
        }
		 

	}
}
