using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using Dungeon_Generator_Core.Geometry;

namespace Dungeon_Generator_Core.Generator
{
    class Room
    {
        public List<Point> entrances;
        public List<Point> points;
        public string category;

        public static List<Point> directions = new Point[] { new Point(1,0) , new Point(0,1), new Point(-1,0), new Point(0,-1)}.ToList();
    
        public Room (Rect rect, string category)
        {
            this.category = category;
            points = new List<Point>();
            entrances = new List<Point>();
            for (var i = 0; i < rect.Width; i++)
            {
                for (var j = 0; j < rect.Height; j++)
                {
                    points.Add(new Point(i + rect.minX, j + rect.minY));
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Room)) return false;
            var e2 = (Room)obj;

            return e2.points.Except(points).Count() == 0;
        }
        public void merge (Room other)
        {
            var count = points.Count;
            points.AddRange(other.points);
            entrances.AddRange(other.entrances);
            Debug.Assert(count < points.Count);
        }

        public void addEntrance (Point entrance)
        {
            if (points.Contains(entrance))
            {
                entrances.Add(entrance);
            }
        }

        public List<Point> getEdgePoints()
        {
            return points.Where((p) => {
                return Point.getNeighbors(p, points).Count != 4;
            }).ToList();
         }
        public List<Point> getInnerPoints()
        {
            return points.Where((p) => {
                return Point.getNeighbors(p, points).Count == 4;
            }).ToList();
        }

        public int area()
        {
            return points.Count;
        }

        public static bool IsNeighbor (Room a, Room b)
        {
            var edgePoints = new List<Point>();
            a.getEdgePoints().ForEach((p) => {
                Directions.directions.ForEach((dir) =>
                {
                    edgePoints.Add(dir + p);
                });
            });
            return edgePoints.Intersect(b.getEdgePoints()).Count() > 0;
        }

        
        public List<Room> getNeighbors (List<Room> rooms)
        {


            return rooms.Where((r) => { return Room.IsNeighbor(this, r); }).ToList();
        }
    
    }
}
