using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace isaac
{
    public partial class GameOverForm : Form
    {

        public GameOverForm()
        {
            InitializeComponent();

            SoundPlayer player = new SoundPlayer(@"sounds/game_over.wav");
            player.Play();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Visible = false;

            Data.redirect = true;
            this.Close();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            this.Visible = false;

            Data.redirect = false;
            

            this.Close();
        }
    }
}
