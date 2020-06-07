using isaac.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace isaac
{
    class GameHistory
    {
        public void CreateGameSave(HeroMemento hero, EnemyMemento[] Enemies, MapMemento Map, GameMemento gameState)
        {
            WorkWithXml xml = new WorkWithXml("users.xml");

            // Game saving data
            XDocument doc = new XDocument();
            XElement save = new XElement("gamesave");

            XElement game = new XElement("game");
            XElement username = new XElement("username", gameState.userName);
            XElement level = new XElement("level", gameState.level);
            XElement countOfEnemies = new XElement("countOfEnemies", gameState.countOfEnemies);

            game.Add(username);
            game.Add(level);
            game.Add(countOfEnemies);

            // Character saving data
            XElement character = new XElement("character");
            XElement hp = new XElement("hitPoints", hero.HitPoints);
            XElement damage = new XElement("Damage", hero.Damage);
            XElement killedEnemies = new XElement("KilledEnemies", hero.KilledEnemies);
            XElement locX = new XElement("locationX", hero.sprite.Location.X);
            XElement locY = new XElement("locationY", hero.sprite.Location.Y);

            character.Add(hp);
            character.Add(damage);
            character.Add(killedEnemies);
            character.Add(locX); 
            character.Add(locY);

            // Enemies saving data
            XElement enemies = new XElement("enemies");
            foreach (EnemyMemento Enemy in Enemies)
            {
                XElement enemy = new XElement("enemy");

                XElement id = new XElement("id", Enemy.id);
                XElement name = new XElement("name", Enemy.Name);
                XElement enemyHp = new XElement("hitPoints", Enemy.HitPoints);
                XElement enemyDamage = new XElement("damage", Enemy.damage);
                XElement locationX = new XElement("locationX", Enemy.sprite.Location.X);
                XElement locationY = new XElement("locationY", Enemy.sprite.Location.Y);

                enemy.Add(id);
                enemy.Add(name);
                enemy.Add(enemyHp);
                enemy.Add(enemyDamage);
                enemy.Add(locationX);
                enemy.Add(locationY);

                enemies.Add(enemy);
            }

            // Map saving data
            XElement map = new XElement("map");
            for (int i = 0; i < Map.map.GetLength(0); i++)
            {
                XElement row = new XElement("row");
                for (int j = 0; j < Map.map.GetLength(1); j++)
                {
                    XElement cell = new XElement("cell", Map.map[i, j]);
                    row.Add(cell);
                }

                map.Add(row);
            }

            save.Add(game);
            save.Add(character);
            save.Add(enemies);
            save.Add(map);
            doc.Add(save);

            Random rnd = new Random();
            int num = rnd.Next(1000, 50000);

            doc.Save(@"database/gamesaves/" + gameState.userName + "_" + gameState.level.ToString() + "_" + num + ".xml");
        }
    }
}
