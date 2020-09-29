using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;

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
            return templates;
        }

        public List<Template> loadAllTemplatesInDirectory (string directoryString)
        {
            List<Template> templates = new List<Template>();
            var currentDirectory = Directory.GetCurrentDirectory();
            
            var directory = new DirectoryInfo(directoryString);
            Console.WriteLine(directory.FullName);
            var files = directory.GetFiles();
            foreach(FileInfo fi in files)
            {
                if (fi.Extension == ".json")
                {
                    var json = File.ReadAllText(fi.FullName);
                    Template template = JsonConvert.DeserializeObject<Template>(json);

                    Console.WriteLine(template.name);
                    templates.Add(template);
                }
            }
            //var json = File.ReadAllText(@"..\\Templates\\office -layout-1.json");
           // var json = File.ReadAllText(@"./../../../Dungeon-Generator-Core/DungeonGeneratorCore/Templates/office-layout-1.json");
            
            return templates;
        }
         
    }
}
