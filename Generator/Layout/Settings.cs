using System;
using System.Collections.Generic;
using System.Text;

using Dungeon_Generator_Core.Geometry;
using Dungeon_Generator_Core.Layout;
using Dungeon_Generator_Core.Generator;

namespace Dungeon_Generator_Core.Generator.Layout
{
    struct DungeonLayoutSettings
    {
        public int adjustedWithMin;
        public int adjustedWidthMax;

        public int adjustedHeightMin;
        public int adjustedHeightMax;

        public int hallwayWidthMin;
        public int hallwayWidthMax;

        public int entranceCount;
    }

    struct FurnitureLayoutSettings
    {
        public int minPropArrayArea;
        public int maxPropArrayArea;

        public int minPropArrayCount;
        public int maxPropArrayCount;

        public List<Prop> propList;


    }
}
