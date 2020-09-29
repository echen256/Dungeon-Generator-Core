﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DungeonGeneratorCore.Generator.Geometry;
using DungeonGeneratorCore.Generator.Layout;
using DungeonGeneratorCore.Generator;
using org.mariuszgromada.math.mxparser;

namespace DungeonGeneratorCore.Generator.TemplateProcessing
{
    [Serializable]
    public class Zone
    {
        public string width;
        public string height;
        public string x;
        public string y;
        public List<string> tags;
        public int dirX;
        public int dirY;
        public FillParameters fillParameters;
    }

    public class ProcessedZone
    {
        public int Width
        {
            get
            {
                return boundingRect.Width;
            }
        }

     
        public int Height
        {
            get
            {
                return boundingRect.Height;
            }
        }
        public int x
        {
            get
            {
                return boundingRect.minX;
            }
        }
        public int y
        {
            get
            {
                return boundingRect.minY;
            }
        }


        public string[] expressions = new string[] { "+", "-", "/", "*" };
        public List<string> tags;
        public int dirX;
        public int dirY;
        public Rect boundingRect;
        public FillParameters fillParameters;
        public ProcessedZone(Room room, Zone zone)  
        {

            boundingRect = room.getBoundingRectangle();
            tags = new List<string>(zone.tags);
            dirX = zone.dirX;
            dirY = zone.dirY; 
            boundingRect = new Rect(parseStartingPosition(boundingRect, zone.x, "x"), parseStartingPosition(boundingRect, zone.y, "y"), parseExpression(boundingRect, zone.width), parseExpression(boundingRect, zone.height));
            fillParameters = zone.fillParameters;
        }

        public ProcessedZone (List<Point> points,   Zone zone) : this (Room.GetBoundingRectangle(points), zone)
        {

        }
 
        public ProcessedZone(Rect rect, Zone zone)
        {
            var boundingRect = rect; 
            tags = new List<string>(zone.tags);
            dirX = zone.dirX;
            dirY = zone.dirY; 
            this.boundingRect = boundingRect;
            fillParameters = zone.fillParameters;
        }

        public Point[] getPointsInZone ( )
        {
            var output = new List<Point>();
 
           for (var i = 0; i < boundingRect.Width; i++)
            {
                for (var j = 0; j < boundingRect.Height; j++){
                    output.Add(new Point(i + boundingRect.minX, j + boundingRect.minY)); 
                }
            }

            return output.ToArray();
        }

        public int parseStartingPosition (Rect rect, string expression, string fieldName)
        {
            if (fieldName == "x")
            {
                return parseExpression(rect, expression) + rect.minX;
            } else
            {
                return parseExpression(rect, expression) + rect.minY;
            }
        }
        public int parseExpression (Rect rect,string expression)
        { 
            Constant w = new Constant("w", rect.Width);
            Constant h = new Constant("h", rect.Height);

            Expression e = new Expression(expression, new PrimitiveElement[] { w,h});

            return (int)Math.Round(e.calculate());
         
 
        }
    }

 
}
 