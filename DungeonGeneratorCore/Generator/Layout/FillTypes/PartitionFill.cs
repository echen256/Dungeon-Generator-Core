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
	public class PartitionFill
	{
		Random random = new Random();

		public void execute(Room room, Zone zone, IPropCollection propCollection, List<IProp> placedProps)
		{

			var processedZone = new ProcessedZone(room, zone);
			var zonePoints = processedZone.getPointsInZone().ToList();

			var validPropCollections = propCollection.getPropList();
			validPropCollections = validPropCollections.FindAll((prop) => { return processedZone.tags.Contains(prop.Identifier()); });

			int cycles = 20;
			 
			while (cycles > 0 && validPropCollections.Count > 0)
            {

				cycles--;
				Console.WriteLine(validPropCollections.Count);
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
					//var selectedPosition = validPropPositions[random.Next(0,  validPropPositions.Count )]; 
					var selectedPosition = validPropPositions[0];
					//CenterPropPositions(selectedPosition, processedZone);
	
					var positions = selectedPosition.possiblePositions;
					var w = selectedPosition.boundingRect.Width;
					var h = selectedPosition.boundingRect.Height;
					var x0 = selectedPosition.boundingRect.minX;
					var y0 = selectedPosition.boundingRect.minY;

					//drawProp(positions[0],new Prop(w,h,1,"green",false, new Point(1,0),new Point(0,0),1,"test"), zonePoints, placedProps);
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
			for (var angle = 0; angle < 4; angle += 1)
			{
				var radian = Math.PI / 2 * angle;
				TryPartitionFill(processedZone, prop.GetRotatedProp(radian), validPropPositions,   zonePoints);

			}
		}


		public void TryPartitionFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemplate> validPropPositions, List<Point> zonePoints)
		{

			Rect boundingRect;
			Rect remainderRect;
			if (random.NextDouble() < .5)
            {
				boundingRect = new Rect(processedZone.x, processedZone.y, Math.Min(processedZone.Width, prop.Width() + 1)  , processedZone.Height);
				remainderRect = new Rect(processedZone.x + boundingRect.Width, processedZone.y, processedZone.Width - boundingRect.Width, processedZone.Height);
            } else
            {
				boundingRect = new Rect(processedZone.x, processedZone.y,  processedZone.Width, Math.Min(processedZone.Height,prop.Height() + 1));
				remainderRect = new Rect(processedZone.x , processedZone.y + boundingRect.Height, processedZone.Width  , processedZone.Height - boundingRect.Height);
			}

			var xCount = boundingRect.Width / prop.Width();
			var yCount = boundingRect.Height / prop.Height();
			

			var points = new List<Point>();

			for (var i = 0; i < xCount; i++)
			{
				for (var j = 0; j < yCount; j++)
				{
					var p = new Point(i * prop.Width() + processedZone.boundingRect.minX, j * prop.Height() + processedZone.boundingRect.minY);
					if (zonePoints.Contains(p))
                    {
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
