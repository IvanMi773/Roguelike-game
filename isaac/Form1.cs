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
        private int width = 706;
        private int height = 729;

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
            for (int i = 0; i < width / sizeOfSides; i++)
            {
                PictureBox pic = new PictureBox();

                pic.BackColor = Color.Black;
                pic.Location = new Point(0, sizeOfSides * i);
                pic.Size = new Size(width - 10, 1);
                this.Controls.Add(pic);

                PictureBox borderLeft = new PictureBox();

                string path = System.IO.Path.GetFullPath(@"textures\stonebricksmooth_cracked.png");
                borderLeft.Image = Image.FromFile(path);
                borderLeft.SizeMode = PictureBoxSizeMode.StretchImage;

                borderLeft.Location = new Point(0, sizeOfSides * i);
                borderLeft.Size = new Size(sizeOfSides, sizeOfSides);
                this.Controls.Add(borderLeft);

                PictureBox borderRight = new PictureBox();

                borderRight.Image = Image.FromFile(path);
                borderRight.SizeMode = PictureBoxSizeMode.StretchImage;

                borderRight.Location = new Point(661, sizeOfSides * i);
                borderRight.Size = new Size(sizeOfSides, sizeOfSides);
                this.Controls.Add(borderRight);
            }

            for (int i = 0; i < width / sizeOfSides; i++)
            {
                PictureBox pic = new PictureBox();

                pic.BackColor = Color.Black;
                pic.Location = new Point(sizeOfSides * i, 0);
                pic.Size = new Size(1, height - 20);
                this.Controls.Add(pic);

                PictureBox borderUp = new PictureBox();

                string path = System.IO.Path.GetFullPath(@"textures\stonebricksmooth_cracked.png");
                borderUp.Image = Image.FromFile(path);
                borderUp.SizeMode = PictureBoxSizeMode.StretchImage;

                borderUp.Location = new Point(sizeOfSides * i, 0);
                borderUp.Size = new Size(sizeOfSides, sizeOfSides);
                this.Controls.Add(borderUp);

                PictureBox borderDown = new PictureBox();

                borderDown.Image = Image.FromFile(path);
                borderDown.SizeMode = PictureBoxSizeMode.StretchImage;

                borderDown.Location = new Point(sizeOfSides * i, 661);
                borderDown.Size = new Size(sizeOfSides, sizeOfSides);
                this.Controls.Add(borderDown);
            }
        }
    }
}
