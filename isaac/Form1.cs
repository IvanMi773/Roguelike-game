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
        private int width = 770;
        private int height = 790;

        private int sizeOfSides = 30;

        PictureBox character = new PictureBox();

        public Form1()
        {
            InitializeComponent();

            this.Width = width;
            this.Height = height;

            this.KeyDown += new KeyEventHandler(keyPress);

            GenerateCharacter();
            GenerateMap();
        }

        private void checkBorders(int x, int y)
        {
            if (character.Location.X < 0 + sizeOfSides)
            {
                moveCharacter(sizeOfSides / 2, 0);
            } else if (character.Location.X > 23 * sizeOfSides + (2 / sizeOfSides))
            {
                moveCharacter(-sizeOfSides / 2, 0);
            } else if (character.Location.Y < 0 + sizeOfSides)
            {
                moveCharacter(0, sizeOfSides / 2);
            } else if (character.Location.Y > 23 * sizeOfSides + (2 / sizeOfSides))
            {
                moveCharacter(0, -sizeOfSides / 2);
            } else
            {
                moveCharacter(x, y);
            }
        }

        private void moveCharacter(int x, int y)
        {
            character.Location = new Point(character.Location.X + x, character.Location.Y + y);
        }

        private void keyPress(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 68:
                    checkBorders(sizeOfSides / 2, 0);
                    break;

                case 65:
                    checkBorders(-(sizeOfSides / 2), 0);
                    break;

                case 87:
                    checkBorders(0, - (sizeOfSides / 2));
                    break;

                case 83:
                    checkBorders(0, (sizeOfSides / 2));
                    break;
            }
        }

        private void GenerateCharacter()
        {
            string pathForCharacter = System.IO.Path.GetFullPath(@"textures\hero-walk-front-1.png");

            character.Image = Image.FromFile(pathForCharacter);
            character.SizeMode = PictureBoxSizeMode.StretchImage;

            character.Location = new Point(300, 300);
            character.Size = new Size(sizeOfSides, sizeOfSides);

            this.Controls.Add(character);
        }

        private void GenerateMap()
        {
            string path = System.IO.Path.GetFullPath(@"textures\stonebricksmooth_cracked.png");
            string pathForDirt = System.IO.Path.GetFullPath(@"textures\dirt.png");
            string pathForStoneMoss = System.IO.Path.GetFullPath(@"textures\stoneMoss.png");

            Random rnd = new Random();

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    if (i == 0)
                    {
                        PictureBox borderLeft = new PictureBox();

                        borderLeft.Image = Image.FromFile(path);
                        borderLeft.SizeMode = PictureBoxSizeMode.StretchImage;

                        borderLeft.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                        borderLeft.Size = new Size(sizeOfSides, sizeOfSides);
                        this.Controls.Add(borderLeft);
                    } else if (i == 24)
                    {
                        PictureBox borderLeft = new PictureBox();

                        borderLeft.Image = Image.FromFile(path);
                        borderLeft.SizeMode = PictureBoxSizeMode.StretchImage;

                        borderLeft.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                        borderLeft.Size = new Size(sizeOfSides, sizeOfSides);
                        this.Controls.Add(borderLeft);
                    } else if (j == 0)
                    {
                        PictureBox borderUp = new PictureBox();

                        borderUp.Image = Image.FromFile(path);
                        borderUp.SizeMode = PictureBoxSizeMode.StretchImage;

                        borderUp.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                        borderUp.Size = new Size(sizeOfSides, sizeOfSides);
                        this.Controls.Add(borderUp);
                    } else if (j == 24)
                    {
                        PictureBox borderDown = new PictureBox();

                        borderDown.Image = Image.FromFile(path);
                        borderDown.SizeMode = PictureBoxSizeMode.StretchImage;

                        borderDown.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                        borderDown.Size = new Size(sizeOfSides, sizeOfSides);
                        this.Controls.Add(borderDown);
                    } else
                    {
                        if (rnd.Next(1, 100) > 40)
                        {
                            PictureBox dirt = new PictureBox();

                            dirt.Image = Image.FromFile(pathForStoneMoss);
                            dirt.SizeMode = PictureBoxSizeMode.StretchImage;

                            dirt.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                            dirt.Size = new Size(sizeOfSides, sizeOfSides);
                            this.Controls.Add(dirt);
                        }
                        else
                        {
                            PictureBox dirt = new PictureBox();

                            dirt.Image = Image.FromFile(pathForDirt);
                            dirt.SizeMode = PictureBoxSizeMode.StretchImage;

                            dirt.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                            dirt.Size = new Size(sizeOfSides, sizeOfSides);
                            this.Controls.Add(dirt);
                        }
                    }
                }
            }
        }
    }
}
