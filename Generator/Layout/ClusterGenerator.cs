using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon_Generator_Core.Geometry;

namespace Dungeon_Generator_Core.Generator
{
    class ClusterGenerator
    {

        System.Random random = new System.Random();
        public double defaultSpreadPossibility = .95;

        public List<Rect> testCase()
        {
            return new Rect[] {new Rect(new Point(0,0),1,1) , new Rect(new Point(1, 0), 1, 1) , new Rect(new Point(0, 1), 1, 1) , new Rect(new Point(1,1), 1, 1), new Rect(new Point(2, 1), 1, 1), new Rect(new Point(2, 0), 1, 1) }.ToList();
        }

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


