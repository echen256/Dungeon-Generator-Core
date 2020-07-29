using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using Dungeon_Generator_Core.Geometry;

namespace Dungeon_Generator_Core.Generator
{
    class Room
    {
        public List<Point> entrances;
        public Point[] points;
        public Point[] edgePoints;
        public Point[] innerPoints;
        public string category;

        public static List<Point> directions = new Point[] { new Point(1,0) , new Point(0,1), new Point(-1,0), new Point(0,-1)}.ToList();
    
        public Room (Rect rect, string category)
        {
            this.category = category;
            entrances = new List<Point>();

            var list = new List<Point>();
            for (var i = 0; i < rect.Width; i++)
            {
                for (var j = 0; j < rect.Height; j++)
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
                if (Point.getNeighbors(p, points).Count != 4)
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
            var list = points.ToList();
            list.AddRange(other.points.ToList());
            populateInternal(list);
           
        }

        public void addEntrance (Point entrance)
        {
            if (points.Contains(entrance))
            {
                entrances.Add(entrance);
            }
        }

        public int area()
        {
            return points.Length;
        }

        public static bool IsNeighbor (Room a, Room b)
        {
            if (a.Equals(b)) return false;
            var isNeighbor = false;
            var points = a.edgePoints;
            var edgePoints = new HashSet<Point>();
           
            var otherPoints = new HashSet<Point>(b.edgePoints);

            for (var i = 0; i < points.Length; i++)
            {
                Directions.directions.ForEach((dir) =>
                {
                    edgePoints.Add(dir + points[i]);
                });
            }
       
            return edgePoints.Intersect(otherPoints).Count() > 0;
        }

        
        public List<Room> getNeighbors (List<Room> rooms)
        {


            return rooms.Where((r) => { return Room.IsNeighbor(this, r); }).ToList();
        }
    
    }
}
