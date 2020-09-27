using System;
using System.Collections.Generic;
using System.Text;

using DungeonGeneratorCore.Generator.Geometry;
using DungeonGeneratorCore.Generator.Layout;
using DungeonGeneratorCore.Generator;

namespace DungeonGeneratorCore.Generator.Layout
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
