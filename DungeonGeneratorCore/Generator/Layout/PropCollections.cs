using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeon_Generator_Core.Layout;

namespace Dungeon_Generator_Core.Layout
{
    public interface IPropCollection : IRelativeCount
    {

        List<IProp> getPropList();

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


 
