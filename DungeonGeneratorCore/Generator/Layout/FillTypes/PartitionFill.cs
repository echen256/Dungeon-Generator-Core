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
	public class PartitionFill : IFill
	{
		Random random = new Random();

		public void DrawProps(PossiblePropPositionsTemplate selectedPropPositions, List<IProp> placedProps, out ProcessedZone processedZone, Zone zone) {
			CommonFillMethods.DrawProps(selectedPropPositions, placedProps, out processedZone, zone);
		}
		public PossiblePropPositionsTemplate ChooseSolution(List<PossiblePropPositionsTemplate> validPropPositions, ProcessedZone zone)
		{

			validPropPositions.OrderBy((col) => { return zone.boundingRect.Area - col.prop.Width() * col.prop.Height() * col.possiblePositions.Count(); });
			return validPropPositions[0];
		}

		public void TryFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemplate> validPropPositions, List<Point> zonePoints)
		{
			for (var angle = 0; angle < 4; angle += 1)
			{
				var radian = Math.PI / 2 * angle;
				TryPartitionFill(processedZone, prop.GetRotatedProp(radian), validPropPositions,   zonePoints);

			}
		}
		public void TryPartitionFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemplate> validPropPositions, List<Point> zonePoints)
		{

			Rect boundingRect = processedZone.boundingRect;
			Rect remainderRect;
			if (boundingRect.Width > boundingRect.Height)
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
 

		 
	}
}
