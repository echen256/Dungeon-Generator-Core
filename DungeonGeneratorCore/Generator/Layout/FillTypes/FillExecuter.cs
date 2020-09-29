using DungeonGeneratorCore.Generator.Layout;
using System;
using System.Collections.Generic;
using DungeonGeneratorCore.Generator.TemplateProcessing;
using DungeonGeneratorCore.Generator.Geometry;
using System.Linq;
using System.Data;
using System.Runtime.InteropServices;

namespace DungeonGeneratorCore.Generator.Layout.FillTypes
{

	using Point = DungeonGeneratorCore.Generator.Geometry.Point;


	public class FillExecuter
	{
		Random random = new Random();
		public int minimumCount = 1;
		public int maximumCount = 5;
		public int maximumWidth = 10;
		public void execute(Room room, Zone zone, IPropCollection propCollection, List<IProp> placedProps, IFill fillType)
		{
			int cycles = 40;
			var validPropListMemory = new List<IProp>();
			var processedZone = new ProcessedZone(room, zone);
			var zonePoints = processedZone.getPointsInZone().ToList();
			var validPropList = propCollection.getPropList().FindAll((prop) => { return processedZone.tags.Contains(prop.Identifier()); });
			
			if (validPropList.Count == 0)
            {
				return;
            }
			while (cycles > 0)
			{
				

				cycles--;
				
				if (processedZone.boundingRect.Area <= 0)
				{
					break;
				}

			
				var validPropPositions = new List<PossiblePropPositionsTemplate>();

				var prop = chooseNextProp(validPropList, validPropListMemory);

				fillType.TryFill(processedZone, prop, validPropPositions, zonePoints );
				 
				if (validPropPositions.Count > 0)
				{
					var selectedPropPositions = fillType.ChooseSolution(validPropPositions, processedZone);
					fillType.DrawProps(selectedPropPositions, placedProps, out processedZone, zone);
				}

				Console.WriteLine(cycles + ", " + validPropList.Count + ", " + processedZone.boundingRect.Area);
			}


		}

		public IProp chooseNextProp(List<IProp> validPropList, List<IProp> validPropListMemory)
        {
			if (validPropList.Count == 0)
			{

				validPropList = new List<IProp>(validPropListMemory);
				validPropList = validPropList.OrderByDescending((item) => { return random.NextDouble(); }).ToList(); ;
			}

			var selectedIndex = random.Next(0, validPropList.Count);
			var prop = validPropList[selectedIndex];
			validPropList.RemoveAt(selectedIndex);
			validPropListMemory.Add(prop);
			return prop;
		}
		 

	}
}
