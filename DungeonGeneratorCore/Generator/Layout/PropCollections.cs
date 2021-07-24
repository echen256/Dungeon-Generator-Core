using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DungeonGeneratorCore.Generator.Layout;

namespace DungeonGeneratorCore.Generator.Layout
{
    public interface IPropCollection : IRelativeCount
    {

        List<IProp> getPropList();

        string[] tags { get; set; }
        string name { get; set; }

    }

    public interface ICollectionPalette
    {
       List<IPropCollection> getPropCollections();
    }

    public interface IRelativeCount
    {
          double getWeight();
        int getMinimumCount();

    }
}


 
