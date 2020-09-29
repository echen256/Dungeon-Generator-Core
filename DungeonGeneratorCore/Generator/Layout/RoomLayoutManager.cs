using DungeonGeneratorCore.Generator;
using System;
using System.Collections.Generic;
using System.Text;
using DungeonGeneratorCore.Generator.Geometry;
using System.Linq;
using System.Data;

namespace DungeonGeneratorCore.Generator.Layout
{
    public class RoomLayoutManager
    {
 
        public static List<Point> getBoundsOfRooms (List<Room> rooms)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;

            for (var i = 0; i < rooms.Count
                ; i++)
            {
                var points = rooms[i].points;
                for (var j = 0; j < points.Length
                    ; j++)
                {
                    if (minX > points[j].X)
                    {
                        minX = points[j].X;
                    }
                    if (minY > points[j].Y)
                    {
                        minY = points[j].Y;
                    }
                    if (maxX < points[j].X)
                    {
                        maxX = points[j].X;
                    }
                    if (maxY < points[j].Y)
                    {
                        maxY = points[j].Y;
                    }
                }
            }

            return new Point[] {  new Point(minX,minY), new Point(maxX,maxY), new Point(maxX - minX, maxY - minY)}.ToList();
        }
        public static List<Room> adjustToCenterOfBounds (List<Room> rooms, int minBoundsX, int minBoundsY, int sizeX, int sizeY)
        {

            var bounds = getBoundsOfRooms(rooms);
            var minX = bounds[0].X;
            var minY = bounds[0].Y;

            var offsetX = minBoundsX - minX;
            var offsetY = minBoundsY - minY;
            for (var i = 0; i < rooms.Count
                ; i++)
            {
                var room = rooms[i];
                var points = room.points;
                for (var j = 0; j < points.Length
                    ; j++)
                {
                    points[j].X += offsetX;
                    points[j].Y += offsetY;
                }
              /*  var entrances = room.entrances;
                for (var j = 0; j < entrances.Count; j++)
                {
                    entrances[j].X += offsetX;
                    entrances[j].Y += offsetY;

                }*/

            }

            return rooms;
        }
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

                var borderPoints = r.edgePoints.Where((p) => {
                    return neighboringPoints.Contains(p);
                }).ToList();
                 
                borderPoints = borderPoints.Where((p) => {
                    return Point.getNeighbors(p, r.points).Count == 3 ;
                }).ToList();

                

                 
                for (var i = 0; i < Math.Min(1, borderPoints.Count); i++) {
                    r.addEntrance(borderPoints[i]);
                }
            });

		}
    }
}
