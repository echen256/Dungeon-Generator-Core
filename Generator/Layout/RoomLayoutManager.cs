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

			chooseEntrances(rooms );
           /* var otherRooms = rooms.Where((r) => { return r.category != "room"; });
            rooms = rooms.Where((r) => { return r.category == "room"; }).ToList();
            for (var i = 0; i < 1; i++)
            {
                rooms = mergeRooms(rooms);
            }
		 	
            rooms.AddRange(otherRooms);*/
			return rooms;

        }


		 

		public List<Room> mergeRooms (List<Room> rooms)  { 
			var stack = new Stack<Room>(rooms);
          //  var newRooms = new List<Room>( );

			while (stack.Count > 0)
			{
                var r = stack.Pop(); 
				var neighbors = r.getNeighbors(rooms);

				var area = 1000;
				Room selectedRoom = null;
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
				//	rooms.Remove(selectedRoom);
				}
             //   newRooms.Add(r);
            }
            return rooms;
		}

	   public void chooseEntrances(List<Room> rooms )
        {
			var pathRooms = rooms.FindAll((r) => { return r.category == "path"; });
			var eligibleRooms = rooms.FindAll((r) => { return r.category == "room"; });
			eligibleRooms.ForEach((r) => {

                var neighboringPaths = r.getNeighbors(pathRooms);
                var neighboringPoints = new List<Point>();
                neighboringPaths.ForEach((rect) => {
                    neighboringPoints.AddRange(rect.getEdgePoints());
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
