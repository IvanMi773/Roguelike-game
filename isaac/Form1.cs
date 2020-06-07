using isaac.Memento;
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

        Map Map = new Map();
        MapMemento MapState;

        Enemy1[] enemyList;
        EnemyMemento[] enemyStateList;
        private int countOfEnemies = 0;

        private int[] characterDir = new int[2];

        Character character = new Character();
        HeroMemento characterState;

        GameHistory history = new GameHistory();
        GameMemento GameState;

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

        string pathForEnemy2Font = System.IO.Path.GetFullPath(@"textures\enemies\enemy2\treant-walk-front-3.png");
        string pathForEnemy2Back = System.IO.Path.GetFullPath(@"textures\enemies\enemy2\treant-walk-back-3.png");
        string pathForEnemy2Left = System.IO.Path.GetFullPath(@"textures\enemies\enemy2\treant-walk-left-3.png");
        string pathForEnemy2Right = System.IO.Path.GetFullPath(@"textures\enemies\enemy2\treant-walk-right-3.png");

        private int currentUserId = 0;

        Timer enemyTimer;
        WorkWithXml xml = new WorkWithXml("users.xml");

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
            character.killedEnemies = 0;
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
                    createStats(level, character.killedEnemies);
                    characterState = character.SaveState();

                    if (character.HitPoints >= 0)
                    {
                        if (enemyList[i].Name == "enemy1")
                        {
                            this.Controls.Remove(hearts[character.HitPoints / 50]);
                        } else
                        {
                            this.Controls.Remove(hearts[character.HitPoints / 50]);
                            this.Controls.Remove(hearts[(character.HitPoints + 50) / 50]);
                        }
                    }

                    if (character.HitPoints == 0)
                    {
                        GameOver();
                    }
                }
            }
        }

        private void removeEnemy(Enemy1 enemy)
        {
            this.Controls.Remove(enemy.sprite);

            enemyList = enemyList.Where(item => item != enemy).ToArray();
            enemyStateList = enemyStateList.Where(item => item.id != enemy.id).ToArray();

            countOfEnemies--;
            character.killedEnemies++;
            createStats(level, character.killedEnemies);
        }

        private void killEnemy(PictureBox arrow)
        {
            for (int i = 0; i < enemyList.Length; i++)
            {
                if (arrow.Location == enemyList[i].sprite.Location)
                {
                    enemyList[i].HitPoints -= character.damage;

                    if (enemyList[i].HitPoints == 0)
                    {
                        removeEnemy(enemyList[i]);
                        characterState = character.SaveState(); 
                        GameState.SetState(level, countOfEnemies, xml.GetUserById(currentUserId));

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

        ListBox list = new ListBox();

        Button btn = new Button();

        private void createStats(int lev, int killed)
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

            btn.Location = new Point(width - 250, 600);
            btn.Click += new EventHandler((o, ev) => {
                history.CreateGameSave(characterState, enemyStateList, MapState, GameState);
            });
            this.Controls.Add(btn);

            //list.Items.Clear();

            //list.Location = new Point(width - 250, 250);
            //list.Size = new Size(200, 200);
            //if (characterState != null)
            //{
            //    list.Items.Add(characterState.HitPoints);
            //    list.Items.Add(characterState.KilledEnemies);
            //}
            //this.Controls.Add(list);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GenerateWorld()
        {
            countOfEnemies = 0;
            enemyTimer = new Timer();
            enemyTimer.Start();

            timer = new Timer();

            GameState = new GameMemento(level, countOfEnemies, xml.GetUserById(currentUserId));

            DestroyMap();
            GenerateEnemies();
            GenerateCharacter();
            GenerateMap();
            createStats(level, character.killedEnemies);

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

                moveEnemy(sizeOfSides, sizeOfSides, enemyList[i], i);
            }
        }

        private void moveEnemy(int x, int y, Enemy1 enemy, int id)
        {
            var directions = new int[4, 2] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

            var dx = character.sprite.Location.X - enemy.sprite.Location.X;
            var dy = character.sprite.Location.Y - enemy.sprite.Location.Y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (dx < 0)
                {
                    // left
                    if (isWall((x * directions[0, 0]), (y * directions[0, 1]), enemy.sprite))
                    {
                        if (enemy.Name == "enemy1")
                        {
                            enemy.sprite.Image = Image.FromFile(pathForEnemy1Left);
                        } else
                        {
                            enemy.sprite.Image = Image.FromFile(pathForEnemy2Left);
                        }

                        enemy.sprite.Location = new Point(enemy.sprite.Location.X + (directions[0, 0] * x), enemy.sprite.Location.Y + (directions[0, 1] * y));
                    }
                }
                else
                {
                    // right
                    if (isWall((x * directions[1, 0]), (y * directions[1, 1]), enemy.sprite))
                    {
                        if (enemy.Name == "enemy1")
                        {
                            enemy.sprite.Image = Image.FromFile(pathForEnemy1Right);
                        }
                        else
                        {
                            enemy.sprite.Image = Image.FromFile(pathForEnemy2Right);
                        }

                        enemy.sprite.Location = new Point(enemy.sprite.Location.X + (directions[1, 0] * x), enemy.sprite.Location.Y + (directions[1, 1] * y));
                    }
                }
            }
            else
            {
                if (dy < 0)
                {
                    // up
                    if (isWall((x * directions[2, 0]), (y * directions[2, 1]), enemy.sprite))
                    {
                        if (enemy.Name == "enemy1")
                        {
                            enemy.sprite.Image = Image.FromFile(pathForEnemy1Back);
                        }
                        else
                        {
                            enemy.sprite.Image = Image.FromFile(pathForEnemy2Back);
                        }

                        enemy.sprite.Location = new Point(enemy.sprite.Location.X + (directions[2, 0] * x), enemy.sprite.Location.Y + (directions[2, 1] * y));
                    }
                }
                else
                {
                    // down
                    if (isWall((x * directions[3, 0]), (y * directions[3, 1]), enemy.sprite))
                    {
                        if (enemy.Name == "enemy1")
                        {
                            enemy.sprite.Image = Image.FromFile(pathForEnemy1Font);
                        }
                        else
                        {
                            enemy.sprite.Image = Image.FromFile(pathForEnemy2Font);
                        }

                        enemy.sprite.Location = new Point(enemy.sprite.Location.X + (directions[3, 0] * x), enemy.sprite.Location.Y + (directions[3, 1] * y));
                    }
                }
            }

            for (int j = 0; j < enemyList.Length; j++)
            {
                if (enemyList[j].sprite.Location.X == enemy.sprite.Location.X && enemyList[j].sprite.Location.Y == enemy.sprite.Location.Y && j != id)
                {
                    removeEnemy(enemyList[j]);

                    break;
                } 
            }

            getDamage();
        }

        private void GenerateEnemies()
        {
            int[,] enemyLocations;

            Random rnd = new Random();
            Random rndX = new Random();
            Random rndY = new Random();

            var places = new int[] { sizeOfSides * 2, sizeOfSides * 3, sizeOfSides * 4, sizeOfSides * 5, sizeOfSides * 6, sizeOfSides * 7, sizeOfSides * 8, sizeOfSides * 9, sizeOfSides * 10, sizeOfSides * 11, sizeOfSides * 12, sizeOfSides * 13, sizeOfSides * 14, sizeOfSides * 15, sizeOfSides * 16, sizeOfSides * 17, sizeOfSides * 18, sizeOfSides * 19, sizeOfSides * 20, sizeOfSides * 21, sizeOfSides * 22, sizeOfSides * 23 };

            int enemy1HitPoints = 100;
            int enemy1Damage = 50;

            int enemy2HitPoints = 300;
            int enemy2Damage = 100;

            int id = 1;

            enemyList = new Enemy1[rnd.Next(1 + level, 6 + level)];
            enemyStateList = new EnemyMemento[enemyList.Length];
            enemyLocations = new int[enemyList.Length, 2];

            for (int i = 0; i < enemyList.Length; i++)
            {
                Enemy1 enemy;

                if (rnd.Next(1, 10) == 1)
                {
                    PictureBox enemy2Sprite = new PictureBox();
                    enemy2Sprite.Image = Image.FromFile(pathForEnemy2Back);
                    enemy2Sprite.SizeMode = PictureBoxSizeMode.StretchImage;
                    enemy = new Enemy1(id, enemy2HitPoints, enemy2Damage, "enemy2", enemy2Sprite);
                } else
                {
                    PictureBox enemy1Sprite = new PictureBox();
                    enemy1Sprite.Image = Image.FromFile(pathForEnemy1Back);
                    enemy1Sprite.SizeMode = PictureBoxSizeMode.StretchImage;
                    enemy = new Enemy1(id, enemy1HitPoints, enemy1Damage, "enemy1", enemy1Sprite);
                }

                int numX = rndX.Next(0, places.Length);
                int numY = rndY.Next(0, places.Length);
                for (int j = 0; j < enemyLocations.GetLength(0); j++)
                {
                    if (enemyLocations[j, 0] != places[numX] && enemyLocations[j, 1] != places[numY])
                    {
                        enemy.sprite.Location = new Point(places[numX], places[numY]);

                        enemyLocations[i, 0] = enemy.sprite.Location.X;
                        enemyLocations[i, 1] = enemy.sprite.Location.Y;

                        break;
                    }
                    else
                    {
                        numX = rnd.Next(0, places.Length);
                        numY = rnd.Next(0, places.Length);
                    }
                }

                enemy.sprite.Size = new Size(sizeOfSides, sizeOfSides);
                enemy.sprite.BackgroundImage = Image.FromFile(pathForBackground);

                this.Controls.Add(enemy.sprite);

                countOfEnemies++;
                enemyList[i] = enemy;
                enemyStateList[i] = enemy.SaveState();
                GameState.SetState(level, countOfEnemies, xml.GetUserById(currentUserId));

                id++;
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
                GameState.SetState(level, countOfEnemies, xml.GetUserById(currentUserId));
                GenerateWorld();
            }
        }

        private bool isWall(int x, int y, PictureBox person)
        {
            if (Map.map[(person.Location.X + x) / sizeOfSides, (person.Location.Y + y) / sizeOfSides] == 1)
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
            if (Map.map[(character.sprite.Location.X + x) / sizeOfSides, (character.sprite.Location.Y + y) / sizeOfSides] == 2)
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

            createStats(level, character.killedEnemies);

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

            characterState = character.SaveState();
        }

        private void DestroyMap()
        {
            Array.Clear(Map.map, 0, Map.map.Length);
            this.Controls.Clear();

            MapState = Map.SaveState();
        }

        private void GenerateMap()
        {
            Map = new Map();
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
                            Map.map[i, j] = 2;

                            countOfDoors++;
                        }
                        else
                        {
                            GenerateBorder(i, j);

                            Map.map[i, j] = 1;
                        }
                    }
                    else if (i == 24)
                    {
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2 && j != 0 && j != 24)
                        {
                            GenerateDoors(i, j);
                            Map.map[i, j] = 2;

                            countOfDoors++;
                        }
                        else
                        {
                            GenerateBorder(i, j);

                            Map.map[i, j] = 1;
                        }
                    }
                    else if (j == 0)
                    {
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2 && i != 0 && i != 24)
                        {
                            GenerateDoors(i, j);
                            Map.map[i, j] = 2;

                            countOfDoors++;
                        }
                        else
                        {
                            GenerateBorder(i, j);

                            Map.map[i, j] = 1;
                        }
                    }
                    else if (j == 24)
                    {
                        if (rnd.Next(0, 50) == 1 && countOfDoors <= 2 && i != 0 && i != 24)
                        {
                            GenerateDoors(i, j);
                            Map.map[i, j] = 2;

                            countOfDoors++;
                        }
                        else
                        {
                            if (countOfDoors == 0)
                            {
                                GenerateDoors(i, j);
                                Map.map[i, j] = 2;

                                countOfDoors++;
                            }
                            else
                            {

                                GenerateBorder(i, j);

                                Map.map[i, j] = 1;
                            }
                        }
                    }
                    else
                    {
                        if (Map.map[i - 1, j] != 2 && Map.map[i + 1, j] != 2 && Map.map[i, j + 1] != 2 && Map.map[i, j - 1] != 2)
                        {
                            if (rnd.Next(1, 100) < 10)
                            {
                                GenerateStoneMoss(i, j);

                                Map.map[i, j] = 1;
                            }
                            else
                            {
                                Map.map[i, j] = 0;
                            }
                        }
                    }
                }
            }

            MapState = Map.SaveState();
        }

        private void GenerateStoneMoss(int i, int j)
        {
            string pathForStoneMoss = System.IO.Path.GetFullPath(@"textures\blocks\stoneMoss.png");

            PictureBox stoneMoss = new PictureBox();

            stoneMoss.Image = Image.FromFile(pathForStoneMoss);
            stoneMoss.SizeMode = PictureBoxSizeMode.StretchImage;

            stoneMoss.Location = new Point(i * sizeOfSides, j * sizeOfSides);
            stoneMoss.Size = new Size(sizeOfSides, sizeOfSides);
            this.Controls.Add(stoneMoss);
        }

        private void GenerateDoors(int i, int j)
        {
            string pathForBackground = System.IO.Path.GetFullPath(@"textures\blocks\waterfall-3.png");

            PictureBox door = new PictureBox();

            door.Image = Image.FromFile(pathForBackground);
            door.SizeMode = PictureBoxSizeMode.StretchImage;

            door.Location = new Point(i * sizeOfSides, j * sizeOfSides);
            door.Size = new Size(sizeOfSides, sizeOfSides);
            this.Controls.Add(door);
        }

        private void GenerateBorder(int i, int j)
        {
            string path = System.IO.Path.GetFullPath(@"textures\blocks\stonebricksmooth_cracked.png");

            PictureBox border = new PictureBox();

            border.Image = Image.FromFile(path);
            border.SizeMode = PictureBoxSizeMode.StretchImage;

            border.Location = new Point(i * sizeOfSides, j * sizeOfSides);
            border.Size = new Size(sizeOfSides, sizeOfSides);
            this.Controls.Add(border);
        }
    }
}
