using System;
using System.Collections.Generic; 
using System.Linq; 
using Dungeon_Generator_Core.Geometry;
using Dungeon_Generator_Core.Layout;
using NetTopologySuite.Algorithm.Distance;

namespace Dungeon_Generator_Core.Generator
{
    public class Room
    {
        public List<Point> entrances;
        public Point[] points;
        public Point[] edgePoints;
        public Point[] innerPoints;
        public string category;

        public static List<Point> directions = new Point[] { new Point(1,0) , new Point(0,1), new Point(-1,0), new Point(0,-1)}.ToList();

        public List<IProp> props;

        public Room (List<Point> points, string category)
        {
            entrances = new List<Point>();
            this.category = category;
            populateInternal(points);
        }
        public Room (Rect rect, string category)
        {
            this.category = category;
            entrances = new List<Point>();

            var list = new List<Point>();
            for (var i = 0; i <= rect.Width; i++)
            {
                for (var j = 0; j <= rect.Height; j++)
                {
                    list.Add(new Point(i + rect.minX, j + rect.minY));
                }
            }
            populateInternal(list);
        }

        void populateInternal(List<Point> list)
        {
            
            points = list.ToArray();
            var ep = new List<Point>();
            var ip = new List<Point>();
            for (var i = 0; i < points.Length; i++)
            {
                var p = points[i];
                if (Point.getFullNeighbors(p,points).Count() != 8)
                {
                    ep.Add(p);
                }
                else
                {
                    ip.Add(p);
                }
            }
            edgePoints = ep.ToArray();
            innerPoints = ip.ToArray();
        }
        public override int GetHashCode()
        {
            var code = 0;
            for (var i = 0; i < points.Length; i++)
            {
                code += points[i].GetHashCode();
            } 
            return code;
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Room)) return false;
            var e2 = (Room)obj;

            return e2.points.Intersect(points).Count() == points.Count();
        }

    
        public void merge (Room other)
        {
            entrances.AddRange(other.entrances);
            var list = new HashSet<Point>(points );
            list.UnionWith(other.points.ToList());
            populateInternal(list.ToList());
           
        }

        public List<Point> getUsableInnerPoints()
        {
            var points = innerPoints.ToList();
            entrances.ForEach((entrance) => {
                Directions.directions.ForEach((dir) => {
                    var e2 = dir + entrance;
                    if (points.Contains(e2))
                    {
                        points.Remove(e2);
                    }
                });
            });
            return points;
        }
        public void addEntrance (Point entrance)
        {
          //  var inner = innerPoints.ToList();
            if (points.Contains(entrance))
            {
                entrances.Add(entrance);
               
            }
          //  innerPoints = inner.ToArray();
        }

        public int area()
        {
            return points.Length;
        }

        public static bool IsNeighbor (Room a, Room b)
        {
            if (a.Equals(b)) return false;
            var edgePoints = new HashSet<Point>(a.edgePoints);  
            var otherPoints = new HashSet<Point>(b.edgePoints);
            return edgePoints.Intersect(otherPoints).Count() > 0;
        }

        
        public List<Room> getNeighbors (List<Room> rooms)
        {


            return rooms.Where((r) => { return Room.IsNeighbor(this, r); }).ToList();
        }
    
    }
}
