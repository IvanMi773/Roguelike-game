using isaac.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace isaac
{
    class Character
    {
        public int HitPoints = 300;
        public int damage = 100;

        public int killedEnemies = 0;

        public PictureBox sprite = new PictureBox();

        public HeroMemento SaveState()
        {
            return new HeroMemento(HitPoints, damage, killedEnemies, sprite);
        }

        public void RestoreState(HeroMemento memento)
        {
            this.HitPoints = memento.HitPoints;
            this.damage = memento.Damage;
            this.killedEnemies = memento.KilledEnemies;
            this.sprite = memento.sprite;
        }
    }
}
