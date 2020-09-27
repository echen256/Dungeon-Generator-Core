using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.IO;


namespace DungeonGeneratorCore.Generator.TemplateProcessing
{
    public class TemplateLoader
    {
        public List<Template> execute()
        {
            List<Template> templates = new List<Template>();
            //var json = File.ReadAllText(@"..\\Templates\\office -layout-1.json");
            var json = File.ReadAllText(@"./../../../Dungeon-Generator-Core/DungeonGeneratorCore/Templates/office-layout-1.json");
            Template template = JsonConvert.DeserializeObject<Template>(json);
            templates.Add(template);
            Console.WriteLine(template);

            return templates;
        }
    }
}
