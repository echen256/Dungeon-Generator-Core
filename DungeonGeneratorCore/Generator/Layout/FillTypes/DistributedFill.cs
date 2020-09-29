using DungeonGeneratorCore.Generator.Layout;
using System;
using System.Collections.Generic;
using DungeonGeneratorCore.Generator.TemplateProcessing;
using DungeonGeneratorCore.Generator.Geometry;
using System.Linq;
using System.Data;
using System.Runtime.InteropServices;

namespace DungeonGeneratorCore.Generator.Layout.FillTypes
{

	using Point = DungeonGeneratorCore.Generator.Geometry.Point;


	public class DistributedFill : IFill
	{
		Random random = new Random();
		public int minimumCount = 1;
		public int maximumCount = 5;
		public int maximumWidth = 10;
		 
		public void DrawProps(PossiblePropPositionsTemplate selectedPropPositions,  List<IProp> placedProps, out ProcessedZone processedZone,  Zone zone)
        {
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
				var rotatedProp = prop.GetRotatedProp(radian); 
				TryDistributedFill(processedZone, rotatedProp, validPropPositions,   zonePoints);
			} 
		}

		public void TryDistributedFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemplate> validPropPositions, List<Point> zonePoints)
		{

			Rect boundingRect = processedZone.boundingRect;
			Rect remainderRect;

			if (prop.Width() > processedZone.Width || prop.Height() > processedZone.Height) return;

			int xCount ;
			int yCount ;

			if (processedZone.fillParameters.fillDirection.X != 0)
			{
				xCount = Math.Min((boundingRect.Width) / prop.Width(), random.Next(minimumCount, maximumCount));
				yCount = 1;

				if (xCount * prop.Width() > maximumWidth)
				{
					xCount = maximumWidth / prop.Width();
				}
				var oldWidth = boundingRect.Width;
				boundingRect = new Rect(boundingRect.minX, boundingRect.minY, xCount * prop.Width(), boundingRect.Height);
				remainderRect = new Rect(boundingRect.min.X + xCount * prop.Width(), boundingRect.min.Y, oldWidth - boundingRect.Width, boundingRect.Height);
			}
			else
			{
				yCount = Math.Min((boundingRect.Height) / prop.Height(), random.Next(minimumCount, maximumCount));
				if (yCount * prop.Height() > maximumWidth)
                {
					yCount = maximumWidth / prop.Height();
                }
				xCount = 1;
				var oldHeight = boundingRect.Height;
				boundingRect = new Rect(boundingRect.minX, boundingRect.minY, boundingRect.Width, yCount * prop.Height());
				remainderRect = new Rect(boundingRect.min.X, boundingRect.min.Y + yCount * prop.Height(), boundingRect.Width, oldHeight - boundingRect.Height);

			}

			var minX = processedZone.boundingRect.minX;
			var minY = processedZone.boundingRect.minY;
			var min = new Point(minX, minY);
			var dir = new Point(1, 1);
 

			var points = new List<Point>();

			for (var i = 0; i < xCount; i++)
			{
				for (var j = 0; j < yCount; j++)
				{
					var p = new Point(dir.X * i * prop.Width(), dir.Y * j * prop.Height()) + min; 
					if (zonePoints.Contains(p) && propIsAligned(processedZone,prop))
                    {
						points.Add(p);		
					} else
                    {
						return;
                    }
					
				}
			}

			if (processedZone.fillParameters.floatType == "right" || processedZone.fillParameters.floatType == "top")
			{
				var center = boundingRect.max - boundingRect.min;
				var centerX = center.X / 2.0 + boundingRect.min.X;
				var centerY = center.Y / 2.0 + boundingRect.min.Y;
				for (var q = 0; q < points.Count; q++)
                {
					points[q] = Rotation.GetRotatedPointAroundPivot(Math.PI,points[q], centerX,centerY);
					points[q].X -= (prop.Width() - 1);
					points[q].Y -= (prop.Height() - 1);
                }
			}

			if (points.Count > 0)
			{
				validPropPositions.Add(new PossiblePropPositionsTemplate(prop, points, boundingRect,remainderRect));
			}
		}

		public bool propIsAligned( ProcessedZone zone, IProp prop)
		
		{
			return new Point(zone.dirX, zone.dirY).Equals(prop.Direction());
		}
 
		
	}
}
