
using Dungeon_Generator_Core.Generator.Visual_Output;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dungeon_Generator_Core
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        public void drawDungeon (object sender, EventArgs e)
        {

            new DungeonDrawer().execute(pictureBox1);
        }

    }
}
