using Dungeon_Generator_Core.Generator;
using System;
using System.Collections.Generic;
using System.Text;
using Dungeon_Generator_Core.Geometry;
using System.Linq;
using System.Data;

using System.Drawing;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;

namespace Dungeon_Generator_Core.Layout
{

	using Point = Dungeon_Generator_Core.Geometry.Point;
	public class FurnitureLayoutGenerator
    {
		Random random = new Random();
        public List<PlacedProp> generateRoomLayout (Room r, List<Prop> props, int money)
        {
			var points = r.innerPoints.ToList();
			var loopPoints = new List<Point>( points);
			var originalPoints = new List<Point>( points);
			var placedProps = new List<PlacedProp>();
			while (loopPoints.Count > 0)
			{
				
				loopPoints = loopPoints.OrderBy((a) => {
					return Point.getNeighbors(a, loopPoints).Count() ;
				}).ToList();

				var index = 0; 
				var p = loopPoints[index];
				loopPoints.RemoveAt(index);


				var validProps = new List<PossiblePropPositions>();
				props = props.OrderBy((a) => { return random.NextDouble(); } ).ToList();
				for (var j = 0; j < props.Count; j++)
				{
					var prop = props[j];

					var arrayFillPositions = multipleArrayFill(p, points, prop, originalPoints);

					if (arrayFillPositions.Count > 0)
					{
						validProps.Add(new PossiblePropPositions(prop,arrayFillPositions));
					}
				}
				if (validProps.Count != 0) {

					var selectedPosition = validProps[random.Next(0, validProps.Count)];
					var prop = selectedPosition.prop;
					var positions = selectedPosition.possiblePositions;
					money -= prop.cost;
					positions.ForEach((point) => {
						drawProp(point, points,prop, placedProps);
					}); 
				}
			}
            return placedProps;
        }

		public List<Point> arrayFill  (Point point,List<Point> points, Prop prop, Point direction, List<Point> originalPoints)  {
			var maxCount = random.Next(1, 5);
			var maxArea = random.Next(5,9);
			var positions = new List<Point>();
			var propArea = prop.width * prop.height;
			var currentArea = 0;

			while (currentArea < maxArea && positions.Count < maxCount)
			{
				if (checkIfPropFits(point, points, prop, originalPoints))
				{
					positions.Add(point);
					point = new Point(point.X + direction.X * prop.width, point.Y + direction.Y * prop.height);
			 
					currentArea += propArea;
				}
				else
				{
					return positions;
				}
			}
			return positions;
		}

		public List<Point> multipleArrayFill   (Point point, List<Point> points, Prop prop, List<Point> originalPoints)  {
		 
			var positions = new List<Point>();
			Directions.directions.ForEach((dir) => {
				var possiblePositions = arrayFill(point, points, prop, dir, originalPoints);
				if (possiblePositions.Count >= positions.Count)
				{
					positions = possiblePositions;
				}
			});
			return positions;
		}

		public bool checkIfPropFits   (Point point, List<Point> points, Prop prop, List<Point> originalPoints)  {
			for (var i = 0; i < prop.width; i++)
			{
				for (var j = 0; j < prop.height; j++)
				{
					var p2 = new Point(point.X + i, point.Y + j); 

					if (!points.Contains(p2))
					{
						return false;
					}

				}
			}
			var wallHuggerCondition = true;
			if (prop.wallHugger) {
				wallHuggerCondition = checkWallHuggerCondition(point, originalPoints, prop);
			}
			return true && wallHuggerCondition;
		}

		public bool checkWallHuggerCondition  (Point point, List<Point> points, Prop prop)  {
			for (var i = 0; i < prop.width; i++)
			{
				for (var j = 0; j < prop.height; j++)
				{
					var p2 =  new Point( point.X + i, point.Y + j );
					if (Point.getNeighbors(p2,points).Count == 4)
					{
						return false;
					}

				}
			}
			return true;
		}

		public void drawProp  (Point point, List<Point> points, Prop prop, List<PlacedProp> placedProps)  {
			for (var i = -1; i < prop.width + 1; i++)
			{
				for (var j = -1; j < prop.height + 1; j++)
				{
					var p2 = new Point(point.X + i, point.Y + j);
					points.Remove(p2);
					 
				}
			}
			placedProps.Add(new PlacedProp(prop, point));
    }
}



	public class Prop
    {
		public int cost;
		public Color color;
		public bool wallHugger;
		public int width;
		public int height;

		public Prop (int width, int height, int cost,Color color, bool wallHugger)
        {
			this.width = width;
			this.height = height;
			this.cost = cost;
			this.color = color;
			this.wallHugger = wallHugger;
        }
    }

	public class PlacedProp
	{
		public Prop prop;
		public Point position;

		public PlacedProp(Prop prop, Point position)
        {
			this.prop = prop;
			this.position = position;
        }
	}

	public class PossiblePropPositions
	{
		public Prop prop;
		public List<Point> possiblePositions;

	public PossiblePropPositions(Prop prop, List<Point> positions)
		{
			this.prop = prop;
			this.possiblePositions = positions;
		}
	}
}
