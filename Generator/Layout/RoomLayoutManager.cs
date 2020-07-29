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
            
           
            rooms = mergeRooms(rooms, 2);
           
            return rooms;

        } 
		public List<Room> mergeRooms (List<Room> rooms, int iterations)  {
            if (iterations == 0) return rooms;
            var newRooms = new List<Room>();
			while (rooms.Count > 0)
			{
                var r = rooms[rooms.Count - 1];
                rooms.RemoveAt(rooms.Count - 1);

                if (r.category == "room"  )
                {
                    var area = 1000;
                    Room selectedRoom = null;
                    var neighbors = r.getNeighbors(rooms).Where((r) => { return r.category == "room"; }).ToList();
                    neighbors.ForEach((neighbor) => {
                        if (neighbor.area() < area)
                        {
                            selectedRoom = neighbor;
                            area = neighbor.area();
                        }
                    });


                    if (selectedRoom != null)
                    {
                        r.merge(selectedRoom);
                        rooms.Remove(selectedRoom);
                    }
                }
                newRooms.Add(r);
            }
          
            return mergeRooms(newRooms,iterations - 1);
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
