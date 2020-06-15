using isaac.Memento;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using isaac.Instances;

namespace isaac.Game
{
    class WorkWithXml
    {
        XmlDocument xDoc = new XmlDocument();

        public WorkWithXml(string filename)
        {
            xDoc.Load(@"database/" + filename);
        }

        public HeroMemento ReadCharacterDataFromFile()
        {
            HeroMemento hero = null;
            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode node in xRoot)
            {
                if (node.Name == "character")
                {
                    string pathForCharacterGoBack = System.IO.Path.GetFullPath(@"textures\hero\walk\hero-walk-back-2.png");
                    PictureBox sprite = new PictureBox();
                    sprite.Image = Image.FromFile(pathForCharacterGoBack);
                    sprite.SizeMode = PictureBoxSizeMode.StretchImage;
                    sprite.Location = new Point(Convert.ToInt32(node["locationX"].InnerText), Convert.ToInt32(node["locationY"].InnerText));
                    sprite.Size = new Size(30, 30);

                    hero = new HeroMemento(Convert.ToInt32(node["hitPoints"].InnerText), Convert.ToInt32(node["Damage"].InnerText), Convert.ToInt32(node["KilledEnemies"].InnerText), sprite);
                }
            }

            return hero;
        }

        public GameMemento ReadGameDataFromFile()
        {
            GameMemento game = null;

            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode node in xRoot)
            {
                if (node.Name == "game")
                {
                    game = new GameMemento(Convert.ToInt32(node["level"].InnerText), Convert.ToInt32(node["countOfEnemies"].InnerText), node["username"].InnerText, Convert.ToInt32(node["score"].InnerText));
                }
            }

            return game;
        }

        public Map ReadMapDataFromFile()
        {
            XmlElement xRoot = xDoc.DocumentElement;

            int[,] m = new int[25, 25];

            int i = 0, j = 0;
            foreach (XmlNode row in xRoot["map"])
            {
                foreach (XmlNode cell in row)
                {
                    m[i, j] = Convert.ToInt32(cell.InnerText);
                    j++;
                }
                i++;
                j = 0;
            }

            Map map = new Map();
            map.map = m;

            return map;
        }

        public EnemyMemento[] ReadEnemyDataFromFile()
        {
            XmlElement xRoot = xDoc.DocumentElement;
            int i = 0;

            foreach (XmlNode node in xRoot["enemies"])
            {
                i++;
            }

            EnemyMemento[] enemies = new EnemyMemento[i];

            i = 0;
            foreach (XmlNode node in xRoot["enemies"])
            {
                string pathForEnemy1Back = System.IO.Path.GetFullPath(@"textures\enemies\enemy1\mole-walk-back-4.png");
                string pathForEnemy2Back = System.IO.Path.GetFullPath(@"textures\enemies\enemy2\treant-walk-back-3.png");
                string pathForBackground = System.IO.Path.GetFullPath(@"textures\blocks\sand-3.png");

                PictureBox sprite = new PictureBox();
                sprite.Size = new Size(30, 30);
                sprite.BackgroundImage = Image.FromFile(pathForBackground);

                if (node["name"].InnerText == "enemy2")
                {
                    sprite.Image = Image.FromFile(pathForEnemy2Back);
                }
                else
                {
                    sprite.Image = Image.FromFile(pathForEnemy1Back);
                }

                sprite.SizeMode = PictureBoxSizeMode.StretchImage;
                sprite.Location = new Point(Convert.ToInt32(node["locationX"].InnerText), Convert.ToInt32(node["locationY"].InnerText));

                EnemyMemento enemy = new EnemyMemento(Convert.ToInt32(node["id"].InnerText), node["name"].InnerText, Convert.ToInt32(node["hitPoints"].InnerText), Convert.ToInt32(node["damage"].InnerText), sprite);

                enemies[i] = enemy;
                i++;
            }

            return enemies;
        }

        public int Register(string username, string password)
        {
            int id = 0;

            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode node in xRoot)
            {
                foreach (XmlNode childnode in node.ChildNodes)
                {
                    if (childnode.Name == "id")
                    {
                        id = Convert.ToInt32(childnode.InnerText);
                    }
                }
            }

            XmlElement idElem = xDoc.CreateElement("id");
            XmlElement userElem = xDoc.CreateElement("user");
            XmlElement usernameElem = xDoc.CreateElement("username");
            XmlElement passwordElem = xDoc.CreateElement("password");

            XmlText usernameText = xDoc.CreateTextNode(username);
            XmlText passwordText = xDoc.CreateTextNode(password);
            XmlText idText = xDoc.CreateTextNode((id + 1).ToString());

            idElem.AppendChild(idText);
            usernameElem.AppendChild(usernameText);
            passwordElem.AppendChild(passwordText);
            userElem.AppendChild(idElem);
            userElem.AppendChild(usernameElem);
            userElem.AppendChild(passwordElem);
            xRoot.AppendChild(userElem);
            xDoc.Save(@"database/users.xml");

            return id + 1;
        }

        public int Login(string username, string password)
        {
            int logined = 0;
            int id = 0;

            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode node in xRoot)
            {
                if (logined == 2)
                {
                    break;
                }

                id = Convert.ToInt32(node.ChildNodes[0].InnerText);

                if (username == node.ChildNodes[1].InnerText)
                {
                    logined++;
                }


                if (password == node.ChildNodes[2].InnerText)
                {
                    logined++;
                }
            }

            if (logined != 2)
            {
                return 0;
            }
            else return id;

        }

        public string GetUserById(int id)
        {
            XmlElement xRoot = xDoc.DocumentElement;

            string user = "";

            foreach (XmlNode node in xRoot)
            {
                if (node.ChildNodes[0].Name == "id")
                {
                    if (Convert.ToInt32(node.ChildNodes[0].InnerText) == id)
                    {
                        user = node.ChildNodes[1].InnerText;
                    }
                }
            }

            return user;
        }
    }
}
