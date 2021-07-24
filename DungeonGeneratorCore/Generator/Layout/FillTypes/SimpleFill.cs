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
	public class SimpleFill : IFill
	{
		Random random = new Random();


		public void DrawProps(PossiblePropPositionsTemplate selectedPropPositions, List<IProp> placedProps, out ProcessedZone processedZone, Zone zone)
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
			 
					TryGridFill(processedZone, prop.GetRotatedProp(radian), validPropPositions );
				 
				

			}
		}

		public void TryGridFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemplate> validPropPositions )
		{
			var offsetX = processedZone.fillParameters.fillOffset.X;
			var offsetY = processedZone.fillParameters.fillOffset.Y;
			var xCount = processedZone.Width / (prop.Width() + offsetX);
			var yCount = processedZone.Height / (prop.Height() + offsetY);
			var min = processedZone.boundingRect.min;
			if (processedZone.fillParameters.floatType == "right")
			{
				min.X += processedZone.Width % (xCount * prop.Width());
			}
			else if (processedZone.fillParameters.floatType == "top")
			{
				min.Y += processedZone.Height % (yCount * prop.Height());
			}

			var points = new List<Point>();
			for (var i = 0; i < xCount; i++)
			{
				for (var j = 0; j < yCount; j++)
				{
					points.Add(
						new Point(
						i *( prop.Width() + offsetX) + min.X  
						, 
						j *( prop.Height() + offsetY) + min.Y
						)
					);
				}
			}

			 
			if (points.Count > 0)
			{

				validPropPositions.Add(new PossiblePropPositionsTemplate(prop, points, processedZone.boundingRect, new Rect(0,0,0,0)));
			}
		}

	}
}
