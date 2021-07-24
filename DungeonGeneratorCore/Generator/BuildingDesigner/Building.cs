using DungeonGeneratorCore.Generator.Geometry;
using DungeonGeneratorCore.Generator.Layout;
using DungeonGeneratorCore.Generator.TemplateProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DungeonGeneratorCore.Generator.BuildingDesigner
{
    public class Building
    {
        List<ProcessedZone> roomZones;
        Rect boundingRect;
        public List<Room> rooms;
        
        public Building(Template buildingTemplate, Rect boundingRect)
        {
            roomZones = new List<ProcessedZone>();
            this.boundingRect = boundingRect;
            buildingTemplate.zones.ForEach((zone) => {
                Console.WriteLine(zone);
                roomZones.Add(ProcessedZoneFactory.ParseZone(boundingRect,zone));
            }); 
            rooms = new List<Room>();
        }
 
        public void PopulateWithFurniture(List<Template> templates, List<IPropCollection> propCollections)
        {
            var random = new System.Random();
            var furnitureLayoutGenerator = new FurnitureLayoutGenerator();
            Console.WriteLine("Room Zones: " + roomZones.Count);
            roomZones.ForEach((processedZone =>
            {
                var tag = processedZone.tags[random.Next(0, processedZone.tags.Count)];
                Console.WriteLine(tag);
                var filteredPropCollections = propCollections.FindAll((pc) => { 
                    return pc.tags.Contains(tag);
                });
                var room = new Room(processedZone.boundingRect, tag);
                var filteredTemplates = templates.FindAll((template) => {
                    return template.tags.Contains(tag);
                });
                if (filteredPropCollections.Count > 0 && filteredTemplates.Count > 0)
                {
              
                    var template = filteredTemplates[random.Next(0, filteredTemplates.Count)];
                    var propCollection = filteredPropCollections[random.Next(0, filteredPropCollections.Count)];
                    Console.WriteLine(propCollection.name);
                    room.props = furnitureLayoutGenerator.generateRoomLayoutBasedOnTemplate(room, propCollection,template);
             
                }
                rooms.Add(room);

            }));
        }
        
       
    }
}
