﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Text;
using DungeonGeneratorCore.Generator.Geometry;

namespace DungeonGeneratorCore.Generator.Layout
{

	public interface IProp
	{

		int MaximumClusterCount();
		int Cost();
		string Identifier();

		string Color { get; set; }
		bool WallHugger();
		int Width();
		int Height();

		Point Direction();

		IProp GetRotatedProp(double radian);

		Point Position();

		IProp GetPropAtPosition(Point p);
	}

	public class Prop : IProp
	{
		public int cost;
		public string Color { get; set; }
		public bool wallHugger;
		public int width;
		public int height;
		int maximumClusterCount;
		string identifier;
		Point direction;
		Point position;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
		}

        public IProp GetRotatedProp(double radian)
		{
			var newSize = Rotation.GetRotatedPointAroundPivot(radian, new Point(Width(), Height()),0,0);
			var rotatedDirection = Rotation.GetRotatedPointAroundPivot(radian, direction,0,0);
			return new Prop((int)Math.Abs(newSize.X), (int)Math.Abs(newSize.Y), Cost(), Color, WallHugger(), rotatedDirection, position, maximumClusterCount, Identifier());
		}

		public int Cost()
		{
			return cost;
		}
		public string Identifier()
		{
			return identifier;
		}
		public bool WallHugger()
		{
			return wallHugger;
		}
		public int Width() { return width; }
		public int Height() { return height; }

		public Point Direction()
		{
			return direction;
		}

		public Point Position()
		{
			return position;
		}

		public int MaximumClusterCount()
		{
			return maximumClusterCount;
		}

		public IProp GetPropAtPosition(Point p)
		{
			return new Prop(width, height, cost, Color, wallHugger, direction, p, maximumClusterCount, Identifier());
		}


		public Prop(int width, int height, int cost, string color, bool wallHugger, Point direction, Point position, int maximumClusterCount, string identifier)
		{
			this.width = width;
			this.height = height;
			this.cost = cost;
			this.Color = color;
			this.wallHugger = wallHugger;
			this.direction = direction;
			this.identifier = identifier;
			this.position = position;
			this.maximumClusterCount = maximumClusterCount;

		}
	}

	public class PossiblePropPositions
	{
		public IProp prop;
		public List<Point> possiblePositions;

		public PossiblePropPositions(IProp prop, List<Point> positions)
		{
			this.prop = prop;
			this.possiblePositions = positions;
		}

		public double GetValue(double distributionFactor)
		{
			return prop.Cost() * possiblePositions.Count * distributionFactor;
		}


	}

	public class PossiblePropPositionsTemplate
	{
		public Rect boundingRect;
		public IProp prop;
		public List<Point> possiblePositions;

		public PossiblePropPositionsTemplate(IProp prop, List<Point> positions)
		{
			this.prop = prop;
			this.possiblePositions = positions;
		}

		public double GetValue(double distributionFactor)
		{
			return prop.Cost() * possiblePositions.Count * distributionFactor;
		}
	}
}
