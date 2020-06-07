using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace isaac.Memento
{
    class HeroMemento
    {
        public int HitPoints { get; private set; }
        public int Damage { get; private set; }
        public int KilledEnemies { get; private set; }
        public PictureBox sprite { get; private set; }

        public HeroMemento(int HitPoints, int Damage, int KilledEnemies, PictureBox sprite)
        {
            this.HitPoints = HitPoints;
            this.Damage = Damage;
            this.KilledEnemies = KilledEnemies;
            this.sprite = sprite;
        }
    }
}
