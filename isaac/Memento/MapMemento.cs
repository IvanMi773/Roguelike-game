using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isaac.Memento
{
    class MapMemento
    {
        public int[,] map = new int[25, 25];

        public MapMemento(int[,] map)
        {
            this.map = map;
        }
    }
}
