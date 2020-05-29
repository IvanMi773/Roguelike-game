using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace isaac
{
    public partial class Form1 : Form
    {
        private int width = 700;
        private int height = 700;

        private int sizeOfSides = 30;

        public Form1()
        {
            InitializeComponent();

            this.Width = width;
            this.Height = height;

            GenerateMap();
        }

        private void GenerateMap()
        {
            for (int i = 0; i <= width / sizeOfSides; i++)
            {
                PictureBox pic = new PictureBox();

                pic.BackColor = Color.Black;
                pic.Location = new Point(0, sizeOfSides * i);
                pic.Size = new Size(width, 1);
                this.Controls.Add(pic);
            }

            for (int i = 0; i <= height / sizeOfSides; i++)
            {
                PictureBox pic = new PictureBox();

                pic.BackColor = Color.Black;
                pic.Location = new Point(sizeOfSides * i, 0);
                pic.Size = new Size(1, height);
                this.Controls.Add(pic);
            }
        }
    }
}
