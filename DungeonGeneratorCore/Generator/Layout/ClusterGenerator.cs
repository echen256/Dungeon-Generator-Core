using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon_Generator_Core.Geometry;

namespace Dungeon_Generator_Core.Generator
{
    public class ClusterGenerator
    {

        System.Random random = new System.Random();
        public readonly double defaultSpreadPossibility = .95;
        public readonly double defaultReductionFactor = .5;

        public List<Rect> testCase()
        {
            return new Rect[] {new Rect(new Point(0,0),1,1) , new Rect(new Point(1, 0), 1, 1) , new Rect(new Point(0, 1), 1, 1) , new Rect(new Point(1,1), 1, 1), new Rect(new Point(2, 1), 1, 1), new Rect(new Point(2, 0), 1, 1) }.ToList();
        }

        public List<Rect> execute(int x, int y)
        {
            return execute(x, y, defaultSpreadPossibility, defaultReductionFactor);
        }

        public List<Rect> execute(int x, int y, double spreadProbability  )
        {
            return execute(x, y, spreadProbability, defaultReductionFactor);
        }

        public List<Rect> execute(int x, int y, double spreadProbability, double reductionFactor)
        {
            List<Rect> rects = new List<Rect>();
            recursiveGenerate(x, y, spreadProbability, reductionFactor, rects);
            return rects;
        }

        public List<Rect> execute(  double spreadProbability, double reductionFactor, int solutionSpaceWidth, int solutionSpaceHeight, int width)
        {
            List<Rect> rects = new List<Rect>();
            for (var i = 0; i < solutionSpaceWidth; i++)
            {
                for (var j = 0; j < solutionSpaceHeight; j++)
                {
                    rects.Add(new Rect(new Point(i * width, j * width), width, width));
                }
            }


            return execute(spreadProbability, reductionFactor, rects);
        }

        public static Dictionary<int, List<int>> generateNeighborMapFromGrid(int solutionSpaceWidth, int solutionSpaceHeight)
        {
            Dictionary<int, List<int>> neighborMap = new Dictionary<int, List<int>>();
            var max = solutionSpaceHeight * solutionSpaceWidth;

            for (var i = 0; i < max; i++)
            {

                var neighbors = new List<int>();
                for (var j = -1; j < 2; j++)
                {
                    var offsets = new int[]{
                            j + i - solutionSpaceWidth, j + i ,  j + i + solutionSpaceWidth
                    };

                    for (var k = 0; k < offsets.Length; k++)
                    {
                        var index = offsets[k];
                        if (index > -1 & index < max )
                        {
                            neighbors.Add(index);
                        }
                    }
                }
                neighborMap[i] = neighbors;
            }
            return neighborMap;
        }



        public static List<int>  getNeighbors (Rect r,  List<Rect> rects)
        {
           
            var neighbors = new List<int>();
            for (var j = 0; j < rects.Count; j++)
            {
                var r2 = rects[j];
                if (r.Equals(r2)) continue;

           
                if (RectanglesTouch(r, r2))
                {
                    neighbors.Add(j);
                }


            }
            return neighbors;
        }
        public static UndirectedGraph<Rect> generateNeigbhorMapFromArbitraryRects (List<Rect> rects)
        {
            Dictionary<int, List<int>> neighborMap = new Dictionary<int, List<int>>();

            PositionalGrid grid = new PositionalGrid(rects);
            UndirectedGraph<Rect> graph = new UndirectedGraph<Rect>(rects);

              for (var i = 0; i < rects.Count; i++)
            {
                var r1 = rects[i];

               var neighbors = new List<int>();
                neighbors.AddRange(grid.getX(r1.minX));
                neighbors.AddRange(grid.getX(r1.maxX));

                neighbors.AddRange(grid.getY(r1.minY));
                neighbors.AddRange(grid.getY(r1.maxY));

                neighbors = neighbors.FindAll((index) =>
                {
                    var r2 = rects[index];
                    return RectanglesTouch(r1, r2);
                });

                graph.AddNode(i, neighbors);



              // graph.AddNode(i,  getNeighbors); 
            }


            return graph;
        }


        public static bool RectanglesTouch(Rect a, Rect b)
        {

            var d = a.max.X + 1 == b.min.X;
            var e = b.max.X + 1 == a.min.X;

            var f = a.max.Y + 1 == b.min.Y;
            var g = b.max.Y + 1 == a.min.Y;


            var h = b.max.X >= a.max.X & b.min.X <= a.max.X;
            var i = b.max.Y >= a.max.Y & b.min.Y <= a.max.Y;

  
            return ((d | e) && i) || ((f | g) && h);

            

        }
        public List<Rect> execute (  double spreadProbability, double reductionFactor, List<Rect> options)
        {
            List<Rect> rects = new List<Rect>();
            var neighbors = generateNeigbhorMapFromArbitraryRects(options);
            var start = new Random().Next(options.Count / 4, 3 * options.Count / 4);
            recursiveGenerate2(start, spreadProbability, reductionFactor, options, rects, neighbors);
           // Smooth(rects, options, 10);
            return rects;
        }

