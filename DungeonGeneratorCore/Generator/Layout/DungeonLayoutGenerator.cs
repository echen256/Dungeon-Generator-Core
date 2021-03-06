﻿using System;
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

    using Edge = Dungeon_Generator_Core.Geometry.Edge;
    using Point = Dungeon_Generator_Core.Geometry.Point;
    using Rect = Dungeon_Generator_Core.Geometry.Rect;
    public class DungeonLayout
    {

        
        System.Random random = new System.Random();
       
        public TemplateResults execute(int x, int y)
        {

            List<Rect> cluster = new ClusterGenerator().execute(0,0,.95,.5);
            var formattedRects = applyTemplate(cluster);
           formattedRects = selectHallways(formattedRects);
           formattedRects = selectEntrances(formattedRects);
            formattedRects = scaleRoomsAndHallways(formattedRects);
             formattedRects = removeDoubleWalls(formattedRects); 


            return formattedRects;
        }


        TemplateResults removeDoubleWalls(TemplateResults formattedRects)
        {
            var roomRects = formattedRects.allRects;
            for (var i = 0; i < roomRects.Count; i++)
            {
                var r = roomRects[i];
                var nextX = r.maxX + 1;
                var nextZ = r.maxY + 1;
                var adjacentRectZ = roomRects.Find((rect) => { return rect.minY == nextZ; });
                var adjacentRectX = roomRects.Find((rect) => { return rect.minX == nextX; });


                if (adjacentRectX != null && adjacentRectZ != null)
                {
                    roomRects[i] = new Rect(r.minX, r.minY, r.Width + 1, r.Height + 1);

                }
                else if (adjacentRectX != null)
                {
                    roomRects[i] = new Rect(r.minX, r.minY, r.Width + 1, r.Height);

                }
                else if (adjacentRectZ != null)
                {
                    roomRects[i] = new Rect(r.minX, r.minY, r.Width, r.Height + 1);
                }

            }
            return formattedRects;
        }

        void processSingleRect(List<Rect> rects, Rect r, int adjustedWidth, int adjustedHeight)
        {
     
            for (var j = 0; j < rects.Count; j++)
            {
                var r2 = rects[j];
                if (! r2.Equals(r))
                { 
                    var h = r2.Height;
                    var w = r2.Width;
                    var x0 = r2.minX;
                    var y0 = r2.minY;
                    if (r.minX == r2.minX)
                    {
                        w = adjustedWidth;
                    }


                    if (r.minY == r2.minY)
                    {
                        h = adjustedHeight;
                    }

                    if (r2.minX > r.maxX)
                    {
                        x0 += adjustedWidth - r.Width;
                    }
                    if (r2.minY > r.maxY)
                    {
                        y0 += adjustedHeight - r.Height;
                    }
                    r2.min = new Point(x0, y0);
                    r2.Height = h;
                    r2.Width = w;
                } 

            }

            r.Width = adjustedWidth;
            r.Height = adjustedHeight;
          
        }

        TemplateResults applyTemplate(List<Rect> rects)
        {
            var paths = new List<Rect>();
            var nonPaths = new List<Rect>();
            var centers = new List<Rect>();
            var allRects = new List<Rect>();
            for (var i = 0; i < rects.Count; i++)
            {
                var r = rects[i];
                for (var j = 0; j < 3; j++)
                {
                    for (var k = 0; k < 3; k++)
                    {
                        var r2 = new Rect(r.minX * 3 + j, r.minY * 3 + k, 1, 1);

                        if (j == 1 && k == 1)
                        {
                            centers.Add(r2);
                        } else 
                        if (j == 1 || k == 1)
                        {
                            paths.Add(r2);
                        }
                        else
                        {
                            nonPaths.Add(r2);
                        }
                        allRects.Add(r2); 
                    }
                }

            }

            return new TemplateResults(allRects, paths, nonPaths, centers);
        }

        TemplateResults scaleRoomsAndHallways (TemplateResults formattedRects)
        {

            var paths = formattedRects.pathIndices;
            var nonPaths = formattedRects.nonPathIndices;
            var allRects = formattedRects.allRects;
            var centers = formattedRects.centers;

            for (var i = 0; i < nonPaths.Count; i++)
            {

                var adjustedHeight =   random.Next(6, 13);
                var adjustedWidth =   random.Next(6, 13);

                processSingleRect(allRects, nonPaths[i], adjustedWidth, adjustedHeight);
            }

            for (var i = 0; i < centers.Count; i++)
            {
                var adjustedWidth =   random.Next(3, 5);

                processSingleRect(allRects, centers[i], adjustedWidth, adjustedWidth);
            }
            return formattedRects;
        }


        TemplateResults selectEntrances (TemplateResults formattedRects)
        {

            var numberOfEntrances = 4;
            formattedRects.allRects = formattedRects.allRects.OrderBy((x) =>  random.NextDouble() ).ToList();

            var options = new List<Rect>(formattedRects.allRects);
            for (var i = 0; i < options.Count;i++)
            {
                if (numberOfEntrances == 0) break;
                var option = options[i]; 
                if (Rect.neighbors(option, formattedRects.allRects).Count == 3 && Rect.neighbors(option, formattedRects.centers).Count == 1)
                {
                    formattedRects.centers.Add(option);
                    formattedRects.pathIndices.Remove(option); 
                    numberOfEntrances--;
                }
            }
            return formattedRects;
        }


        TemplateResults selectHallways (TemplateResults formattedRects)
        {
          
           
            var edges = new List<Edge>();

            var coordinates = new List<Coordinate>();
            formattedRects.centers.ForEach((p) =>
            {
                coordinates.Add(new Coordinate(p.min.X, p.min.Y));
            });

            var triangulator = new  DelaunayTriangulationBuilder();
            triangulator.SetSites(coordinates.ToArray());
            var triangulatedEdges = triangulator.GetEdges(new GeometryFactory(new PrecisionModel(.1)));
            triangulatedEdges.Geometries.ToList().ForEach((edge) => {
                var Q = edge.Coordinates[0];
                var P = edge.Coordinates[1];
                edges.Add(new Edge(new Point((int)Q.X, (int) Q.Y), new Point((int) P.X, (int) P.Y)));
               
            });

  
            var edgePoints = new HashSet<Point>();
      
            edges = new Prims().exec(edges); 
            edges.ForEach((edge) =>
            { 
                var p12 = edge.getPoints(); 
                edgePoints.UnionWith(edge.getPoints());
            });

            var newPaths = new HashSet<Rect>();
            var newNonPaths = formattedRects.nonPathIndices;

                formattedRects.allRects.ForEach((r) =>
                {
                    if (edgePoints.Contains(r.min) )
                    {

                        newPaths.Add(r);   
                    } else 
                    {
                        newNonPaths.Add(r);
                    }
                });  
            formattedRects.nonPathIndices = newNonPaths.ToList();
            formattedRects.pathIndices = newPaths.ToList(); 
            return formattedRects;
        }
    
    }
}
