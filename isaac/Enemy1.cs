using isaac.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace isaac
{
    class Enemy1
    {
        public int id;
        public string Name;
        public int HitPoints;
        public int damage;

        public PictureBox sprite;

        public Enemy1(int id, int hp, int damage, string name, PictureBox sprite)
        {
            this.id = id;
            this.HitPoints = hp;
            this.damage = damage;
            this.sprite = sprite;
            this.Name = name;
        }

        public EnemyMemento SaveState()
        {
            return new EnemyMemento(id, Name, HitPoints, damage, sprite);
        }

        public void RestoreState(EnemyMemento memento)
        {
            this.id = memento.id;
            this.HitPoints = memento.HitPoints;
            this.damage = memento.damage;
            this.sprite = memento.sprite;
            this.Name = memento.Name;
        }
    }
}
