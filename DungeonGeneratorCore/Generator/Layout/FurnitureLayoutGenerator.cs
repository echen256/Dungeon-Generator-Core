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

				for (var j = 0; j < props.Count / 2; j++)
				{
					var prop = props[j];

					for (var angle = 0; angle < 4; angle += 1)
					{
						var radian = Math.PI / 2 * angle; 

						var tempProp = prop.GetRotatedProp(radian);

						var arrayFillPositions = multipleArrayFill(p, points, tempProp, room);

						if (arrayFillPositions.Count > 0)
						{
							validProps.Add(new PossiblePropPositions(prop, arrayFillPositions, radian));
						}
					}

					
				}
				if (validProps.Count != 0) {
					var distributionFactor = random.NextDouble() * 4  + 2;
					//validProps = validProps.OrderBy<PossiblePropPositions>() ;
					//validProps = validProps.OrderBy(a => a.prop.Width() * a.prop.Height()).ToList();
					validProps = validProps.OrderByDescending(a => { return a.GetValue(distributionFactor); } ).ToList();
					 
					var selectedPosition = validProps[0];
					var positions = selectedPosition.possiblePositions;
					//money -= prop.Cost();
					positions.ForEach((point) => {
						drawProp(point, points, selectedPosition.prop, placedProps, selectedPosition.rotation);
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
			if (prop.WallHugger()) {
				return checkWallHuggerCondition(point, room, prop);
			}
			return true;
		}

		public bool checkWallHuggerCondition  (Point point, Room room, IProp prop)  {

			var propRoom = new Room(new Rect(point, prop.Width(), prop.Height()),"prop");
			var edgePoints = room.edgePoints;
			return edgePoints.Intersect(propRoom.points).Count() > 0;
		}

		public void drawProp  (Point point, List<Point> points, IProp prop, List<PlacedProp> placedProps, double rotation)  {
			var offset = random.Next(1, 3);
			var width = prop.GetRotatedProp(rotation).width;
			var height = prop.GetRotatedProp(rotation).height; 
			for (var i = -offset; i < width + offset; i++)
			{
				for (var j = -offset; j < height + offset; j++)
				{
					var p2 = new Point(point.X + i, point.Y + j);
					points.Remove(p2);
					 
				}
			}
			placedProps.Add(new PlacedProp(prop, point,rotation));
    }
}

	public interface IProp
    {
		  int Cost();
		string Identifier();
		bool WallHugger();
		int Width();
		int Height();

		Prop GetRotatedProp(double radian);

	
	}

	public class Prop : IProp
    {
		public int cost;
		public string color;
		public bool wallHugger;
		public int width;
		public int height;
	
		public Prop GetRotatedProp(double radian)
		{ 
			var newSizeX = (int)Math.Abs(Math.Round( Width() * Math.Cos(radian) - Height() * Math.Sin(radian)));
			var newSizeY = (int)Math.Abs(Math.Round( Width() * Math.Sin(radian) +  Height() * Math.Cos(radian)));
			return   new Prop(newSizeX, newSizeY,  Cost(),  Identifier(),  WallHugger());

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
		public double rotation;


		public PlacedProp(IProp prop, Point position, double rotation)
        {
			this.prop = prop;
			this.position = position;
			this.rotation = rotation;
        }
	}

	public class PossiblePropPositions
	{
		public IProp prop;
		public List<Point> possiblePositions;
		public double rotation;

	public PossiblePropPositions(IProp prop, List<Point> positions, double rotation)
		{
			this.prop = prop;
			this.possiblePositions = positions;
			this.rotation = rotation;
		}

		public double GetValue(double distributionFactor)
        {
			return prop.Cost()  * possiblePositions.Count  ;
        }
	}
}
