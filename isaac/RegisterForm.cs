using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace isaac
{
    public partial class RegisterForm : Form
    {
        XmlDocument xDoc = new XmlDocument();
        private bool isRegistered = false;
        private int id = 0;

        public RegisterForm()
        {
            InitializeComponent();

            ControlBox = false;

            xDoc.Load(@"database/users.xml");
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (!isRegistered)
            {
                base.OnClosing(e);
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            label1.Visible = false;
            button1.Visible = false;
            button2.Visible = false;

            button3.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            label1.Visible = false;
            button1.Visible = false;
            button2.Visible = false;

            button4.Visible = true;
            label5.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            label1.Visible = true;
            button1.Visible = true;
            button2.Visible = true;

            button4.Visible = false;
            label5.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;

            button3.Visible = false;
            label2.Visible = false;

            errorValidation.Visible = false;
            errorValidation1.Visible = false;

            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            errorValidation.Visible = false;
            errorValidation1.Visible = false;

            int logined = 0;

            errorValidation.Text = "";
            errorValidation1.Text = "";

            string username = textBox1.Text;
            string password = textBox2.Text;

            if (passwordValidation(password) == "" && usernameValidation(username) == "")
            {
                XmlElement xRoot = xDoc.DocumentElement;

                foreach (XmlNode node in xRoot)
                {
                    foreach (XmlNode childnode in node.ChildNodes)
                    {
                        if (childnode.Name == "username")
                        {
                            if (username == childnode.InnerText)
                            {
                                logined++;
                            }
                        }

                        if (childnode.Name == "password")
                        {
                            if (password == childnode.InnerText)
                            {
                                logined++;
                            }
                        }
                    }
                }

                if (logined == 2)
                {
                    isRegistered = true;

                    this.Close();
                }
                else
                {
                    errorValidation.Text = "Неправильний логін або пароль";
                    errorValidation1.Text = "Неправильний логін або пароль";

                    errorValidation.Visible = true;
                    errorValidation1.Visible = true;
                }

            }
            else
            {
                errorValidation.Text = usernameValidation(username);
                errorValidation1.Text = passwordValidation(password);

                errorValidation.Visible = true;
                errorValidation1.Visible = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            errorValidation.Visible = false;
            errorValidation1.Visible = false;

            errorValidation.Text = "";
            errorValidation1.Text = "";

            string username = textBox1.Text;
            string password = textBox2.Text;

            if (passwordValidation(password) == "" && usernameValidation(username) == "")
            {
                XmlElement xRoot = xDoc.DocumentElement;

                foreach (XmlNode node in xRoot)
                {
                    XmlNode attr = node.Attributes.GetNamedItem("id");
                    if (attr != null)
                        id = Convert.ToInt32(attr.Value);
                }

                XmlAttribute idAttr = xDoc.CreateAttribute("id");
                XmlElement userElem = xDoc.CreateElement("user");
                XmlElement usernameElem = xDoc.CreateElement("username");
                XmlElement passwordElem = xDoc.CreateElement("password");

                XmlText usernameText = xDoc.CreateTextNode(username);
                XmlText passwordText = xDoc.CreateTextNode(password);
                XmlText idText = xDoc.CreateTextNode((++id).ToString());

                idAttr.AppendChild(idText);
                usernameElem.AppendChild(usernameText);
                passwordElem.AppendChild(passwordText);
                userElem.AppendChild(usernameElem);
                userElem.Attributes.Append(idAttr);
                userElem.AppendChild(passwordElem);
                xRoot.AppendChild(userElem);
                xDoc.Save(@"database/users.xml");

                isRegistered = true;

                this.Close();
            } else
            {
                errorValidation.Text = usernameValidation(username);
                errorValidation1.Text = passwordValidation(password);

                errorValidation.Visible = true;
                errorValidation1.Visible = true;
            }
        }

        private string passwordValidation(string password)
        {
            if (password != "" && password.Length <= 0)
            {
                return "Пароль не може бути пустим";
            } else if (password.Length < 8)
            {
                return "Пароль має містити не менш ніж 8 символів";
            }
            else
            {
                return "";
            }
        }

        private string usernameValidation(string username)
        {
            if (username != "" && username.Length <= 0)
            {
                return "Імя користувача не може бути пустим";
            }
            else if (username.Length < 4)
            {
                return "Імя користувача має містити не менш ніж 4 символи";
            }
            else
            {
                return "";
            }
        }
    }
}
