using isaac.Memento;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace isaac.Instances
{
    class Map
    {
        public int[,] map = new int[25, 25];
        public int countOfDoors;

        public MapMemento SaveState()
        {
            return new MapMemento(map, countOfDoors);
        }

        public void RestoreState(MapMemento memento)
        {
            this.map = memento.map;
            this.countOfDoors = memento.countOfDoors;
        }
    }
}
