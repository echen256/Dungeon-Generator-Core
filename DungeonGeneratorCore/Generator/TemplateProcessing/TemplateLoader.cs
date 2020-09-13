using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.IO;


namespace Dungeon_Generator_Core.TemplateProcessing
{
    public class TemplateLoader
    {
        public List<Template> execute()
        {
            List<Template> templates = new List<Template>();

            var json = File.ReadAllText(@"E:\Dev\Github\DungeonGeneratorCore\DungeonGeneratorCore\Templates\office-layout-1.json");
            Template template = JsonConvert.DeserializeObject<Template>(json);
            templates.Add(template);
            Console.WriteLine(template);

            return templates;
        }
    }
}
