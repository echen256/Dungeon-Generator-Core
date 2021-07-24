using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;

namespace DungeonGeneratorCore.Generator.Layout
{
    public class PropJSONLoader
    {
        public List<IProp> execute()
        {
            List<IProp> props = new List<IProp>();
            //var json = File.ReadAllText(@"..\\Props\\office -layout-1.json");
            var json = File.ReadAllText(@"./../../../Dungeon-Generator-Core/DungeonGeneratorCore/PropCollections/office-props-1.json");
            Prop prop = JsonConvert.DeserializeObject<Prop>(json);
            props.Add(prop);
            return props;
        }


      


        public List<Prop> loadAllPropsInDirectory(string directoryString)
        {
            List<Prop> Props = new List<Prop>();
            var currentDirectory = Directory.GetCurrentDirectory();

            var directory = new DirectoryInfo(directoryString); 
            var files = directory.GetFiles();
            foreach (FileInfo fi in files)
            {
                if (fi.Extension == ".json")
                { 
                    var json = File.ReadAllText(fi.FullName);
                    Prop Prop = JsonConvert.DeserializeObject<Prop>(json);
                    Props.Add(Prop);
                }
            }
            return Props;
        }

    }
}
