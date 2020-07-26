using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator.Layout
{
    class DungeonLayout
    {

        System.Random random = new System.Random();
       
        public List<Rect> execute(int x, int y)
        {

            List<Rect> cluster = new ClusterGenerator().execute(x, y);

 
            var formattedRects = applyTemplate(cluster);
            var paths = formattedRects.pathIndices;
            var nonPaths = formattedRects.nonPathIndices;
            var allRects = formattedRects.allRects;


            for (var i = 0; i < nonPaths.Count; i++)
            {

                var adjustedHeight = random.Next(6, 13);
                var adjustedWidth = random.Next(6, 13);

                processSingleRect(allRects, nonPaths[i], adjustedWidth, adjustedHeight);
            }

            for (var i = 0; i < paths.Count; i++)
            {
                var r = paths[i];

                var adjustedWidth = r.Width;
                var adjustedHeight = r.Height;
                if (adjustedHeight > adjustedWidth)
                {
                    adjustedWidth = random.Next(2, 5);
                }
                else
                {
                    adjustedHeight = random.Next(2, 5);
                }
                processSingleRect(allRects, paths[i], adjustedWidth, adjustedHeight);
            }

        
          //  removeDoubleWalls(output);
            // output = mergeNextRoom(output);
            return allRects;
        }

       /* List<CellRect> mergeNextRoom(List<Rect> roomRects)
        {
            var output = new List<CellRect>();
            roomRects.Sort((a, b) =>
            {
                return a.minX.CompareTo(b.minX) + a.minZ.CompareTo(b.minZ);
            });
            while (roomRects.Count > 0)
            {
                var r = roomRects[0];
                roomRects.RemoveAt(0);
                var nextX = r.maxX;
                var nextZ = r.maxZ;

                var adjacentRectX = roomRects.Find((rect) => { return rect.minX == nextX && !output.Contains(rect); });
                var adjacentRectZ = roomRects.Find((rect) => { return rect.minZ == nextZ && !output.Contains(rect); });

                if (adjacentRectX != null)
                {
                    if (adjacentRectX.Height == r.Height)
                    {
                        output.Add(new CellRect(r.minX, r.minZ, r.Width + adjacentRectX.Width - 1, r.Height));
                    }
                    else
                    {
                        output.Add(r);

                    }
                }
                else if (adjacentRectZ != null)
                {
                    if (adjacentRectX.Width == r.Width)
                    {
                        output.Add(new CellRect(r.minX, r.minZ, r.Width, r.Height + adjacentRectX.Height - 1));
                    }
                    else
                    {
                        output.Add(r);
                    }

                }
                else
                {
                    output.Add(r);
                }
            }
            return output;
        }*/

        /*void removeDoubleWalls(List<CellRect> roomRects)
        {
            for (var i = 0; i < roomRects.Count; i++)
            {
                var r = roomRects[i];
                var nextX = r.maxX + 1;
                var nextZ = r.maxZ + 1;
                var adjacentRectZ = roomRects.Find((rect) => { return rect.minZ == nextZ; });
                var adjacentRectX = roomRects.Find((rect) => { return rect.minX == nextX; });


                if (adjacentRectX != null && adjacentRectZ != null)
                {
                    roomRects[i] = new CellRect(r.minX, r.minZ, r.Width + 1, r.Height + 1);

                }
                else if (adjacentRectX != null)
                {
                    roomRects[i] = new CellRect(r.minX, r.minZ, r.Width + 1, r.Height);

                }
                else if (adjacentRectZ != null)
                {
                    roomRects[i] = new CellRect(r.minX, r.minZ, r.Width, r.Height + 1);
                }

            }
        }*/

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
                    r2.Width = w;
                    r2.Height = h; 
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

                        if (j == 1 && i == 1)
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
    }
}
