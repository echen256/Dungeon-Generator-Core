using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGeneratorCore.Generator.Layout.FillTypes
{

	using Point = DungeonGeneratorCore.Generator.Geometry.Point;
	using Rect = DungeonGeneratorCore.Generator.Geometry.Rect;
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
