using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DungeonGeneratorCore.Generator.TemplateProcessing;
using DungeonGeneratorCore.Generator.Geometry;

namespace DungeonGeneratorCore.Generator.Layout.FillTypes
{
    class CommonFillMethods
    {
		public static void DrawProps(PossiblePropPositionsTemplate selectedPropPositions, List<IProp> placedProps, out ProcessedZone processedZone, Zone zone)
		{
			var positions = selectedPropPositions.possiblePositions;
			var prop = selectedPropPositions.prop;
			positions.ForEach((point) =>
			{
				placedProps.Add(prop.GetPropAtPosition(point));
			});
			processedZone = new ProcessedZone(selectedPropPositions.remainderRect, zone);
		}
		public static void CenterPropPositions(PossiblePropPositions positions, ProcessedZone processedZone)
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


	}
}
