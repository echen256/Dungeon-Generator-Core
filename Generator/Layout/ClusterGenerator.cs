using System;
using System.Collections.Generic;

namespace Generator.Layout
{
    class ClusterGenerator
    {

        System.Random random = new System.Random();
        public double defaultSpreadPossibility = .75;

        public List<Rect> execute(int x, int y)
        {
            List<Rect> rects = new List<Rect>();
            recursiveGenerate(0, 0, defaultSpreadPossibility, rects);
            return rects;
        }

        void recursiveGenerate (int x, int y , double probability, List<Rect> rects)
        {
            var rect = new Rect(x, y, 1, 1);
            if (rects.Contains(rect))
            {
                return;
            }
            rects.Add(rect);
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    if (Math.Abs(i) + Math.Abs(j) == 1)
                    {
                        if (random.NextDouble() < probability)
                        {

                            recursiveGenerate(x + i, j + y, probability / 2, rects);
                        }
                    }
                }
            }
        }
    }

}


