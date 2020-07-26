using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Generator.Layout
{
    class Room
    {
        public List<IntVec2> entrances;
        public List<IntVec2> points;

        public static List<IntVec2> directions = new IntVec2[] { new IntVec2(1,0) , new IntVec2(0,1), new IntVec2(-1,0), new IntVec2(0,-1)}.ToList();
    
        public Room (CellRect rect)
        {

        }
    
    }
}
