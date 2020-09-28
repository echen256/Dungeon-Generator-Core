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


	public class DistributedFill
	{
		Random random = new Random();
		public int minimumCount = 1;
		public int maximumCount = 5;
		public int maximumWidth = 10;
		public void execute(Room room, Zone zone, IPropCollection propCollection, List<IProp> placedProps)
		{

			var processedZone = new ProcessedZone(room, zone);
			var zonePoints = processedZone.getPointsInZone().ToList();

			var validPropList = propCollection.getPropList();
			validPropList = validPropList.FindAll((prop) => { return processedZone.tags.Contains(prop.Identifier()); });

			var validPropListMemory = new List<IProp>();
			int cycles = 40;
			 
			while (cycles > 0)
            {

				cycles--;
				if (processedZone.boundingRect.Area <= 0)
				{
					break;
				}

				var validPropPositions = new List<PossiblePropPositionsTemplate>();

				if (validPropList.Count == 0)
                {
					
					validPropList = new List<IProp>(validPropListMemory);
					validPropList = validPropList.OrderByDescending((item) => { return random.NextDouble(); } ).ToList(); ;
                }
				var selectedIndex = random.Next(0, validPropList.Count);
				var prop = validPropList[selectedIndex];
				validPropList.RemoveAt(selectedIndex);


				validPropListMemory.Add(prop);
				
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

			if (boundingRect.Width > boundingRect.Height)
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
					if (zonePoints.Contains(p) && checkWallHuggerCondition(p,processedZone,prop))
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

		public bool checkWallHuggerCondition(Point point, ProcessedZone zone, IProp prop)
		
		{
			return new Point(zone.dirX, zone.dirY).Equals(prop.Direction());
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
