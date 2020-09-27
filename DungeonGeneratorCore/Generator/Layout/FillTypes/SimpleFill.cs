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
	public class SimpleFill
	{
		Random random = new Random();

		public void execute(Room room, Zone zone, IPropCollection propCollection, List<IProp> placedProps)
		{

			var processedZone = new ProcessedZone(room, zone);
			var validPropCollections = propCollection.getPropList();
			validPropCollections = validPropCollections.FindAll((prop) => { return processedZone.tags.Contains(prop.Identifier()); });
			var validPropPositions = new List<PossiblePropPositions>();
			var zonePoints = processedZone.getPointsInZone().ToList();

			validPropCollections.ForEach((prop) =>
			{
				SampleAllRotations(processedZone, prop, validPropPositions);
			});

			if (validPropPositions.Count > 0)
			{

				validPropPositions.OrderBy((col) => { return zonePoints.Count - col.prop.Width() * col.prop.Height() * col.possiblePositions.Count(); });
				var selectedPosition = validPropPositions[0];
				//CenterPropPositions(selectedPosition, processedZone);
				var positions = selectedPosition.possiblePositions;
				positions.ForEach((point) =>
				{
					drawProp(point, selectedPosition.prop, zonePoints, placedProps);
				});
			}
		}
		public void SampleAllRotations(ProcessedZone processedZone, IProp prop, List<PossiblePropPositions> validPropPositions)
		{
			for (var angle = 0; angle < 4; angle += 1)
			{
				var radian = Math.PI / 2 * angle;
			 
					TryGridFill(processedZone, prop.GetRotatedProp(radian), validPropPositions );
				 
				

			}
		}

		
		public void TryGridFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositions> validPropPositions )
		{
			var xCount = processedZone.Width / (prop.Width());
			var yCount = processedZone.Height / prop.Height();

			var points = new List<Point>();
			for (var i = 0; i < xCount; i++)
			{
				for (var j = 0; j < yCount; j++)
				{
					points.Add(
						new Point(
						i * prop.Width() + processedZone.boundingRect.minX  
						, 
						j * prop.Height() + processedZone.boundingRect.minY 
						)
					);
				}
			}
			if (points.Count > 0)
			{

				validPropPositions.Add(new PossiblePropPositions(prop, points));
			}
		}

		public void CenterPropPositions(PossiblePropPositions positions, ProcessedZone processedZone)
		{
			var rect = processedZone.boundingRect;
			var remainderX = rect.Width % positions.prop.Width();
			var remainderY = rect.Width % positions.prop.Height();

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
			var offset = 1;
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
	}
}
