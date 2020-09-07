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
    public class Distribute
    {

        
        public Queue<IPropCollection> DistributeItems (List<IPropCollection> list, int count)
        {
            System.Random Random = new System.Random();
            Queue<IPropCollection> queue = new Queue<IPropCollection>();
            
            var sumOfWeights = 0.0;
        
            list = list.OrderByDescending((pc) => {
                sumOfWeights += pc.getWeight();
                return pc.getMinimumCount();
            }).ToList();

            list.ForEach((pc) => {
                var minimumCount = pc.getMinimumCount(); 
                var weight = pc.getWeight();
                var share = Math.Max((int)(count * weight / sumOfWeights), minimumCount);
 
                for (var i = 0; i < share; i++)
                {
                   queue.Enqueue(pc);
                }

            }); 
 
            return queue;
        }
    }
}
