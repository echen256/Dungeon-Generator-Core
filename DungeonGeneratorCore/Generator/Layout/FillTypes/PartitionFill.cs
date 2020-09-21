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

			var validPropCollections = propCollection.getPropList();
			validPropCollections = validPropCollections.FindAll((prop) => { return processedZone.tags.Contains(prop.Identifier()); });
			
			var zonePoints = processedZone.getPointsInZone().ToList();


			int cycles = 20;
			while (zonePoints.Count > 0 && cycles > 0)
            {
				var validPropPositions = new List<PossiblePropPositionsTemp>();
				
				cycles--;
				Console.WriteLine(processedZone.boundingRect);
				validPropCollections.ForEach((prop) =>
				{
					SampleAllRotations(processedZone, prop, validPropPositions, zonePoints);
				});

				if (validPropPositions.Count > 0)
				{
					validPropPositions.OrderBy((col) => { return zonePoints.Count - col.prop.Width() * col.prop.Height() * col.possiblePositions.Count(); });
					//var selectedPosition = validPropPositions[random.Next(0,  validPropPositions.Count )]; 
					var selectedPosition = validPropPositions[0];
					//CenterPropPositions(selectedPosition, processedZone);
					var positions = selectedPosition.possiblePositions;
					positions.ForEach((point) =>
					{ 
						drawProp(point, selectedPosition.prop, zonePoints, placedProps);
					});


					var newPoints = new List<Point>();
					zonePoints.ForEach((p) => { 
						if (! selectedPosition.boundingRect.Contains(p))
                        {
							newPoints.Add(p);
                        }
					});

					processedZone = new ProcessedZone(newPoints,zone );

				


				}
				
			}

			
		}
		public void SampleAllRotations(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemp> validPropPositions, List<Point> zonePoints)
		{
			for (var angle = 0; angle < 1; angle += 1)
			{
				var radian = Math.PI / 2 * angle;
				TryPartitionFill(processedZone, prop.GetRotatedProp(0), validPropPositions,   zonePoints);

			}
		}


		public void TryPartitionFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemp> validPropPositions, List<Point> zonePoints)
		{
			var xCount = processedZone.width / prop.Width();
			var yCount = processedZone.height / prop.Height();
			Rect boundingRect;

			if (random.NextDouble() < .5)
            {
				xCount = Math.Min(1, xCount);
				boundingRect = new Rect(processedZone.x, processedZone.y, prop.Width(), processedZone.height);
            } else
            {
				yCount = Math.Min(1, yCount);
				boundingRect = new Rect(processedZone.x, processedZone.y, processedZone.width, prop.Height());
			}

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

				validPropPositions.Add(new PossiblePropPositionsTemp(prop, points, boundingRect));
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
			public IProp prop;
			public List<Point> possiblePositions;

			public PossiblePropPositionsTemp(IProp prop, List<Point> positions, Rect boundingRect)
			{
				this.prop = prop;
				this.possiblePositions = positions;
				this.boundingRect = boundingRect;
			}

			public double GetValue(double distributionFactor)
			{
				return prop.Cost() * possiblePositions.Count * distributionFactor;
			}
		}
	}
}
