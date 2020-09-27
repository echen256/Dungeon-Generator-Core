using DungeonGeneratorCore.Generator;
using System;
using System.Collections.Generic;
using DungeonGeneratorCore.Generator.TemplateProcessing;
using DungeonGeneratorCore.Generator.Geometry;
using System.Linq;
using System.Data;

namespace DungeonGeneratorCore.Generator.Layout.FillTypes
{

	using Point = DungeonGeneratorCore.Generator.Geometry.Point;
	public class DistributedFill
	{
		Random random = new Random();

		public void execute(Room room, Zone zone, IPropCollection propCollection, List<IProp> placedProps)
		{

			var processedZone = new ProcessedZone(room, zone);
			var zonePoints = processedZone.getPointsInZone().ToList();

			var validPropCollections = propCollection.getPropList();
			validPropCollections = validPropCollections.FindAll((prop) => { return processedZone.tags.Contains(prop.Identifier()); });

			int cycles = 20;
			 
			while (cycles > 0)
            {

				cycles--;
				Console.WriteLine(processedZone.boundingRect.Area);
				if (processedZone.boundingRect.Area <= 0)
				{
					break;
				}

				var validPropPositions = new List<PossiblePropPositionsTemplate>();
				var prop = validPropCollections[0];
				validPropCollections.Add(prop);
				validPropCollections.RemoveAt(0);
				SampleAllRotations(processedZone, prop, validPropPositions, zonePoints);

				if (validPropPositions.Count > 0)
				{
					validPropPositions.OrderBy((col) => { return zonePoints.Count - col.prop.Width() * col.prop.Height() * col.possiblePositions.Count(); });  
					var selectedPosition = validPropPositions[0]; 
	
					var positions = selectedPosition.possiblePositions;
					var w = selectedPosition.boundingRect.Width;
					var h = selectedPosition.boundingRect.Height;
					var x0 = selectedPosition.boundingRect.minX;
					var y0 = selectedPosition.boundingRect.minY; 
					positions.ForEach((point) =>
					{
						drawProp(point, selectedPosition.prop, zonePoints, placedProps);
					
					});

					processedZone = new ProcessedZone(selectedPosition.remainderRect,   zone);

					
				}
				
			}

			
		}
		public void SampleAllRotations(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemplate> validPropPositions, List<Point> zonePoints)
		{
			for (var angle = 1; angle < 2; angle += 1)
			{
				var radian = Math.PI / 2 * angle;
				TryDistributedFill(processedZone, prop.GetRotatedProp(radian), validPropPositions,   zonePoints);
			}
		}


		public void TryDistributedFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemplate> validPropPositions, List<Point> zonePoints)
		{

			Rect boundingRect = processedZone.boundingRect;
			Rect remainderRect;
		
			if (prop.Width() > processedZone.Width || prop.Height() > processedZone.Height) return;

			var xCount = boundingRect.Width / prop.Width();
			var yCount = boundingRect.Height / prop.Height();

			Console.WriteLine(boundingRect.Width + " " + prop.Width());

			if (boundingRect.Width > boundingRect.Height)
            {
				xCount = Math.Min((boundingRect.Width ) / prop.Width() , random.Next(1,4));
				yCount = 1;
				var oldWidth = boundingRect.Width;
				boundingRect = new Rect(boundingRect.minX, boundingRect.minY, xCount * prop.Width(), boundingRect.Height);
				remainderRect = new Rect(boundingRect.min.X + xCount * prop.Width(), boundingRect.min.Y ,  oldWidth - boundingRect.Width, boundingRect.Height );

			}
			else
            {
				yCount = Math.Min((boundingRect.Width ) / prop.Height()  , random.Next(1, 4));
				xCount = 1;
				var oldHeight = boundingRect.Height;
				boundingRect = new Rect(boundingRect.minX, boundingRect.minY, boundingRect.Width, yCount * prop.Height());
				remainderRect = new Rect(boundingRect.min.X  , boundingRect.min.Y + yCount * prop.Height(), boundingRect.Width  , oldHeight - boundingRect.Height);

			}

			var minX = processedZone.boundingRect.minX;
			var minY = processedZone.boundingRect.minY;

			var dir = new Point(processedZone.dirX, processedZone.dirY);
			 

			var points = new List<Point>();

			for (var i = 0; i < xCount; i++)
			{
				for (var j = 0; j < yCount; j++)
				{
					var p = new Point(i * prop.Width() + processedZone.boundingRect.minX, j * prop.Height() + processedZone.boundingRect.minY);
					if (zonePoints.Contains(p))
                    {

						if (checkWallHuggerCondition(p, new Room(processedZone.getPointsInZone().ToList(),""), prop))
                        {

                        }
						points.Add(p);
					} else
                    {
						return;
                    }
					
				}
			}


			if (points.Count > 0)
			{
				validPropPositions.Add(new PossiblePropPositionsTemplate(prop, points, boundingRect,remainderRect));
			}
		}

		public bool checkWallHuggerCondition(Point point, Room room, IProp prop)
		{
			var edgePoints = new List<Point>();
			for (var i = 0; i < prop.Width(); i++)
			{
				for (var j = 0; j < prop.Height(); j++)
				{
					edgePoints.Add(new Point(i, j) + point + prop.Direction());
				}
			}

			return edgePoints.Intersect(room.edgePoints).Count() > 0;
		}

		public void CenterPropPositions(PossiblePropPositionsTemplate positions, ProcessedZone processedZone)
		{
			var rect = processedZone.boundingRect;
			var remainderX = rect.Width % positions.prop.Width();
			var remainderY = rect.Height % positions.prop.Height();

			if (remainderX > 0)
			{
				if (remainderX % 2 == 1)
				{
					var midPoint = positions.possiblePositions.Count / 2;
					for (var i = midPoint; i < positions.possiblePositions.Count; i++)
					{
						positions.possiblePositions[i].X += remainderX;
					}
				}
				else
				{
					for (var i = 0; i < positions.possiblePositions.Count; i++)
					{
						positions.possiblePositions[i].X += remainderX / 2;
					}
				}
			}

			if (remainderY > 0)
			{
				if (remainderY % 2 == 1)
				{
					var midPoint = positions.possiblePositions.Count / 2;
					for (var i = midPoint; i < positions.possiblePositions.Count; i++)
					{
						positions.possiblePositions[i].Y += remainderY;
					}
				}
				else
				{
					for (var i = 0; i < positions.possiblePositions.Count; i++)
					{
						positions.possiblePositions[i].Y += remainderY / 2;
					}
				}
			}

		}

		public void drawProp(Point point, IProp prop, List<Point> points, List<IProp> placedProps)
		{
 
			var width = prop.Width();
			var height = prop.Height();
			for (var i = 0; i < width ; i++)
			{
				for (var j = 0; j < height ; j++)
				{
					var p2 = new Point(point.X + i, point.Y + j);

					points.Remove(p2);

				}
			}
			placedProps.Add(prop.GetPropAtPosition(point));
		}

		public class PossiblePropPositionsTemplate 
		{
			public Rect boundingRect;
			public Rect remainderRect;
			public IProp prop;
			public List<Point> possiblePositions;

			public PossiblePropPositionsTemplate(IProp prop, List<Point> positions, Rect boundingRect, Rect remainderRect)
			{
				this.prop = prop;
				this.possiblePositions = positions;
				this.boundingRect = boundingRect;
				this.remainderRect = remainderRect;
			}

			public double GetValue(double distributionFactor)
			{
				return prop.Cost() * possiblePositions.Count * distributionFactor;
			}
		}
	}
}
