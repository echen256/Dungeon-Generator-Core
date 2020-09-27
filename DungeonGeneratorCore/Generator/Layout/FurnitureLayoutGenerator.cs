using DungeonGeneratorCore.Generator;
using System;
using System.Collections.Generic;
using DungeonGeneratorCore.Generator.TemplateProcessing;
using DungeonGeneratorCore.Generator.Geometry;
using DungeonGeneratorCore.Generator.Layout.FillTypes;
using System.Linq;
using System.Data; 

namespace DungeonGeneratorCore.Generator.Layout
{

	using Point = DungeonGeneratorCore.Generator.Geometry.Point;
	public class FurnitureLayoutGenerator
    {
		Random random = new Random();


		public List<IProp> generateRoomLayoutBasedOnTemplate (Room room, IPropCollection propCollection, TemplateProcessing.Template template)
        {
			List<IProp> props = propCollection.getPropList();
			List<IProp> placedProps = new List<IProp>();
			room = new Room(room.getUsableInnerPoints(), room.category);




			template.zones.ForEach((zone) => {
			
				if (zone.fillParameters.fillType == "simple")
                {
					new SimpleFill().execute(room,zone, propCollection, placedProps);
				} else if (zone.fillParameters.fillType == "partition")
                {
					new PartitionFill().execute(room,zone, propCollection, placedProps);
				} else if (zone.fillParameters.fillType == "distributed")
                {
					new DistributedFill().execute(room, zone, propCollection, placedProps);
				}
				
				
			});



			 
			return placedProps;
		}
		 









		public List<IProp> generateRoomLayout (Room room, IPropCollection propCollection, int money)
        {
			List<IProp> props = propCollection.getPropList();
			List<IProp> placedProps = new List<IProp>();
			List<Point> loopPoints = new List<Point>( room.getUsableInnerPoints());




			var maxCycles = 150;
			var cycles = 0;

			while (loopPoints.Count > 0 && cycles < maxCycles)
			{
				cycles++;
				if (loopPoints.Count == 0) return placedProps;
				
				
				props = props.OrderBy((a) => { return random.NextDouble(); }).ToList();
				loopPoints = loopPoints.OrderBy((a) => {
					return random.NextDouble();
				}).ToList();

				var p = loopPoints[0];
				
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
		//	if (! prop.WallHugger()) return false;
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
			var offset =1;
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
			var maxCount = prop.MaximumClusterCount();
			var maxArea = 20;
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

}
