using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Dungeon_Generator_Core.Generator;
using System.Drawing;
using System.Data.Common;
using Dungeon_Generator_Core.Geometry;
using Dungeon_Generator_Core.Layout;
using System.Linq;
using System.Diagnostics;

namespace Dungeon_Generator_Core.Generator.Visual_Output
{

    using Point = Dungeon_Generator_Core.Geometry.Point;
    class DungeonDrawer
    {
        public void offsetRects (List<Rect> rects)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            rects.ForEach((r) =>
            {
                if (minX > r.min.X)
                {
                    minX = r.min.X;
                }
                if (minY > r.min.Y)
                {
                    minY = r.min.Y;
                }
            });

            for (var i = 0; i < rects.Count; i++)
            {
                var r = rects[i];
                rects[i].min = new Point(r.min.X + Math.Abs(minX), r.min.Y + Math.Abs(minY));
            }
        }
 
        public void drawSingleTile (Point p, Room room, SolidBrush brush, Pen pen, int factor, int offsetX, int offsetY, Graphics formGraphics)
        {
            formGraphics.FillRectangle(brush, new Rectangle(offsetX + p.X * factor, offsetY + p.Y * factor, factor, factor));
            //formGraphics.DrawRectangle(pen, new Rectangle(offsetX + p.X * factor, offsetY + p.Y * factor, factor, factor));

    

            if (! room.points.Contains(p + new Point(0, 1)))
            {
                var p1 = p + new Point(0, 1);
                var p2 = p + new Point(1, 1);

                formGraphics.DrawLine(pen, new System.Drawing.Point(offsetX + factor * p1.X, offsetY + factor * p1.Y), new System.Drawing.Point(offsetX + factor * p2.X, offsetY + factor * p2.Y));
            }

            if (!room.points.Contains(p + new Point(0, -1)))
            {
                var p1 = p + new Point(0, 0);
                var p2 = p + new Point(1, 0);

                formGraphics.DrawLine(pen, new System.Drawing.Point(offsetX + factor * p1.X, offsetY + factor * p1.Y), new System.Drawing.Point(offsetX + factor * p2.X, offsetY + factor * p2.Y));
            }
            if (!room.points.Contains(p + new Point(1, 0)))
            {
                var p1 = p + new Point(1, 0);
                var p2 = p + new Point(1, 1);

                formGraphics.DrawLine(pen, new System.Drawing.Point(offsetX + factor * p1.X, offsetY + factor * p1.Y), new System.Drawing.Point(offsetX + factor * p2.X, offsetY + factor * p2.Y));
            }
            if (!room.points.Contains(p + new Point(-1,0)))
            {
                var p1 = p + new Point(0, 0);
                var p2 = p + new Point(0, 1);

                formGraphics.DrawLine(pen, new System.Drawing.Point(offsetX + factor * p1.X, offsetY + factor * p1.Y), new System.Drawing.Point(offsetX + factor * p2.X, offsetY + factor * p2.Y));
            }
        }
        
        public void drawSingleRoom(PictureBox form)
        {
            form.Image = null;
            form.Update();
            var rooms = new HashSet<Room>(new Room[] { new Room(new Rect(new Point(0, 0), 10, 10), "room"), new Room(new Rect(new Point(0, 0), 10, 10), "room") }).ToList();
            Debug.WriteLine(rooms.Count);
            Pen border = new Pen(ColorTranslator.FromHtml("#10161A"));


            var palette = new Dictionary<string, SolidBrush> {
                { "room", new System.Drawing.SolidBrush(ColorTranslator.FromHtml("#5C7080"))},
                { "path" , new System.Drawing.SolidBrush(ColorTranslator.FromHtml("#BFCCD6"))},
                { "center" , new SolidBrush(ColorTranslator.FromHtml("#CED9E0"))},
                { "entrance", new SolidBrush(ColorTranslator.FromHtml("#5642A6")) }
            };

            Graphics formGraphics = form.CreateGraphics();

            var factor = 5;
            var offsetX = 100;
            var offsetY = 100;

           
            rooms.ForEach((r) => {
                foreach(Point p in r.points)
                {
                    drawSingleTile(p, r, palette[r.category], border, factor, offsetX, offsetY, formGraphics);
                }
                foreach (Point p in r.entrances)
                {
                    drawSingleTile(p, r, palette["entrance"], border, factor, offsetX, offsetY, formGraphics);
                }
 
                if (r.category == "room")
                {
                    var props = new FurnitureLayoutGenerator().generateRoomLayout(r);
                    props.ForEach((propData) => {
                        var position = propData.position;
                        var prop = propData.prop;
                        var brush = new SolidBrush(prop.color);
                        for (var i = 0; i < prop.width; i++)
                        {
                            for (var j = 0; j < prop.height; j++)
                            {
                                drawSingleTile(new Point(i, j) + position, r, brush, border, factor, offsetX, offsetY, formGraphics);
                            }
                        }
                    });
                }


            });
        }
        
        public void execute(PictureBox form)
        {
            form.Image = null;
            form.Update();
            Pen border = new Pen(ColorTranslator.FromHtml("#10161A"));
            var palette = new Dictionary<string, SolidBrush> {
                { "room", new System.Drawing.SolidBrush(ColorTranslator.FromHtml("#5C7080"))},
                { "path" , new System.Drawing.SolidBrush(ColorTranslator.FromHtml("#BFCCD6"))},
                { "center" , new SolidBrush(ColorTranslator.FromHtml("#CED9E0"))},
                { "entrance", new SolidBrush(ColorTranslator.FromHtml("#5642A6")) },
                { "wall", new SolidBrush(ColorTranslator.FromHtml("#293742")) }
            };
            Graphics formGraphics = form.CreateGraphics();

            var factor = 5;
            var offsetX = 100;
            var offsetY = 100;


            var templateResults = new DungeonLayout().execute(0, 0);
            var rooms = new RoomLayoutManager().execute(templateResults);


            rooms.ForEach((r) => {

                var count = rooms.FindAll(r.Equals).Count;
                foreach (Point p in r.innerPoints)
                {
                    drawSingleTile(p, r, palette[r.category], border, factor, offsetX, offsetY, formGraphics);
                }
                foreach (Point p in r.edgePoints)
                {
                    drawSingleTile(p, r, palette["wall"], border, factor, offsetX, offsetY, formGraphics);
                }
                foreach (Point p in r.entrances)
                {
                    drawSingleTile(p, r, palette["entrance"], border, factor, offsetX, offsetY, formGraphics);
                }

                if (r.category == "room")
                {
                    var props = new FurnitureLayoutGenerator().generateRoomLayout(r);
                    props.ForEach((propData) => {
                        var position = propData.position;
                        var prop = propData.prop;
                        var brush = new SolidBrush(prop.color);
                        for (var i = 0; i < prop.width; i++)
                        {
                            for (var j = 0; j < prop.height; j++)
                            {
                                drawSingleTile(new Point(i, j) + position, r, brush, border, factor, offsetX, offsetY, formGraphics);
                            }
                        }
                    });
                }
            });
        }
    }
}
