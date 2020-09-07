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