        void recursiveGenerate2(int startIndex,  double probability, double reduction, List<Rect> options, List<Rect> rects, UndirectedGraph<Rect> neighbors)
        {
            if (startIndex >= options.Count) return;
            if (rects.Contains(options[startIndex])) return;
            rects.Add(options[startIndex]);
            neighbors.RemoveNode(startIndex);
            var localNeighbors = neighbors[startIndex];


            for (var i = 0; i < localNeighbors.Count; i++)
            {

                if (random.NextDouble() < probability)
                {
                    recursiveGenerate2(localNeighbors[i], probability * reduction * reduction, reduction, options, rects, neighbors);
                }
            } 
        }

        void recursiveGenerate (int x, int y , double probability, double reduction, List<Rect> rects)
        {
            var rect = new Rect(x, y, 1, 1);
            if (rects.Contains(rect))
            {
                return;
            }
            rects.Add(rect);
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    if (Math.Abs(i) + Math.Abs(j) == 1)
                    {
                        if (random.NextDouble() < probability)
                        {

                            recursiveGenerate(x + i, j + y, probability * reduction, reduction, rects);
                        }
                    }
                }
            }
        }

        public void Smooth (List<Rect> cluster, List<Rect> options, int cycles)
        {
            var excludedRects = options.Except(cluster).ToList();
            var graph1 = new UndirectedGraph<Rect>(cluster);
            var graph2 = new UndirectedGraph<Rect>(excludedRects);
            for (var i = 0; i < cluster.Count; i++)
            {
                graph1.AddNode(i, getNeighbors);
            }
            for (var i = 0; i < options.Count; i++)
            {
                graph2.AddNode(i, getNeighbors);
            }

            List<KeyValuePair<int, List<int>>> myList1 = graph1.graph.ToList();
            List<KeyValuePair<int, List<int>>> myList2 = graph2.graph.ToList();

            myList1.Sort(
                delegate (KeyValuePair<int, List<int>> pair1,
                KeyValuePair<int, List<int>> pair2)
                {
                    return pair1.Value.Count.CompareTo(pair2.Value.Count);
                }
            );
            myList1.Reverse();


            myList2.Sort(
                delegate (KeyValuePair<int, List<int>> pair1,
                KeyValuePair<int, List<int>> pair2)
                {
                    return pair1.Value.Count.CompareTo(pair2.Value.Count);
                }
            );

            myList2.Reverse();


            for (var i = 0; i < cycles; i++)
            {
                var r1 = cluster[myList1.Last().Key];
                r1 = new Rect(r1.min, r1.Width, r1.Height);
                var r2 = excludedRects[myList2.Last().Key];
                Console.WriteLine(r1.min);
                Console.WriteLine(r2.min);

                Console.WriteLine("****************");
                cluster[myList1.Last().Key] = r2;
                excludedRects[myList2.Last().Key] = r1;
            }

        }
    }

   


    public class PositionalGrid
    {
        List<Rect> data;

        Dictionary<int, List<int>> xPositions;
        Dictionary<int, List<int>> yPositions;

        public PositionalGrid(List<Rect> data)
        {
            this.data = data;

            xPositions = new Dictionary<int, List<int>>();
            yPositions = new Dictionary<int, List<int>>();

            for (var i = 0; i < data.Count; i++)
            {
                var r = data[i];
                
                if (! xPositions.ContainsKey(r.minX))
                {
                    xPositions[r.minX] = new List<int>();
                }
                if (! yPositions.ContainsKey(r.minY))
                {
                    yPositions[r.minY] = new List<int>();
                }
                if (! xPositions.ContainsKey(r.maxX) )
                {
                    xPositions[r.maxX] = new List<int>();
                }
                if (! yPositions.ContainsKey(r.maxY) )
                {
                    yPositions[r.maxY] = new List<int>();
                }

                xPositions[r.minX].Add(i);
                yPositions[r.minY].Add(i);
                xPositions[r.maxX].Add(i);
                yPositions[r.maxY].Add(i);

            }

        }


        public List<int> getX (int index)
        {
            if (! xPositions.ContainsKey(index))
            {
                return new List<int>();
            }
            return xPositions[index];
        }
        public List<int> getY(int index)
        {
            if (!yPositions.ContainsKey(index))
            {
                return new List<int>();
            }
            return yPositions[index];
        }


    }

    public class UndirectedGraph<T>
    {
        public Dictionary<int, List<int>> graph;

        List<T> data;

        public UndirectedGraph(List<T> data){
                graph = new Dictionary<int, List<int>>();
            this.data = data;
        }

        public delegate List<int> GetNeighborNodes(T item, List<T> items);

        public bool AddNode(int index, List<int> neighbors)
        {
            if (index < 0 || index >= data.Count) return false;
            graph[index] = neighbors;

            return true;
        }

        public bool AddNode (int index, GetNeighborNodes g)
        {
            if (index < 0 || index >= data.Count) return false;
          
             var item = data[index];

            var neighbors = g(item, data);

            graph[index] = neighbors;

            return true;
        }

        public bool RemoveNode (int index)
        {
            var neighbors = graph[index];
            neighbors.ForEach((neighborIndex) =>
            {
                graph[neighborIndex].Remove(index);
            });
            return true;
        }

        public List<int> this[int index]
        {
            get
            {
                return graph[index];       
            }
        }
    }

}


