using Dungeon_Generator_Core.Generator;
using System;
using System.Collections.Generic;
using System.Text;
using Dungeon_Generator_Core.Geometry;
using System.Linq;

using System.Data;
using System.Diagnostics;

namespace Dungeon_Generator_Core.Layout
{

	using Point = Dungeon_Generator_Core.Geometry.Point;
	public class FurnitureLayoutGenerator
    {
		Random random = new Random();
        public List<PlacedProp> generateRoomLayout (Room room, List<IProp> props, int money)
        {
	 
			var points = room.getUsableInnerPoints();

			room = new Room(room.getUsableInnerPoints(), room.category);

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

					var arrayFillPositions = multipleArrayFill(p, points, prop, room);

					if (arrayFillPositions.Count > 0)
					{
						validProps.Add(new PossiblePropPositions(prop,arrayFillPositions));
					}
				}
				if (validProps.Count != 0) {
					var distributionFactor = random.NextDouble() * 4  + 2;
					//validProps = validProps.OrderBy<PossiblePropPositions>() ;
					//validProps = validProps.OrderBy(a => a.prop.Width() * a.prop.Height()).ToList();
					validProps = validProps.OrderByDescending(a => a.GetValue(distributionFactor) ).ToList();
					 
					var selectedPosition = validProps[0];
					var positions = selectedPosition.possiblePositions;
					//money -= prop.Cost();
					positions.ForEach((point) => {
						drawProp(point, points, selectedPosition.prop, placedProps);
					}); 
				}
			}
            return placedProps;
        }

		public List<Point> arrayFill  (Point point,List<Point> points, IProp prop, Point direction, Room room)  {
			var maxCount = random.Next(1, 5);
			var maxArea = random.Next(5,9);
			var positions = new List<Point>();
			var propArea = prop.Width() * prop.Height();
			var currentArea = 0;

			while (currentArea < maxArea && positions.Count < maxCount)
			{
				if (checkIfPropFits(point, points, prop, room))
				{
					positions.Add(point);
					point = new Point(point.X + direction.X * prop.Width(), point.Y + direction.Y * prop.Height());
			 
					currentArea += propArea;
				}
				else
				{
					return positions;
				}
			}
			return positions;
		}

		public List<Point> multipleArrayFill   (Point point, List<Point> points, IProp prop, Room room)  {
		 
			var positions = new List<Point>();
			Directions.directions.ForEach((dir) => {
				var possiblePositions = arrayFill(point, points, prop, dir, room);
				if (possiblePositions.Count >= positions.Count)
				{
					positions = possiblePositions;
				}
			});
			return positions;
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
			var wallHuggerCondition = true;
			if (prop.WallHugger()) {
				wallHuggerCondition = checkWallHuggerCondition(point, room, prop);
			}
			return true && wallHuggerCondition;
		}

		public bool checkWallHuggerCondition  (Point point, Room room, IProp prop)  {

			var propRoom = new Room(new Rect(point, prop.Width(), prop.Height()),"prop");
			var edgePoints = room.edgePoints;
			return edgePoints.Intersect(propRoom.points).Count() > 0;
			/*for (var i = 0; i < prop.Width(); i++)
			{
				for (var j = 0; j < prop.Height(); j++)
				{
					var p2 =  new Point( point.X + i, point.Y + j );
					if (Point.getNeighbors(p2,points).Count == 4)
					{
						return false;
					}

				}
			}
			return true;*/
		}

		public void drawProp  (Point point, List<Point> points, IProp prop, List<PlacedProp> placedProps)  {
			for (var i = -1; i < prop.Width() + 1; i++)
			{
				for (var j = -1; j < prop.Height() + 1; j++)
				{
					var p2 = new Point(point.X + i, point.Y + j);
					points.Remove(p2);
					 
				}
			}
			placedProps.Add(new PlacedProp(prop, point));
    }
}

	public interface IProp
    {
		  int Cost();
		string Identifier();
		bool WallHugger();
		int Width();
		int Height();
	}

	public class Prop : IProp
    {
		public int cost;
		public string color;
		public bool wallHugger;
		public int width;
		public int height;

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

		public Prop (int width, int height, int cost,string color, bool wallHugger)
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
		public IProp prop;
		public Point position;

		public PlacedProp(IProp prop, Point position)
        {
			this.prop = prop;
			this.position = position;
        }
	}

	public class PossiblePropPositions
	{
		public IProp prop;
		public List<Point> possiblePositions;

	public PossiblePropPositions(IProp prop, List<Point> positions)
		{
			this.prop = prop;
			this.possiblePositions = positions;
		}

		public double GetValue(double distributionFactor)
        {
			return prop.Cost() / (distributionFactor * possiblePositions.Count) ;
        }
	}
}
