using System;
using System.Collections;
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

        private int width = 770 + 300;
        private int height = 790;
        private int sizeOfSides = 30;

        private int[,] map = new int[25, 25];

        private Enemy1[] enemyList;
        private int[,] enemyLocations;
        private int countOfEnemies = 0;

        private int[] characterDir = new int[2];

        Character character = new Character();

        string pathForBackground = System.IO.Path.GetFullPath(@"textures\blocks\waterfall-3.png");

        string pathForCharacterGoFont = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-front-2.png");
        string pathForCharacterGoBack = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-back-2.png");
        string pathForCharacterGoSideLeft = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-side-left-2.png");
        string pathForCharacterGoSideRight = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-side-right-2.png");

        string pathForFullHeart = System.IO.Path.GetFullPath(@"textures\hearts-1.png");
        string pathForEmptyHeart = System.IO.Path.GetFullPath(@"textures\hearts-2.png");

        string pathForEnemy1Font = System.IO.Path.GetFullPath(@"textures\enemies\enemy1\mole-walk-front-4.png");
        string pathForEnemy1Back = System.IO.Path.GetFullPath(@"textures\enemies\enemy1\mole-walk-back-4.png");
        string pathForEnemy1Left = System.IO.Path.GetFullPath(@"textures\enemies\enemy1\mole-walk-side-left-4.png");
        string pathForEnemy1Right = System.IO.Path.GetFullPath(@"textures\enemies\enemy1\mole-walk-side-right-4.png");

        private int currentUserId = 0;

        Timer enemyTimer;
        WorkWithXml xml = new WorkWithXml();

        public Form1(int id)
        {
            this.currentUserId = id;

            InitializeComponent();

            this.Width = width;
            this.Height = height;

            this.KeyDown += new KeyEventHandler(keyPress);

            GenerateWorld();

            enemyTimer = new Timer();
            enemyTimer.Tick += new EventHandler(moveEnemies);
            enemyTimer.Interval = 500;
            enemyTimer.Start();


        }

        private int level = 1;

        private void GameOver()
        {
            level = 1;
            timer.Stop();
            
            MessageBox.Show("You are dead", "Defeat");
            GenerateWorld();
        }

        private void getDamage()
        {
            for (int i = 0; i < enemyList.Length; i++)
            {
                if (enemyList[i] == null)
                {
                    break;
                }

                if (enemyList[i].sprite.Location == character.sprite.Location)
                {
                    character.HitPoints -= enemyList[i].damage;
                    createStats(level, character.killedEnemies, character.HitPoints);

                    if (character.HitPoints >= 0)
                    {
                        this.Controls.Remove(hearts[character.HitPoints / 50]);
                    }

                    if (character.HitPoints == 0)
                    {
                        GameOver();
                    }
                }
            }
        }

        private void killEnemy(PictureBox arrow)
        {
            for (int i = 0; i < enemyList.Length; i++)
            {
                if (arrow.Location == enemyList[i].sprite.Location)
                {
                    if ((enemyList[i].HitPoints - character.damage) == 0)
                    {
                        this.Controls.Remove(enemyList[i].sprite);

                        enemyList = enemyList.Where(item => item != enemyList[i]).ToArray();

                        countOfEnemies--;
                        character.killedEnemies++;
                        createStats(level, character.killedEnemies, character.HitPoints);

                        break;
                    } else
                    {
                        break;
                    }
                }
            }
        }

        PictureBox[] hearts = new PictureBox[6];

        Label levelLabel = new Label();
        Label killedEnemiesLabel = new Label();
        Label countOfEnemiesLabel = new Label();
        Label usernameLabel = new Label();

        private void createStats(int lev, int killed, int hp)
        {
            usernameLabel.Text = xml.GetUserById(currentUserId).ToString();
            usernameLabel.Size = new Size(100, 30);
            usernameLabel.Location = new Point(width - 250, 10);
            usernameLabel.Font = new Font(usernameLabel.Font.Name, 20, FontStyle.Bold);
            this.Controls.Add(usernameLabel);
            usernameLabel.Refresh();

            levelLabel.Text = "Рівень: " + lev.ToString();
            levelLabel.Location = new Point(width - 250, 70);
            levelLabel.Size = new Size(100, 25);
            levelLabel.Font = new Font(levelLabel.Font.Name, 14, FontStyle.Regular);
            this.Controls.Add(levelLabel);
            levelLabel.Refresh();
            
            killedEnemiesLabel.Text = "Вбито: " + killed.ToString();
            killedEnemiesLabel.Location = new Point(width - 250, 160);
            killedEnemiesLabel.Size = new Size(100, 25);
            killedEnemiesLabel.Font = new Font(killedEnemiesLabel.Font.Name, 12, FontStyle.Regular);
            this.Controls.Add(killedEnemiesLabel);
            killedEnemiesLabel.Refresh();

            countOfEnemiesLabel.Text = "Потрібно вбити: " + countOfEnemies.ToString();
            countOfEnemiesLabel.Location = new Point(width - 250, 185);
            countOfEnemiesLabel.Size = new Size(150, 25);
            countOfEnemiesLabel.Font = new Font(countOfEnemiesLabel.Font.Name, 12, FontStyle.Regular);
            this.Controls.Add(countOfEnemiesLabel);
            countOfEnemiesLabel.Refresh();
        }

        private void GenerateWorld()
        {
            countOfEnemies = 0;
            enemyTimer = new Timer();
            enemyTimer.Start();

            timer = new Timer();

            DestroyMap();
            GenerateEnemies();
            GenerateCharacter();
            GenerateMap();
            createStats(level, character.killedEnemies, character.HitPoints);

            background.Image = Image.FromFile(pathForBackground);
            background.Location = new Point(0, 0);
            background.Size = new Size(750, 750);
            background.SizeMode = PictureBoxSizeMode.StretchImage;
            background.BringToFront();
            this.Controls.Add(background);

            character.sprite.BackgroundImage = background.Image;
        }

        private void moveEnemies(Object myObject, EventArgs eventArgs)
        {
            for (int i = 0; i < enemyList.Length; i++)
            {
                if (enemyList[i] == null)
                {
                    break;
                }

                moveEnemy(sizeOfSides, sizeOfSides, enemyList[i].sprite, i);
            }
        }

        private void moveEnemy(int x, int y, PictureBox enemy, int id)
        {
            var directions = new int[4, 2] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

            var dx = character.sprite.Location.X - enemy.Location.X;
            var dy = character.sprite.Location.Y - enemy.Location.Y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (dx < 0)
                {
                    // left
                    if (isWall((x * directions[0, 0]), (y * directions[0, 1]), enemy))
                    {
                        enemy.Image = Image.FromFile(pathForEnemy1Left);
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

            for (int j = 0; j < enemyList.Length; j++)
            {
                if (enemyList[j].sprite.Location.X == enemy.Location.X && enemyList[j].sprite.Location.Y == enemy.Location.Y && j != id)
                {
                    this.Controls.Remove(enemyList[j].sprite);
                    enemyList = enemyList.Where(item => item != enemyList[j]).ToArray();

                    character.killedEnemies++;
                    countOfEnemies--;
                    createStats(level, character.killedEnemies, character.HitPoints);

                    break;

                } 
            }

            getDamage();
        }

        private void GenerateEnemies()
        {
            Random rnd = new Random();

            var places = new int[] { sizeOfSides * 2, sizeOfSides * 3, sizeOfSides * 4, sizeOfSides * 5, sizeOfSides * 6, sizeOfSides * 7, sizeOfSides * 8, sizeOfSides * 9, sizeOfSides * 10, sizeOfSides * 11, sizeOfSides * 12, sizeOfSides * 13, sizeOfSides * 14, sizeOfSides * 15, sizeOfSides * 16, sizeOfSides * 17, sizeOfSides * 18, sizeOfSides * 19, sizeOfSides * 20, sizeOfSides * 21, sizeOfSides * 22, sizeOfSides * 23 };

            enemyList = new Enemy1[rnd.Next(1 + level, 6 + level)];
            enemyLocations = new int[enemyList.Length, 2];

            for (int i = 0; i < enemyList.Length; i++)
            {
                Enemy1 enemy1 = new Enemy1();
                enemy1.sprite.Image = Image.FromFile(pathForEnemy1Back);
                enemy1.sprite.SizeMode = PictureBoxSizeMode.StretchImage;

                int num = rnd.Next(0, places.Length);
                for (int j = 0; j < enemyLocations.GetLength(0); j++)
                {
                    if (enemyLocations[j, 0] != places[num] && enemyLocations[j, 1] != places[num])
                    {
                        enemy1.sprite.Location = new Point(places[num], places[num]);

                        enemyLocations[i, 0] = enemy1.sprite.Location.X;
                        enemyLocations[i, 1] = enemy1.sprite.Location.Y;

                        break;
                    }
                    else
                    {
                        num = rnd.Next(0, places.Length);
                    }
                }

                enemy1.sprite.Size = new Size(sizeOfSides, sizeOfSides);

                enemy1.sprite.BackgroundImage = Image.FromFile(pathForBackground);

                enemy1.HitPoints = 100;

                this.Controls.Add(enemy1.sprite);

                countOfEnemies++;
                enemyList[i] = enemy1;
            }
        }

        Timer timer;

        private void shot()
        {
            int i = 0;

            string pathForArrow = System.IO.Path.GetFullPath(@"textures\arrow.png");

            PictureBox arrow = new PictureBox();
            arrow.Image = Image.FromFile(pathForArrow);
            arrow.SizeMode = PictureBoxSizeMode.StretchImage;

            arrow.Location = new Point(character.sprite.Location.X, character.sprite.Location.Y);

            if (characterDir[0] == -1)
            {
                arrow.Image.RotateFlip(RotateFlipType.Rotate90FlipX);
            }
            else if (characterDir[0] == 1)
            {
                arrow.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            else if (characterDir[1] == 1)
            {
                arrow.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }


            arrow.Size = new Size(sizeOfSides, sizeOfSides);
            this.Controls.Add(arrow);

            timer = new Timer();
            timer.Interval = 50;
            timer.Start();

            timer.Tick += new EventHandler((o, ev) =>
            {
                if (isWall(sizeOfSides * characterDir[0], sizeOfSides * characterDir[1], arrow))
                {
                    arrow.Location = new Point(arrow.Location.X + sizeOfSides * characterDir[0], arrow.Location.Y + sizeOfSides * characterDir[1]);
                    i++;
                    killEnemy(arrow);

                    arrow.BringToFront();
                    arrow.BackgroundImage = background.Image;

                    if (i > 8)
                    {
                        timer.Stop();
                        this.Controls.Remove(arrow);
                        i = 0;
                    }
                }
                else
                {
                    timer.Stop();
                    this.Controls.Remove(arrow);
                    i = 0;
                }
            });
        }

        private void GoDoors(int x, int y)
        {
            if (countOfEnemies <= 0)
            {
                level++;
                character.sprite.Location = new Point(300, 300);
                GenerateWorld();
            }
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
            if (map[(character.sprite.Location.X + x) / sizeOfSides, (character.sprite.Location.Y + y) / sizeOfSides] == 2)
            {
                GoDoors(x, y);
            }
            else if (isWall(x, y, character.sprite))
            {
                characterDir[0] = (x / sizeOfSides);
                characterDir[1] = (y / sizeOfSides);

                character.sprite.Image = Image.FromFile(path);
                character.sprite.Location = new Point(character.sprite.Location.X + x, character.sprite.Location.Y + y);
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

            character.sprite.Image = Image.FromFile(pathForCharacterGoBack);
            character.sprite.SizeMode = PictureBoxSizeMode.StretchImage;
            character.sprite.Location = new Point(300, 300);
            character.sprite.Size = new Size(sizeOfSides, sizeOfSides);

            character.HitPoints = 300;
            character.killedEnemies = 0;

            createStats(level, character.killedEnemies, character.HitPoints);

            for (int i = 0; i < character.HitPoints; i += 50)
            {
                PictureBox heart = new PictureBox();
                heart.Image = Image.FromFile(pathForFullHeart);
                heart.SizeMode = PictureBoxSizeMode.StretchImage;
                heart.Location = new Point(width - 250 + i / 2, 120);
                heart.Size = new Size(25, 25);

                hearts[i / 50] = heart;
            }

            for (int i = 0; i < 6; i++)
            {
                this.Controls.Add(hearts[i]);
            }

            this.Controls.Add(character.sprite);
        }

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
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2 && j != 0 && j != 24)
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
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2 && j != 0 && j != 24)
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
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2 && i != 0 && i != 24)
                        {
                            GenerateDoors(i, j);
                            map[i, j] = 2;

                            countOfDoors++;
                        }
                        else
                        {
                            PictureBox borderUp = new PictureBox();

                            borderUp.Image = Image.FromFile(path);
                            borderUp.SizeMode = PictureBoxSizeMode.StretchImage;

                            borderUp.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                            borderUp.Size = new Size(sizeOfSides, sizeOfSides);
                            this.Controls.Add(borderUp);

                            map[i, j] = 1;
                        }
                    }
                    else if (j == 24)
                    {
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2 && i != 0 && i != 24)
                        {
                            GenerateDoors(i, j);
                            map[i, j] = 2;

                            countOfDoors++;
                        }
                        else
                        {
                            if (countOfDoors == 0)
                            {
                                GenerateDoors(i, j);
                                map[i, j] = 2;

                                countOfDoors++;
                            }
                            else
                            {

                                PictureBox borderDown = new PictureBox();

                                borderDown.Image = Image.FromFile(path);
                                borderDown.SizeMode = PictureBoxSizeMode.StretchImage;

                                borderDown.Location = new Point(i * sizeOfSides, j * sizeOfSides);
                                borderDown.Size = new Size(sizeOfSides, sizeOfSides);
                                this.Controls.Add(borderDown);

                                map[i, j] = 1;
                            }
                        }

                    }
                    else
                    {
                        if (map[i - 1, j] != 2 && map[i + 1, j] != 2 && map[i, j + 1] != 2 && map[i, j - 1] != 2)
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
                            }
                            else
                            {
                                map[i, j] = 0;
                            }
                        }
                    }
                }
            }
        }
    }
}
