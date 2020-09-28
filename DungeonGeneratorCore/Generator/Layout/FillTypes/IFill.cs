using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DungeonGeneratorCore.Generator.Layout;
using DungeonGeneratorCore.Generator.TemplateProcessing;
using DungeonGeneratorCore.Generator.Geometry;

namespace DungeonGeneratorCore.Generator.Layout.FillTypes
{
    public interface IFill
    {
        void TryFill(ProcessedZone processedZone, IProp prop, List<PossiblePropPositionsTemplate> validPropPositions, List<Point> zonePoints);

        void DrawProps(PossiblePropPositionsTemplate selectedPropPositions, ProcessedZone processedZone, List<IProp> placedProps);


        PossiblePropPositionsTemplate ChooseSolution(List<PossiblePropPositionsTemplate> validPropPositions);

    }
}
