
using System;
using System.Collections.Generic;
using Dungeon_Generator_Core.Geometry;

namespace Dungeon_Generator_Core.Generator
{
    public class Prims
    {
        public List<Edge> results;
        public List<Point> newPoints;
        public List<Point> points;
        public List<Edge> edges;
        public void init(List<Edge> edges)
        {
            this.results = new List<Edge>();
            this.edges = edges;
            this.newPoints = new List<Point>();
            this.points = new List<Point>();

            edges.ForEach((edge) =>
            {
                this.points.Add(edge.P);
                this.points.Add(edge.Q);
            });
            this.points = new List<Point>(new HashSet<Point>(points));
            var point = points[0];
            newPoints.Add(point);
        }

        public Edge select(List<Point> newPoints, List<Edge> edges)
        {
            var filteredEdges = edges.FindAll((edge) =>
            {
                var b1 = newPoints.Contains(edge.P) ? 1 : 0;
                var b2 = newPoints.Contains(edge.Q) ? 1 : 0;
                return b1 + b2 == 1;
            });

            if (filteredEdges.Count == 0) return null;

            filteredEdges.Sort((a, b) =>
            {
                if (a.weight > b.weight) return 1;
                if (a.weight < b.weight) return -1;
                return 0;
            });

            var result = filteredEdges[0];
            edges.Remove(result);
            return result;
        }

        public List<Edge> iterate()
        {
            if (newPoints.Count >= points.Count)
            {
                return new List<Edge>();
            }
            var edge = select(newPoints, edges);
            if (edge == null)
            {
                return new List<Edge>();
            }

            results.Add(edge);
            if (! newPoints.Contains(edge.P))
            {
                newPoints.Add(edge.P);
            }
            if (!newPoints.Contains(edge.Q))
            {
                newPoints.Add(edge.Q);
            }
            return results;
        }

        public List<Edge> exec (List<Edge> edges)
        {
            init(edges);
            while (newPoints.Count < points.Count)
            {
                var edge = select(newPoints, edges);
                if (edge == null)
                {
                    Console.WriteLine("error");
                    break;
                }
                results.Add(edge);
                if (!newPoints.Contains(edge.P))
                {
                    newPoints.Add(edge.P);
                }
                if (!newPoints.Contains(edge.Q))
                {
                    newPoints.Add(edge.Q);
                }


            }
            return results;
        }
    }
}
