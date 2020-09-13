using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeon_Generator_Core.Geometry;
using Dungeon_Generator_Core.Layout;
using Dungeon_Generator_Core.Generator;

namespace Dungeon_Generator_Core.TemplateProcessing
{
    public class Zone
    {
        public string width;
        public string height;
        public string x;
        public string y;
        public List<string> tags;
        public int dirX;
        public int dirY;
        public string fillType;
    }

    public class ProcessedZone
    {
        public int width;
        public int height;
        public int x;
        public int y;
        public List<string> tags;
        public int dirX;
        public int dirY;
        public Rect boundingRect;
        public Room room;
        public string fillType;
        public ProcessedZone(Room room, Zone zone)
        {
            var boundingRect = room.getBoundingRectangle();
            width = parseExpression(boundingRect, zone.width);
            height = parseExpression(boundingRect, zone.height);
            x = parseStartingPosition(boundingRect, zone.x,"x");
            y = parseStartingPosition(boundingRect, zone.y,"y");
            tags = new List<string>(zone.tags);
            dirX = zone.dirX;
            dirY = zone.dirY;
            this.room = room;
            this.boundingRect = new Rect(x, y, width, height);
            this.fillType = zone.fillType;
        }

        public Point[] getPointsInZone ( )
        {
            var output = new List<Point>();
            for (var i = 0; i < room.points.Length; i++)
            {
                if (boundingRect.Contains(room.points[i]))
                {
                    output.Add(room.points[i]);
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
            var result = 0;

            if (expression.Equals("w"))
            {
                return rect.Width;
            } else if (expression.Equals("h"))
            {
                return rect.Height;
            } else if (int.TryParse(expression, out result))
            {
                return result;
            } else if (expression.Contains("-")) {
                var values = expression.Split('-');
                if (expression.Contains("w"))
                {
                    return rect.Width - int.Parse(values[1]);
                } else
                {
                    return rect.Height - int.Parse(values[1]);
                }

            } else
            {
                return 0;
            }
 
        }
    }

 
}
 