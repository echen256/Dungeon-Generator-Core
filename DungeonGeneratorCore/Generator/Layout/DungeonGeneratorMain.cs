using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Triangulate;
using NetTopologySuite.CoordinateSystems;
using GeoAPI.Geometries;
using Dungeon_Generator_Core.Geometry;
using Dungeon_Generator_Core.Generator;
using NetTopologySuite.Geometries;
using System.Diagnostics;

namespace Dungeon_Generator_Core.Layout
{
    public class DungeonGeneratorMain
    {

        System.Random random = new System.Random();

        public List<Rect> subdivide(List<Rect> cluster, List<Rect> total, int divisions, int newWidth)
        {

            List<Rect> cluster2 = new List<Rect>();
         
            cluster.ForEach((rect) =>
            {
                total.Remove(rect);
                for (var i = 0; i < divisions; i++)
                {
                    for (var j = 0; j < divisions; j++)
                    {
                        var minX = rect.minX + i * newWidth;
                        var minY = rect.minY + j * newWidth;
                        var r = new Rect(minX, minY, newWidth, newWidth);
                        cluster2.Add(r);
                        total.Add(r);
                    }
                }
            });
            Console.WriteLine(cluster.Count + ", " + cluster2.Count);
            return cluster2;
        }

        public void calcBorderOfCluster(List<Rect> cluster, List<Rect> total, UndirectedGraph<Rect> neighbormap, List<Rect> border, List<Rect> nonBorder)
        {

              for (var i = 0; i < cluster.Count; i++) {
                var rect = cluster[i];
                var index = total.IndexOf(rect);
                var neighbors = neighbormap[index];
                var outsideNeighborCount = 0;
                neighbors.ForEach((neighborIndex) =>
                {
                    var neighbor = total[neighborIndex];
                    if (! cluster.Contains(neighbor))
                    {
                        outsideNeighborCount++;
                    }
                });
                if (outsideNeighborCount > 0)
                {
                    border.Add(rect);
                } else
                {
                    nonBorder.Add(rect);
                }
            };

        }

        public void subdivideCluster(List<Rect> cluster1, List<List<Rect>> clusters)
        {
            while (cluster1.Count > 0  )
            {
                var nextCluster = new ClusterGenerator().execute(1, .85, cluster1);
                cluster1 = cluster1.Except(nextCluster).ToList();
                clusters.Add(nextCluster);
            
            }
        }
        public List<Room> execute2(int x, int y, ICollectionPalette collectionPalette)
        {

            var width = 16;
            var rooms = new List<Room>();
            var cluster1 = new ClusterGenerator().execute(1, .75, 10, 10, width);
            var divisions = 2;
            var output = new List<Room>();
            loop(width, divisions, cluster1, output,2, "0");


            return output;
        }


        void loop(int width, int divisions, List<Rect> cluster1, List<Room> output, int iterations, string id)
        {
            if (divisions <= 0 | width <= 0 | iterations <= 0)
            {
                cluster1.ForEach((rect) =>
                {
                    output.Add(new Room(rect, id));
                });
                return;
            }

 
            var newWidth = width / divisions;

            var newClusters = new List<List<Rect>>();
            var total = cluster1;
            subdivideCluster(cluster1, newClusters);


            int clusterCount = 0;
            for (var i = 0; i < newClusters.Count; i++) {
                newClusters[i] = subdivide(newClusters[i], total, divisions, newWidth);
            }

            var neighborMap = ClusterGenerator.generateNeigbhorMapFromArbitraryRects(total);

            for (var i = 0; i < newClusters.Count; i++)
            {
                var border = new List<Rect>();
                var nonBorder = new List<Rect>();

                var cluster2 = newClusters[i];

                /*if (iterations == 1)
                {
                    nonBorder = cluster2;
                } else
                {*/
                   calcBorderOfCluster(cluster2, total, neighborMap, border, nonBorder);
               // }
                 
         

                border.ForEach((r) =>
                {
                    output.Add(new Room(r, "path"));
                });

                if (iterations > 0)
                {
                    loop(newWidth, divisions  , nonBorder, output, iterations - 1,  id + ":" + clusterCount );
                }

                clusterCount++;
            };
        }

        public List<Room> execute(int x, int y, ICollectionPalette collectionPalette)
        {
             var propCollections = collectionPalette.getPropCollections();
             var templateResults = new DungeonLayout().execute(x,y);
             var rooms = new RoomLayoutManager().execute(templateResults); 
            var propCollectionDistribution = new Distribute().DistributeItems(propCollections, rooms.Count - 1);


             rooms.ForEach((r) => {
                 if (r.category == "room")
                 {
                     var propCollection = propCollectionDistribution.Dequeue();
                     r.props = new FurnitureLayoutGenerator().generateRoomLayout(r, propCollection, 1000);
                     propCollectionDistribution.Enqueue(propCollection);
                 }

             });

            return rooms;
        }


    }
 
     
}
