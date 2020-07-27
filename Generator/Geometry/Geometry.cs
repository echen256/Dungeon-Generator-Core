
using System;
using System.Collections.Generic;
using System.Linq; 

namespace Dungeon_Generator_Core.Geometry
{
 
    public static class Directions
    {
        public static List<Point> directions = new Point[] {new Point(1,0), new Point(0,1), new Point(-1,0), new Point(0,-1) }.ToList();
    }
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return "(" + X + ',' + Y + ')';
        }
        public override int GetHashCode()
        {
            return (int)(X * 10012 + Y * 4312);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Point)) return false;
            var e2 = (Point)obj;
            return e2.X == X && e2.Y == Y;
        }

        public static Point operator+(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator-(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator*(Point a, Point b)
        {
            return new Point(a.X * b.X, a.Y * b.Y);
        }

        public static Point operator /(Point a, Point b)
        {
            return new Point(a.X / b.X, a.Y / b.Y);
        }
        public static List<Point> getNeighbors (Point p, List<Point> points)
        {
            var results = new List<Point>();
            Directions.directions.ForEach((dir) =>
            {
                var p2 = p + dir;
                if (points.Contains(p2))
                {
                    results.Add(p2);
                }
            });
            return results;

        }
    }
    public class Edge 
    {
        public Point P { get; set; }
        public Point Q { get; set; }
        public int Index { get; set; }


        public Edge(Point P, Point Q)
        {
            this.P = P;
            this.Q = Q;
        }
        public int x1
        {
            get
            {
                return (int)P.X;
            }
        }

        public int x2
        {
            get
            {
                return (int)Q.X;
            }
        }
        public int y1
        {
            get
            {
                return (int)P.Y;
            }
        }

        public int y2
        {
            get
            {
                return (int)Q.Y;
            }
        }

        public int weight { get {
               var d = Math.Sqrt(Math.Pow(Q.X - P.X, 2) + Math.Pow(Q.Y - P.Y, 2));
                return (int)d;


            } } 

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Edge)) return false;
            var e2 = (Edge)obj;

            return e2.GetHashCode() == GetHashCode();
        }

        public bool Connected(Edge e2)
        {
            return ! e2.Equals(this) && (e2.P.Equals( P) || e2.Q.Equals(P) || e2.P.Equals(Q) || e2.Q.Equals( Q));
        }

        public static List<Edge> getConnected (Edge e, List<Edge> edges)
        {
            return edges.Where((e2) =>
            {
                return e.Connected(e2);
            }).ToList();
        }

        public override string ToString()
        {
            return "[" + P.ToString() + "," + Q.ToString() + "]";
        }

        public List<Point> getPoints()
        {
            var x0 = this.P.X;
            var y0 = this.P.Y;
            var x1 = this.Q.X;
            var y1 = this.Q.Y;

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;
            var output = new List<Point>();
            while (true)
            {
                output.Add(new Point(x0, y0));
                if ((x0 == x1) && (y0 == y1))
                {
                    break;
                }
                var e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
            return output;
        }

        public Point other(Point point)
        {
            if (point.Equals(P) && !point.Equals(Q))
            {
                return Q;
            }
            else
            {
                return P;
            }


        }
        public override int GetHashCode()
        {
            return P.GetHashCode() + Q.GetHashCode();
        }
    }

    public class Rect
    {

        public int minX
        {
            get
            {
                return min.X;
            }
        }
        public int minY
        {
            get
            {
                return min.Y;
            }
        }

        public int maxX
        {
            get
            {
                return max.X;
            }
        }

        public int maxY
        {
            get
            {
                return max.Y ;
            }
        }
        public Point min { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Point max { get
            {
                return new Point(min.X + Width - 1, min.Y + Height - 1);
            }
            }

        public Rect (int x, int y, int w, int h) : this(new Point(x,y), w,h)
        {

        }
        public Rect(Point p, int w, int h)
        {
            this.min = p;
            this.Width = w;
            this.Height = h;
        }

        public static List<Rect> neighbors (Rect r, List<Rect> others)
        {
            return others.Where((rect) =>
            {
                if (r.min.X == rect.min.X + rect.Width & r.min.Y == rect.min.Y) return true;
                if (r.min.X == rect.min.X & r.min.Y == rect.min.Y + rect.Height) return true;
                if (r.min.X + r.Width == rect.min.X & r.min.Y == rect.min.Y) return true;
                if (r.min.X == rect.min.X & r.min.Y + r.Height == rect.min.Y) return true;
                return false;
            }).ToList();
        }
        public override int GetHashCode()
        {
            return Width * 10023 + Height * 3423 + min.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Rect)) return false;
            var e2 = (Rect  )obj;
            return base.Equals(e2);
        }
        public bool Equals(Rect other)
        {
            return other.min == this.min && other.Width == this.Width && other.Height == this.Height;
        }

        public override string ToString()
        {
            return "[" + min.ToString() + ", w:" + Width + ", h: " + Height + "]";
        }
    }

}
