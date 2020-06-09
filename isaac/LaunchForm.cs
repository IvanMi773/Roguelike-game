using isaac.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class LaunchForm : Form
    {
        private int userId;
        private string username;

        int currentGame;

        public LaunchForm(int userId)
        {
            InitializeComponent();

            this.userId = userId;
            WorkWithXml xml = new WorkWithXml("users.xml");

            username = xml.GetUserById(userId);

            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].HeaderCell.Value = "Гравець";
            dataGridView1.Columns[1].HeaderCell.Value = "Рівень";
            dataGridView1.Columns[2].HeaderCell.Value = "Очки";

            if (userId != 1)
            {
                dataGridView1.ReadOnly = true;
            } else
            {
                button2.Visible = true;
            }

            GetUserGames(username);

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
        }

        string[] saves = Directory.GetFiles(@"database/gamesaves/");
        string[] userGames;

        private void GetUserGames(string username)
        {
            int t = 0;

            userGames = new string[saves.Length];

            for (int i = 0; i < saves.Length; i++)
            {
                string[] fileName = saves[i].Split(new[] { '/' });
                string[] userInFileName = fileName[2].Split(new[] { '_', '.' });

                if (userInFileName[0] == username || userId == 1)
                {
                    userGames[t] = fileName[2];
                    t++;
                }
            }

            dataGridView1.RowCount = t;

            for (int i = 0; i < userGames.Length; i++)
            {
                if (userGames[i] == null || userGames[i] == "")
                {
                    break;
                }

                string[] userInFileName = userGames[i].Split(new[] { '_', '.' });

                dataGridView1.Rows[i].Cells[0].Value = userInFileName[0];
                dataGridView1.Rows[i].Cells[1].Value = userInFileName[1];
                dataGridView1.Rows[i].Cells[2].Value = userInFileName[2];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentGame = dataGridView1.CurrentCell.RowIndex;

            if (currentGame != 0)
            {
                this.Visible = false;

                Form1 form = new Form1(userId, userGames[currentGame]);
                form.ShowDialog();

                this.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Visible = false;

            Form1 form = new Form1(userId);
            form.ShowDialog();

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentGame = dataGridView1.CurrentCell.RowIndex;

            if (currentGame != 0)
            {
                File.Delete(@"database/gamesaves/" + userGames[currentGame]);

                dataGridView1.Rows.RemoveAt(currentGame);
                dataGridView1.Refresh();
            }
        }
    }
}
