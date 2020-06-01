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
        private PictureBox[] enemyList = new PictureBox[30];

        PictureBox character = new PictureBox();

        private int level = 0;

        string pathForBackground = System.IO.Path.GetFullPath(@"textures\blocks\waterfall-1.png");

        string pathForCharacterGoFont = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-front-2.png");
        string pathForCharacterGoBack = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-back-2.png");
        string pathForCharacterGoSideLeft = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-side-left-2.png");
        string pathForCharacterGoSideRight = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-side-right-2.png");

        string pathForCharacterShotFont = System.IO.Path.GetFullPath(@"textures\hero\attack\hero-attack-front-weapon-3.png");
        string pathForCharacterShotBack = System.IO.Path.GetFullPath(@"textures\hero\attack\hero-attack-back-weapon-2.png");
        string pathForCharacterShotSideLeft = System.IO.Path.GetFullPath(@"textures\hero\attack\hero-attack-side-left-weapon-3.png");
        string pathForCharacterShotSideRight = System.IO.Path.GetFullPath(@"textures\hero\attack\hero-attack-side-right-weapon-3.png");

        string pathForEnemy1Font = System.IO.Path.GetFullPath(@"textures\enemies\enemy1\mole-walk-front-4.png");
        string pathForEnemy1Back = System.IO.Path.GetFullPath(@"textures\enemies\enemy1\mole-walk-back-4.png");
        string pathForEnemy1Left = System.IO.Path.GetFullPath(@"textures\enemies\enemy1\mole-walk-side-left-4.png");
        string pathForEnemy1Right = System.IO.Path.GetFullPath(@"textures\enemies\enemy1\mole-walk-side-right-4.png");

        public Form1()
        {
            InitializeComponent();

            this.Width = width;
            this.Height = height;

            this.KeyDown += new KeyEventHandler(keyPress);

            GenerateWorld();

            Timer timer = new Timer();
            timer.Tick += new EventHandler(moveEnemies);
            timer.Interval = 250;
            timer.Start();
        }

        private void GenerateWorld()
        {
            DestroyMap();
            GenerateEnemies();
            GenerateCharacter();
            GenerateMap();

            background.Image = Image.FromFile(pathForBackground);
            background.Location = new Point(0, 0);
            background.Size = new Size(750, 750);
            background.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(background);

            character.BackgroundImage = background.Image;
        }

        private void moveEnemies(Object myObject, EventArgs eventArgs)
        {
            foreach (PictureBox enemy in enemyList)
            {
                if (enemy == null)
                {
                    break;
                }
                moveEnemy(sizeOfSides, sizeOfSides, enemy);
            }
        }

        private void moveEnemy(int x, int y, PictureBox enemy)
        {
            var directions = new int[4, 2] { { -1, 0}, { 1, 0}, { 0, -1 }, { 0, 1 } };

            var dx = character.Location.X - enemy.Location.X;
            var dy = character.Location.Y - enemy.Location.Y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (dx < 0)
                {
                    // left
                    if (isWall((x * directions[0, 0]), (y * directions[0, 1]), enemy))
                    {
                        enemy.Image = Image.FromFile(pathForEnemy1Left);
                        enemy.Location = new Point(enemy.Location.X + (directions[0, 0] * x), enemy.Location.Y + (directions[0, 1] * y));
                    } else
                    {
                        enemy.Location = new Point(enemy.Location.X + (directions[0, 0] * x), enemy.Location.Y + (directions[0, 1] * y));
                    }
                }
                else
                {
                    // right
                    if (isWall((x * directions[1, 0]), (y * directions[1, 1]), enemy))
                    {
                        enemy.Image = Image.FromFile(pathForEnemy1Right);
                        enemy.Location = new Point(enemy.Location.X + (directions[1, 0] * x), enemy.Location.Y + (directions[1, 1] * y));
                    }
                }
            }
            else
            {
                if (dy < 0)
                {
                    // up
                    if (isWall((x * directions[2, 0]), (y * directions[2, 1]), enemy))
                    {
                        enemy.Image = Image.FromFile(pathForEnemy1Back);
                        enemy.Location = new Point(enemy.Location.X + (directions[2, 0] * x), enemy.Location.Y + (directions[2, 1] * y));
                    }
                }
                else
                {
                    // down
                    if (isWall((x * directions[3, 0]), (y * directions[3, 1]), enemy))
                    {
                        enemy.Image = Image.FromFile(pathForEnemy1Font);
                        enemy.Location = new Point(enemy.Location.X + (directions[3, 0] * x), enemy.Location.Y + (directions[3, 1] * y));
                    }
                }
            }            
        }

        private void GenerateEnemies()
        {
            for(int i = 0; i < enemyList.Length; i++)
            {
                if (enemyList[i] == null)
                {
                    break;
                }
                enemyList[i] = null;
            }

            Random rnd = new Random();

            var places = new int[] { 60, 90, 120, 150, 180, 210 };

            for (int i = 0; i <= rnd.Next(1, 11); i++)
            {
                PictureBox enemy1 = new PictureBox();

                enemy1.Image = Image.FromFile(pathForEnemy1Back);
                enemy1.SizeMode = PictureBoxSizeMode.StretchImage;
                enemy1.Location = new Point(places[rnd.Next(0, 6)], places[rnd.Next(0, 6)]);
                enemy1.Size = new Size(sizeOfSides, sizeOfSides);

                enemy1.BackgroundImage = Image.FromFile(pathForBackground);

                this.Controls.Add(enemy1);

                enemyList[i] = enemy1;
            }
        }

        private void shot()
        {
            string pathForArrow = System.IO.Path.GetFullPath(@"textures\arrow.png");
            PictureBox arrow = new PictureBox();

            arrow.Image = Image.FromFile(pathForArrow);
            arrow.SizeMode = PictureBoxSizeMode.StretchImage;
            arrow.Location = new Point(character.Location.X + (sizeOfSides), character.Location.Y + (sizeOfSides));
            arrow.Size = new Size(sizeOfSides, sizeOfSides);
            this.Controls.Add(arrow);

            for (int i = 0; i < 6; i++)
            {
                arrow.Location = new Point(arrow.Location.X + (sizeOfSides / 2), arrow.Location.Y + (sizeOfSides / 2));
            }

            //this.Controls.Remove(arrow);
            label1.Text = "remov";

            //if (character.Image == Image.FromFile(pathForCharacterGoBack))
            //{
            //    character.Image = Image.FromFile(pathForCharacterShotBack);
            //} else if (character.Image == Image.FromFile(pathForCharacterGoFont))
            //{
            //    character.Image = Image.FromFile(pathForCharacterShotFont);
            //}
            //else if (character.Image == Image.FromFile(pathForCharacterGoSideLeft))
            //{
            //    character.Image = Image.FromFile(pathForCharacterShotSideLeft);
            //}
            //else if (character.Image == Image.FromFile(pathForCharacterGoSideRight))
            //{
            //    character.Image = Image.FromFile(pathForCharacterShotSideRight);
            //}
        }

        private void GoDoors()
        {
            character.Location = new Point(300, 300);
            //DestroyMap();
            GenerateWorld();
        }

        private bool isWall(int x, int y, PictureBox person)
        {
            if (map[(person.Location.X + x) / sizeOfSides, (person.Location.Y + y) / sizeOfSides] == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void moveCharacter(int x, int y, string path)
        {
            if (map[(character.Location.X) / sizeOfSides, (character.Location.Y) / sizeOfSides] == 2)
            {
                GoDoors();

            } else if (isWall(x, y, character))
            {
                character.Image = Image.FromFile(path);
                character.Location = new Point(character.Location.X + x, character.Location.Y + y);
            }
        }

        private void keyPress(object sender, KeyEventArgs e)
        {
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

                case 32:
                    shot();
                    break;
            }
        }

        private void GenerateCharacter()
        {
            string pathForCharacterGoBack = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-back-2.png");

            character.Image = Image.FromFile(pathForCharacterGoBack);
            character.SizeMode = PictureBoxSizeMode.StretchImage;
            character.Location = new Point(300, 300);
            character.Size = new Size(sizeOfSides, sizeOfSides);

            this.Controls.Add(character);
        }

        Random rnd = new Random();

        private void GenerateDoors(int i, int j)
        {
            PictureBox door = new PictureBox();

            door.Image = Image.FromFile(pathForBackground);
            door.SizeMode = PictureBoxSizeMode.StretchImage;

            door.Location = new Point(i * sizeOfSides, j * sizeOfSides);
            door.Size = new Size(sizeOfSides, sizeOfSides);
            this.Controls.Add(door);
        }

        private void DestroyMap()
        {
            //for (int i = 0; i < 25; i++)
            //{
            //    for (int j = 0; j < 25; j++)
            //    {
            //        w.Write(map[i, j] + " ");
            //    }
            //    w.Write("\n");
            //}
            Array.Clear(map, 0, map.Length);
            this.Controls.Clear();
        }

        private void GenerateMap()
        {
            string path = System.IO.Path.GetFullPath(@"textures\blocks\stonebricksmooth_cracked.png");
            string pathForDirt = System.IO.Path.GetFullPath(@"textures\blocks\dirt.png");
            string pathForStoneMoss = System.IO.Path.GetFullPath(@"textures\blocks\stoneMoss.png");

            Random rnd = new Random();
            int countOfDoors = 0;

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    if (i == 0)
                    {
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2)
                        {
                            GenerateDoors(i, j);
                            map[i, j] = 2;

                            countOfDoors++;
                        }
                        else
                        {
                            PictureBox borderLeft = new PictureBox();

                            borderLeft.Image = Image.FromFile(path);
                            borderLeft.SizeMode = PictureBoxSizeMode.StretchImage;

                            borderLeft.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                            borderLeft.Size = new Size(sizeOfSides, sizeOfSides);
                            this.Controls.Add(borderLeft);

                            map[i, j] = 1;
                        }
                    }
                    else if (i == 24)
                    {
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2)
                        {
                            GenerateDoors(i, j);
                            map[i, j] = 2;

                            countOfDoors++;
                        }
                        else
                        {
                            PictureBox borderLeft = new PictureBox();

                            borderLeft.Image = Image.FromFile(path);
                            borderLeft.SizeMode = PictureBoxSizeMode.StretchImage;

                            borderLeft.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                            borderLeft.Size = new Size(sizeOfSides, sizeOfSides);
                            this.Controls.Add(borderLeft);

                            map[i, j] = 1;
                        }
                    }
                    else if (j == 0)
                    {
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2)
                        {
                            GenerateDoors(i, j);
                            map[i, j] = 2;

                            countOfDoors++;
                        } else 
                        {
                            PictureBox borderUp = new PictureBox();

                            borderUp.Image = Image.FromFile(path);
                            borderUp.SizeMode = PictureBoxSizeMode.StretchImage;

                            borderUp.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                            borderUp.Size = new Size(sizeOfSides, sizeOfSides);
                            this.Controls.Add(borderUp);

                            map[i, j] = 1;
                        }
                    } else if (j == 24)
                    {
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2)
                        {
                            GenerateDoors(i, j);
                            map[i, j] = 2;

                            countOfDoors++;
                        } else
                        {
                            PictureBox borderDown = new PictureBox();

                            borderDown.Image = Image.FromFile(path);
                            borderDown.SizeMode = PictureBoxSizeMode.StretchImage;

                            borderDown.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                            borderDown.Size = new Size(sizeOfSides, sizeOfSides);
                            this.Controls.Add(borderDown);

                            map[i, j] = 1;
                        }

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
                    }
                }
            }

            //using( StreamWriter w = new StreamWriter(@"file.txt"))
            //{
            //    for (int i = 0; i < 24; i++)
            //    {
            //        for (int j = 0; j < 24; j++)
            //        {
            //            w.Write(map[i, j] + " ");
            //        }
            //        w.Write("\n");
            //    }
            //}
        }
    }
}
