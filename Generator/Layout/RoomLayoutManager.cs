using Dungeon_Generator_Core.Generator;
using System;
using System.Collections.Generic;
using System.Text;
using Dungeon_Generator_Core.Geometry;
using System.Linq;
using System.Data;

namespace Dungeon_Generator_Core.Layout
{
    class RoomLayoutManager
    {
 
        public List<Room> execute (TemplateResults formattedRects)
        {
            var rooms = new List<Room>();
		 
            formattedRects.pathIndices.ForEach((path) =>
            {
                rooms.Add(new Room(path, "path"));
            });
            formattedRects.centers.ForEach((path) =>
            {
                rooms.Add(new Room(path, "center"));
            });
            formattedRects.nonPathIndices.ForEach((path) =>
            {
                rooms.Add(new Room(path, "room"));
            });
            rooms = new List<Room>(new HashSet<Room>(rooms));
			chooseEntrances(rooms );
            
           
            rooms = mergeRooms(rooms,1,IsDisconnected, IsValidRoom );
            rooms = mergeRooms(rooms, 1, IsValidRoom, IsValidRoom);
            rooms = mergeRooms(rooms, 100, IsHallwayRoom, IsHallwayRoom);

            return rooms;

        }

        private bool IsHallwayRoom(Room r)
        {
            return r.category == "path" || r.category == "center" ;
        }
        private bool IsValidRoom (Room r)
        {
            return r.category == "room";
        }
        private bool IsDisconnected (Room r)
        {
            return r.entrances.Count == 0;
        }
		public List<Room> mergeRooms (List<Room> rooms, int iterations, Func<Room, bool> eligibility, Func<Room, bool> validNeighbor)  {

            var eligibleRooms = new List<Room>(rooms.Where(eligibility ).Where(validNeighbor));

            if (eligibleRooms.Count() == 0 || iterations == 0)
            {
                return rooms;
            } 
             
			while (eligibleRooms.Count  > 0)
			{
                var r = eligibleRooms.Last();
                eligibleRooms.RemoveAt(eligibleRooms.Count - 1);
 
                    var area = 1000;
                    Room selectedRoom = null;
                    var neighbors = r.getNeighbors(rooms).Where(validNeighbor).ToList();
                    if (neighbors.Count > 0)
                    {
                        neighbors = neighbors.OrderBy((neighbor) => { return neighbor.area(); }).ToList();

                        r.merge(neighbors[0]);
                        rooms.Remove(neighbors[0]);
                        eligibleRooms.Remove(neighbors[0]);
                        
                    }
               
            }
          
            return mergeRooms(rooms, iterations - 1, eligibility,validNeighbor  );
		}

	   public void chooseEntrances(List<Room> rooms )
        {
			var pathRooms = rooms.FindAll((r) => { return r.category == "path"; });
			var eligibleRooms = rooms.FindAll((r) => { return r.category == "room"; });
			eligibleRooms.ForEach((r) => {

                var neighboringPaths = r.getNeighbors(pathRooms);
                var neighboringPoints = new List<Point>();
                neighboringPaths.ForEach((rect) => {
                    neighboringPoints.AddRange(rect.edgePoints);
                });
                var borderPoints = r.points.Where((p) => {
                    return Point.getNeighbors(p, neighboringPoints).Count > 0;
                }).ToList();


                borderPoints = borderPoints.Where((p) => {
                    return Point.getNeighbors(p, r.points).Count == 3;
                }).ToList();

                 
                for (var i = 0; i < Math.Min(1, borderPoints.Count); i++) {
                    r.addEntrance(borderPoints[i]);
                }
            });

		}
    }
}
