using System;
using System.Collections.Generic;

namespace Generator
{
    public class Subdivider
    {


        int[] divisionWidths = new int[] { 1, 2, 3 };
        int minWidth;
        int minHeight;
        int maxArea;
        System.Random random = new System.Random();
        public List<CellRect> execute(IntVec3 vec, int radius, int iterations, int minWidth, int minHeight, int maxArea)
        {
            var rect = new CellRect(vec.x - radius, vec.z - radius, 2 * radius + 1, 2 * radius + 1);

            return execute(rect, iterations, minWidth, minHeight, maxArea);
        }

        public List<CellRect> execute(CellRect rect, int iterations, int minWidth, int minHeight, int maxArea)
        {
            this.minHeight = minHeight;
            this.minWidth = minWidth;
            this.maxArea = maxArea;
            var results = new List<CellRect>();
            subdivide(rect, results, iterations, true);
            return results;
        }

        public void subdivide(CellRect rect, List<CellRect> results, int iterations, bool vertical)
        {
            if (iterations < 1)
            {

                results.Add(rect);
                return;
            }

            var subdivisions = parseResults(rect, 1, vertical);
            if (subdivisions.Count == 1)
            {
                results.Add(subdivisions[0]);
                return;
            }

            foreach (CellRect result_rect in subdivisions)
            {
                subdivide(result_rect, results, iterations - 1, result_rect.Width > result_rect.Height);
            }


        }

        public List<CellRect> parseResults(CellRect rect, int divisionWidth, bool v)
        {
            var rects = new List<CellRect>();
            var vertical = v;
            var area = rect.Width * rect.Height;
            if (rect.Width < 2 * minWidth + divisionWidth && rect.Height < 2 * minHeight + divisionWidth)
            { 
                rects.Add(rect);
                return rects;
            }

            else if (rect.Width < 2 * minWidth + divisionWidth)
            {
                vertical = false;
            }
            else if (rect.Height < 2 * minHeight + divisionWidth)
            {
                vertical = true;
            }

            if (vertical)
            {
                var w = Math.Max(minWidth, (int)Math.Floor(random.NextDouble() * rect.Width / 2));
                var w2 = rect.Width - w - divisionWidth;
                rects.Add(new CellRect(rect.minX, rect.minZ, w, rect.Height));
                rects.Add(new CellRect(w + rect.minX + divisionWidth, rect.minZ, w2, rect.Height));
            }
            else
            {
 
                var h = Math.Max(minHeight, (int)Math.Floor(random.NextDouble() * rect.Height / 2));
                var h2 = rect.Height - h - divisionWidth;
                rects.Add(new CellRect(rect.minX, rect.minZ, rect.Width, h));
                rects.Add(new CellRect(rect.minX, rect.minZ + h + divisionWidth, rect.Width, h2));
            }
            return rects;

        }



        public List<CellRect> parallelCutSubdivide (CellRect rect, int subdivision_width, int subdivision_height)
        {
            var output = new List<CellRect>();
            var minX = rect.minX;
            var minZ = rect.minZ;

           
            var i_1 = 0;
            var j_1 = 0;

            var maximumCount = getMaximumNumberOfPropsPlaced(rect, subdivision_width, subdivision_height);
            var horizontalCount = maximumCount.x;
            var verticalCount = maximumCount.z;


            var remainders = getRemainder(rect, subdivision_width, subdivision_height);
 

            for (var i = 0; i < horizontalCount;i++)
            {
                for (var j = 0; j < verticalCount;j++)
                {

                    i_1 = i * subdivision_width;
                    j_1 = j * subdivision_height; 

                    var r = new CellRect(minX + i_1, minZ + j_1, subdivision_width, subdivision_height);
                
                    output.Add(r);
                }
            }

            return output;
        }

        public IntVec2 getRemainder(CellRect rect, int subdivision_width, int subdivision_height)
        {

            var maximumCount = getMaximumNumberOfPropsPlaced(rect, subdivision_width, subdivision_height);
            var horizontalCount = maximumCount.x;
            var verticalCount = maximumCount.z;



            var vertical_remainder = rect.Height % (subdivision_height * verticalCount);
            var horizontal_remainder = rect.Width % (subdivision_width * horizontalCount);

            return new IntVec2(horizontal_remainder, vertical_remainder);
        }

        public IntVec2 getMaximumNumberOfPropsPlaced(CellRect rect, int subdivision_width, int subdivision_height)
        {
            var horizontalCount = rect.Width / subdivision_width;
            var verticalCount = (rect.Height) / (subdivision_height ) ;


            return new IntVec2(horizontalCount, verticalCount);
        }

        public IntVec2 getMaximumNumberOfPropsPlacedWithinBounds(CellRect rect, int subdivision_width, int subdivision_height, int area)
        {

            var sqrt = (int)Math.Sqrt(area);

            var h = sqrt / subdivision_width;
            var v = sqrt / subdivision_height;
            
            var horizontalCount =rect.Width / subdivision_width;
            var verticalCount = (rect.Height) / (subdivision_height);
            

            return new IntVec2(Math.Min(horizontalCount,h), Math.Min(v, verticalCount));
        }

        public int getSortValue(CellRect rect, int subdivision_width, int subdivision_height)
        {

            var remainders = getRemainder(rect, subdivision_width, subdivision_height);
            var maximumCount = getMaximumNumberOfPropsPlaced(rect, subdivision_width, subdivision_height);
            var horizontalCount = maximumCount.x;
            var verticalCount = maximumCount.z;
            var vertical_remainder = remainders.z;
            var horizontal_remainder = remainders.x;


            return - (horizontalCount * verticalCount ) * (vertical_remainder) * (horizontal_remainder);
        }

    }
}
