using isaac.Instances;
using isaac.Memento;
using isaac.Game;
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

        // вікно гри і геймбар
        private int width = 770 + 300;
        private int height = 790;
        private int sizeOfSides = 30;

        private int score = 0;

        Map Map = new Map();
        MapMemento MapState;

        Enemy1[] enemyList;
        EnemyMemento[] enemyStateList;
        private int countOfEnemies = 0;

        private int[] characterDir = new int[2];

        Character character = new Character();
        HeroMemento characterState;
        PictureBox[] heart;

        GameHistory history = new GameHistory();
        GameMemento GameState;

        string pathForBackground = Path.GetFullPath(@"textures\blocks\sand-3.png");
        string pathForBackground2 = Path.GetFullPath(@"textures\blocks\waterfall-2.png");
        string pathForOpenedDoor = Path.GetFullPath(@"textures\blocks\sand-3.png");

        string pathForCharacterGoFont = Path.GetFullPath(@"textures\hero\walk\hero-walk-front.gif");
        string pathForCharacterGoBack = Path.GetFullPath(@"textures\hero\walk\hero-walk-back.gif");
        string pathForCharacterGoSideLeft = Path.GetFullPath(@"textures\hero\walk\hero-walk-side-left.gif");
        string pathForCharacterGoSideRight = Path.GetFullPath(@"textures\hero\walk\hero-walk-side-right.gif");

        string pathForFullHeart = Path.GetFullPath(@"textures\hearts-1.png");

        string pathForEnemy1Font = Path.GetFullPath(@"textures\enemies\enemy1\enemy1-walk-front.gif");
        string pathForEnemy1Back = Path.GetFullPath(@"textures\enemies\enemy1\enemy1-walk-back.gif");
        string pathForEnemy1Left = Path.GetFullPath(@"textures\enemies\enemy1\enemy1-walk-side-left.gif");
        string pathForEnemy1Right = Path.GetFullPath(@"textures\enemies\enemy1\enemy1-walk-side-right.gif");

        string pathForEnemy2Font = Path.GetFullPath(@"textures\enemies\enemy2\treant-walk-front.gif");
        string pathForEnemy2Back = Path.GetFullPath(@"textures\enemies\enemy2\treant-walk-back.gif");
        string pathForEnemy2Left = Path.GetFullPath(@"textures\enemies\enemy2\treant-walk-side-left.gif");
        string pathForEnemy2Right = Path.GetFullPath(@"textures\enemies\enemy2\treant-walk-side-right.gif");

        private int currentUserId = 0;

        Timer enemyTimer;
        WorkWithXml xml = new WorkWithXml("users.xml");

        public Form1(int id, string GameSavePath = null)
        {
            currentUserId = id;

            InitializeComponent();

            this.Width = width;
            this.Height = height;

            enemyTimer = null;
            enemyTimer = new Timer();
            enemyTimer.Tick += new EventHandler(moveEnemies);
            enemyTimer.Interval = 500;

            RestoreGame(GameSavePath);

            this.KeyDown += new KeyEventHandler(keyPress);
        }

        private int level = 1;

        private void RestoreGame(string gamePath)
        {
            if (gamePath != null)
            {
                WorkWithXml savedData = new WorkWithXml("gamesaves/" + gamePath);

                characterState = savedData.ReadCharacterDataFromFile();
                character.RestoreState(characterState);
                character.SaveState();

                GameState = savedData.ReadGameDataFromFile();
                level = GameState.level;
                score = GameState.score;

                Map = savedData.ReadMapDataFromFile();
                Map.SaveState();

                int t = 0;
                for (int i = 0; i < 25; i++)
                {
                    for (int j = 0; j < 25; j++)
                    {
                        if (Map.map[i, j] == 1)
                        {
                            GenerateBorder(i, j);
                        }
                        else if (Map.map[i, j] == 2)
                        {
                            GenerateDoors(i, j, t);
                            t++;
                        }
                    }
                }

                enemyStateList = savedData.ReadEnemyDataFromFile();
                enemyList = new Enemy1[enemyStateList.Length];

                for (int i = 0; i < enemyStateList.Length; i++)
                {
                    if (enemyStateList[i] == null)
                    {
                        break;
                    }
                    enemyList[i] = new Enemy1(enemyStateList[i].id, enemyStateList[i].HitPoints, enemyStateList[i].damage, enemyStateList[i].Name, enemyStateList[i].sprite);
                    this.Controls.Add(enemyList[i].sprite);

                    countOfEnemies++;
                }

                if (countOfEnemies == 0)
                {
                    OpenDoors();
                }

                GenerateWorld(true);
            }
            else
            {
                PictureBox sprite = new PictureBox();
                sprite.Image = Image.FromFile(pathForCharacterGoBack);
                sprite.SizeMode = PictureBoxSizeMode.StretchImage;
                sprite.Location = new Point(300, 300);
                sprite.Size = new Size(sizeOfSides, sizeOfSides);

                characterState = new HeroMemento(300, 100, 0, sprite);
                character.RestoreState(characterState);
                characterState = character.SaveState();

                GenerateWorld(false);
            }
        }

        // коли користувач створює нову гру то генеруємо карту і ворогів
        // інакше це робимо в RestoreState
        private void GenerateWorld(bool isFirstGame)
        {
            timer = new Timer();

            if (!isFirstGame)
            {
                countOfEnemies = 0;
                GenerateMap();
                GenerateEnemies();
            }

            GenerateCharacter();

            enemyTimer.Start();

            GameState = new GameMemento(level, countOfEnemies, xml.GetUserById(currentUserId), score);

            createStats(level, character.killedEnemies);

            saveBtn.Location = new Point(width - 300, 700);
            saveBtn.Size = new Size(120, 40);
            saveBtn.BackgroundImage = Image.FromFile(pathForBackground2);
            saveBtn.FlatStyle = FlatStyle.Popup;
            saveBtn.Font = new Font(saveBtn.Font.Name, 13, FontStyle.Bold);
            saveBtn.ForeColor = Color.White;
            saveBtn.Enabled = false;
            saveBtn.Text = "Зберегти";
            saveBtn.Click += new EventHandler((o, ev) =>
            {
                history.CreateGameSave(characterState, enemyStateList, MapState, GameState);
                saveBtn.Enabled = false;
                exitBtn.Enabled = false;
            });
            this.Controls.Add(saveBtn);

            exitBtn.Location = new Point(width - 170, 700);
            exitBtn.Size = new Size(120, 40);
            exitBtn.BackgroundImage = Image.FromFile(pathForBackground2);
            exitBtn.FlatStyle = FlatStyle.Popup;
            exitBtn.Font = new Font(exitBtn.Font.Name, 13, FontStyle.Bold);
            exitBtn.ForeColor = Color.White;
            exitBtn.Text = "Вийти";
            exitBtn.Enabled = false;
            exitBtn.Click += new EventHandler((o, ev) =>
            {
                Application.Exit();
            });
            this.Controls.Add(exitBtn);

            background.Image = Image.FromFile(pathForBackground);
            background.Location = new Point(0, 0);
            background.Size = new Size(750, 750);
            background.SizeMode = PictureBoxSizeMode.StretchImage;
            background.BringToFront();
            this.Controls.Add(background);

            character.sprite.BackgroundImage = background.Image;
        }

        private void GameOver()
        {
            characterState = null;

            PictureBox sprite = new PictureBox();
            sprite.Image = Image.FromFile(pathForCharacterGoBack);
            sprite.SizeMode = PictureBoxSizeMode.StretchImage;
            sprite.Location = new Point(300, 300);
            sprite.Size = new Size(sizeOfSides, sizeOfSides);

            characterState = new HeroMemento(300, 100, 0, sprite);
            character.RestoreState(characterState);
            characterState = character.SaveState();

            level = 1;
            score = 0;
            timer.Stop();

            enemyTimer.Stop();

            GameOverForm f = new GameOverForm();
            f.ShowDialog();
            
            if (Data.redirect)
            {
                this.Visible = false;

                LaunchForm form = new LaunchForm(currentUserId);
                form.ShowDialog();

                this.Close();
            } else
            {
                GenerateWorld(false);
            }
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
                        }
                        else
                        {
                            this.Controls.Remove(hearts[character.HitPoints / 50]);
                            this.Controls.Remove(hearts[(character.HitPoints + 50) / 50]);
                        }
                    }

                    if (character.HitPoints <= 0)
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
            createStats(level, character.killedEnemies);
        }

        Random rnd = new Random();

        private void killEnemy(PictureBox arrow)
        {
            for (int i = 0; i < enemyList.Length; i++)
            {
                if (enemyList[i] == null)
                {
                    break;
                }

                if (arrow.Location == enemyList[i].sprite.Location)
                {
                    enemyList[i].HitPoints -= character.damage;

                    if (enemyList[i].HitPoints == 0)
                    {
                        if (enemyList[i].Name == "enemy1")
                        {
                            score += 100;
                        }
                        else if (enemyList[i].Name == "enemy2")
                        {
                            score += 300;
                        }

                        if (rnd.Next(1, 10) == 1 && enemyList[i].Name == "enemy2")
                        {
                            for (int j = 0; j < heart.Length; j++)
                            {
                                if (heart[j] == null)
                                {
                                    heart[j] = new PictureBox();
                                    heart[j].Image = Image.FromFile(pathForFullHeart);
                                    heart[j].SizeMode = PictureBoxSizeMode.StretchImage;
                                    heart[j].BackgroundImage = background.Image;
                                    heart[j].Location = enemyList[i].sprite.Location;
                                    heart[j].Size = new Size(sizeOfSides - 10, sizeOfSides - 10);
                                    this.Controls.Add(heart[j]);

                                    heart[j].BringToFront();

                                    break;
                                }
                            }
                        }

                        character.killedEnemies++;
                        removeEnemy(enemyList[i]);
                        characterState = character.SaveState();
                        GameState.SetState(level, countOfEnemies, xml.GetUserById(currentUserId), score);

                        if (countOfEnemies <= 0)
                        {
                            OpenDoors();
                        }

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

            countOfEnemiesLabel.Text = "Очки: " + score.ToString();
            countOfEnemiesLabel.Location = new Point(width - 250, 185);
            countOfEnemiesLabel.Size = new Size(150, 25);
            countOfEnemiesLabel.Font = new Font(countOfEnemiesLabel.Font.Name, 12, FontStyle.Regular);
            this.Controls.Add(countOfEnemiesLabel);
            countOfEnemiesLabel.Refresh();
        }

        Button saveBtn = new Button();
        Button exitBtn = new Button();

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
                        }
                        else
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
                if (enemyList[j] == null)
                {
                    break;
                }

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

            var places = new int[] { sizeOfSides * 2, sizeOfSides * 3, sizeOfSides * 4, sizeOfSides * 5, sizeOfSides * 6, sizeOfSides * 7, sizeOfSides * 8, sizeOfSides * 12, sizeOfSides * 13, sizeOfSides * 14, sizeOfSides * 15, sizeOfSides * 16, sizeOfSides * 17, sizeOfSides * 18, sizeOfSides * 19, sizeOfSides * 20, sizeOfSides * 21, sizeOfSides * 22, sizeOfSides * 23 };

            int enemy1HitPoints = 100;
            int enemy1Damage = 50;

            int enemy2HitPoints = 300;
            int enemy2Damage = 100;

            int id = 1;

            enemyList = new Enemy1[rnd.Next(1 + level, 6 + level)];
            enemyStateList = new EnemyMemento[enemyList.Length];
            enemyLocations = new int[enemyList.Length, 2];
            heart = new PictureBox[enemyList.Length];

            for (int i = 0; i < enemyList.Length; i++)
            {
                Enemy1 enemy;

                if (rnd.Next(1, 10) <= 1 + level)
                {
                    PictureBox enemy2Sprite = new PictureBox();
                    enemy2Sprite.Image = Image.FromFile(pathForEnemy2Back);
                    enemy2Sprite.SizeMode = PictureBoxSizeMode.StretchImage;
                    enemy = new Enemy1(id, enemy2HitPoints, enemy2Damage, "enemy2", enemy2Sprite);
                }
                else
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

        private void GoDoors()
        {
            if (countOfEnemies <= 0)
            {
                level++;
                score += 100 * level;
                character.sprite.Location = new Point(300, 300);
                characterState = character.SaveState();
                GameState.SetState(level, countOfEnemies, xml.GetUserById(currentUserId), score);
                GenerateWorld(false);
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
                GoDoors();
            }
            else if (isWall(x, y, character.sprite))
            {
                characterDir[0] = (x / sizeOfSides);
                characterDir[1] = (y / sizeOfSides);

                character.sprite.Image = Image.FromFile(path);
                character.sprite.Location = new Point(character.sprite.Location.X + x, character.sprite.Location.Y + y);

                if (heart != null)
                {
                    for (int i = 0; i < heart.Length; i++)
                    {
                        if (heart[i] != null)
                        {
                            if (character.sprite.Location == heart[i].Location)
                            {
                                if (character.HitPoints < 300)
                                {
                                    character.HitPoints += 50;
                                    characterState = character.SaveState();
                                    character.RestoreState(characterState);

                                    hearts = new PictureBox[6];

                                    for (int s = 0; s < character.HitPoints; s += 50)
                                    {
                                        PictureBox heart = new PictureBox();
                                        heart.Image = Image.FromFile(pathForFullHeart);
                                        heart.SizeMode = PictureBoxSizeMode.StretchImage;
                                        heart.Location = new Point(width - 250 + s / 2, 120);
                                        heart.Size = new Size(25, 25);

                                        hearts[i / 50] = heart;
                                    }

                                    for (int j = 0; j < 6; j++)
                                    {
                                        this.Controls.Add(hearts[i]);
                                    }
                                }

                                this.Controls.Remove(heart[i]);
                            }
                        }
                    }
                }
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

            saveBtn.Enabled = true;
            exitBtn.Enabled = true;
        }

        private void GenerateCharacter()
        {
            hearts = new PictureBox[6];
            character.RestoreState(characterState);

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

        private void OpenDoors()
        {
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i] == null)
                {
                    break;
                }

                doors[i].Image = Image.FromFile(pathForOpenedDoor);
                doors[i].SizeMode = PictureBoxSizeMode.StretchImage;
                doors[i].Refresh();
            }
        }

        private void DestroyMap()
        {
            Array.Clear(Map.map, 0, Map.map.Length);
            this.Controls.Clear();

            MapState = Map.SaveState();
        }

        PictureBox[] doors = new PictureBox[3];

        private void GenerateMap()
        {
            DestroyMap();

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
                            GenerateDoors(i, j, countOfDoors);
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
                            GenerateDoors(i, j, countOfDoors);
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
                            GenerateDoors(i, j, countOfDoors);
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
                            GenerateDoors(i, j, countOfDoors);
                            Map.map[i, j] = 2;

                            countOfDoors++;
                        }
                        else
                        {
                            if (countOfDoors == 0)
                            {
                                GenerateDoors(i, j, countOfDoors);
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

        private void GenerateDoors(int i, int j, int countOfDoors)
        {
            string pathForDoor = Path.GetFullPath(@"textures\blocks\door-1.png");

            PictureBox door = new PictureBox();

            door.Image = Image.FromFile(pathForDoor);
            door.SizeMode = PictureBoxSizeMode.StretchImage;

            door.Location = new Point(i * sizeOfSides, j * sizeOfSides);
            door.Size = new Size(sizeOfSides, sizeOfSides);
            door.Parent = background;
            this.Controls.Add(door);

            doors[countOfDoors] = door;
        }

        private void GenerateBorder(int i, int j)
        {
            string path = System.IO.Path.GetFullPath(@"textures\blocks\stonebricksmooth_cracked.png");

            PictureBox border = new PictureBox();

            border.Image = Image.FromFile(path);
            border.SizeMode = PictureBoxSizeMode.StretchImage;

            border.Location = new Point(i * sizeOfSides, j * sizeOfSides);
            border.Size = new Size(sizeOfSides, sizeOfSides);
            border.Parent = background;

            this.Controls.Add(border);
        }
    }
}
