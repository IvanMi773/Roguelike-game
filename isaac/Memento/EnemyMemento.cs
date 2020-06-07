using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace isaac.Memento
{
    class EnemyMemento
    {
        public int id { get; private set; }
        public string Name { get; private set; }
        public int HitPoints { get; private set; }
        public int damage { get; private set; }

        public PictureBox sprite { get; private set; }

        public EnemyMemento(int id, string Name, int HitPoints, int Damage, PictureBox sprite)
        {
            this.id = id;
            this.HitPoints = HitPoints;
            this.damage = Damage;
            this.Name = Name;
            this.sprite = sprite;
        }
    }
}
