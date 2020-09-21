using Dungeon_Generator_Core.Generator;
using System;
using System.Collections.Generic;
using Dungeon_Generator_Core.TemplateProcessing;
using Dungeon_Generator_Core.Geometry;
using System.Linq;
using System.Data;

namespace Dungeon_Generator_Core.Layout.FillTypes
{

	using Point = Dungeon_Generator_Core.Geometry.Point;
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
			while (zonePoints.Count > 0 && cycles > 0)
            {
				var validPropPositions = new List<PossiblePropPositionsTemp>();
				
				cycles--;
				Console.WriteLine(processedZone.boundingRect);

				var prop = validPropCollections[0];

				SampleAllRotations(processedZone, prop, validPropPositions, zonePoints);

				validPropCollections.Add(prop);
				validPropCollections.RemoveAt(0);

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
		public void SampleAllRotations(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemp> validPropPositions, List<Point> zonePoints)
		{
			for (var angle = 0; angle < 4; angle += 1)
			{
				var radian = Math.PI / 2 * angle;
				TryPartitionFill(processedZone, prop.GetRotatedProp(radian), validPropPositions,   zonePoints);

			}
		}


		public void TryPartitionFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemp> validPropPositions, List<Point> zonePoints)
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
			var yCount  =boundingRect.Height / prop.Height();
			

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

				validPropPositions.Add(new PossiblePropPositionsTemp(prop, points, boundingRect,remainderRect));
			}
		}

		public void CenterPropPositions(PossiblePropPositionsTemp positions, ProcessedZone processedZone)
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
			var offset = 0;
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

		public class PossiblePropPositionsTemp 
		{
			public Rect boundingRect;
			public Rect remainderRect;
			public IProp prop;
			public List<Point> possiblePositions;

			public PossiblePropPositionsTemp(IProp prop, List<Point> positions, Rect boundingRect, Rect remainderRect)
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
