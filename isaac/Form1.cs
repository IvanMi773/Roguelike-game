using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace isaac
{
    public partial class Form1 : Form
    {
        PictureBox background = new PictureBox();

        private int width = 770;
        private int height = 790;

        private int sizeOfSides = 30;
        private int[,] map = new int[25, 25];

        PictureBox character = new PictureBox();

        public Form1()
        {
            InitializeComponent();


            this.Width = width;
            this.Height = height;

            this.KeyDown += new KeyEventHandler(keyPress);

            GenerateCharacter();
            GenerateMap();

            background.Image = Image.FromFile(System.IO.Path.GetFullPath(@"textures\waterfall-1.png"));
            background.Location = new Point(0, 0);
            background.Size = new Size(750, 750);
            background.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(background);

            character.Parent = background;
            character.BackgroundImage = background.Image;
        }

        private bool isWall(int x, int y)
        {
            if (map[(character.Location.X + x) / (sizeOfSides), (character.Location.Y + y) / sizeOfSides] == 1)
            {
                return false;
            } else
            {
                return true;
            }
        }

        private void moveCharacter(int x, int y, string path)
        {
            if (isWall(x, y))
            {
                character.Image = Image.FromFile(path);
                character.Location = new Point(character.Location.X + x, character.Location.Y + y);
            }
        }

        private void keyPress(object sender, KeyEventArgs e)
        {
            string pathForCharacterGoFont = System.IO.Path.GetFullPath(@"textures\hero-walk-front-2.png");
            string pathForCharacterGoBack = System.IO.Path.GetFullPath(@"textures\hero-walk-back-2.png");
            string pathForCharacterGoSideLeft = System.IO.Path.GetFullPath(@"textures\hero-walk-side-left-2.png");
            string pathForCharacterGoSideRight = System.IO.Path.GetFullPath(@"textures\hero-walk-side-right-2.png");

            switch (e.KeyValue)
            {
                case 68:
                    moveCharacter(sizeOfSides, 0, pathForCharacterGoSideRight);
                    break;

                case 65:
                    moveCharacter(-sizeOfSides, 0, pathForCharacterGoSideLeft);
                    break;

                case 87:
                    moveCharacter(0, -sizeOfSides, pathForCharacterGoBack);
                    break;

                case 83:
                    moveCharacter(0, sizeOfSides, pathForCharacterGoFont);
                    break;
            }
        }

        private void GenerateCharacter()
        {
            string pathForCharacterGoBack = System.IO.Path.GetFullPath(@"textures\hero-walk-back-2.png");

            character.Image = Image.FromFile(pathForCharacterGoBack);
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

                        map[i, j] = 1;
                    } else if (i == 24)
                    {
                        PictureBox borderLeft = new PictureBox();

                        borderLeft.Image = Image.FromFile(path);
                        borderLeft.SizeMode = PictureBoxSizeMode.StretchImage;

                        borderLeft.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                        borderLeft.Size = new Size(sizeOfSides, sizeOfSides);
                        this.Controls.Add(borderLeft);

                        map[i, j] = 1;
                    } else if (j == 0)
                    {
                        PictureBox borderUp = new PictureBox();

                        borderUp.Image = Image.FromFile(path);
                        borderUp.SizeMode = PictureBoxSizeMode.StretchImage;

                        borderUp.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                        borderUp.Size = new Size(sizeOfSides, sizeOfSides);
                        this.Controls.Add(borderUp);

                        map[i, j] = 1;
                    } else if (j == 24)
                    {
                        PictureBox borderDown = new PictureBox();

                        borderDown.Image = Image.FromFile(path);
                        borderDown.SizeMode = PictureBoxSizeMode.StretchImage;

                        borderDown.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                        borderDown.Size = new Size(sizeOfSides, sizeOfSides);
                        this.Controls.Add(borderDown);

                        map[i, j] = 1;
                    } else
                    {
                        if (rnd.Next(1, 100) < 10)
                        {
                            PictureBox dirt = new PictureBox();

                            dirt.Image = Image.FromFile(pathForStoneMoss);
                            dirt.SizeMode = PictureBoxSizeMode.StretchImage;

                            dirt.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                            dirt.Size = new Size(sizeOfSides, sizeOfSides);
                            this.Controls.Add(dirt);

                            map[i, j] = 1;
                        } else
                        {
                            map[i, j] = 0;
                        }
                        
                        //else
                        //{
                        //PictureBox dirt = new PictureBox();

                        //dirt.Image = Image.FromFile(pathForDirt);
                        //dirt.SizeMode = PictureBoxSizeMode.StretchImage;

                        //dirt.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                        //dirt.Size = new Size(sizeOfSides, sizeOfSides);
                        //this.Controls.Add(dirt);
                        //}
                    }
                }
            }
        }
    }
}
