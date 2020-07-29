using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dungeon_Generator_Core.Geometry;
using Dungeon_Generator_Core.Generator;

namespace Dungeon_Generator_Core.Layout
{
   
        class TemplateResults
        {
            public List<Rect> allRects;
        public List<Rect> pathIndices;
        public List<Rect> nonPathIndices;
        
        public List<Rect> centers;
        public TemplateResults(List<Rect> allRects, List<Rect> pathIndices, List<Rect> nonPathIndices, List<Rect> centers)
            {
                this.allRects = allRects;
                this.pathIndices = pathIndices;
                this.nonPathIndices = nonPathIndices;
            this.centers = centers;
            }

        }
  
}
