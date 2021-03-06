﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isaac.Memento
{
    class GameMemento
    {
        public int level { get; private set; }
        public int countOfEnemies { get; private set; }
        public string userName { get; private set; }
        public int score { get; private set; }

        public GameMemento(int lev, int count, string username, int score)
        {
            this.level = lev;
            this.countOfEnemies = count;
            this.userName = username;
            this.score = score;
        }

        public void SetState(int lev, int count, string username, int score)
        {
            this.level = lev;
            this.countOfEnemies = count;
            this.userName = username;
            this.score = score;
        }
    }
}
