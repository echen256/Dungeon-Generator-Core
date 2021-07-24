﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DungeonGeneratorCore.Generator.TemplateProcessing
{

    [Serializable]
    public class Template
    {
        public string name;
        public int miniumumWidth;
        public int minimumHeight;
        public List<Zone> zones;
        public string[] tags;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    
 
    }
}
