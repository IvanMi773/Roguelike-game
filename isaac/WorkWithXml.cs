using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace isaac
{
    class WorkWithXml
    {
        XmlDocument xDoc = new XmlDocument();

        public WorkWithXml()
        {
            xDoc.Load(@"database/users.xml");
        }

        public int Register(string username, string password)
        {
            int id = 0;

            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode node in xRoot)
            {
                foreach (XmlNode childnode in node.ChildNodes)
                {
                    if (childnode.Name == "id")
                    {
                        id = Convert.ToInt32(childnode.InnerText);
                    }
                }
            }

            XmlElement idElem = xDoc.CreateElement("id");
            XmlElement userElem = xDoc.CreateElement("user");
            XmlElement usernameElem = xDoc.CreateElement("username");
            XmlElement passwordElem = xDoc.CreateElement("password");

            XmlText usernameText = xDoc.CreateTextNode(username);
            XmlText passwordText = xDoc.CreateTextNode(password);
            XmlText idText = xDoc.CreateTextNode((id + 1).ToString());

            idElem.AppendChild(idText);
            usernameElem.AppendChild(usernameText);
            passwordElem.AppendChild(passwordText);
            userElem.AppendChild(idElem);
            userElem.AppendChild(usernameElem);
            userElem.AppendChild(passwordElem);
            xRoot.AppendChild(userElem);
            xDoc.Save(@"database/users.xml");

            return id + 1;
        }

        public int Login(string username, string password)
        {
            int logined = 0;
            int id = 0;

            XmlElement xRoot = xDoc.DocumentElement;

            foreach (XmlNode node in xRoot)
            {
                if (logined == 2)
                {
                    break;
                }

                id = Convert.ToInt32(node.ChildNodes[0].InnerText);

                if (username == node.ChildNodes[1].InnerText)
                {
                    logined++;
                }
                    

                if (password == node.ChildNodes[2].InnerText)
                {
                    logined++;
                }
            }

            if (logined != 2)
            {
                return 0;
            }
            else return id;

        }

        public string GetUserById(int id)
        {
            XmlElement xRoot = xDoc.DocumentElement;

            string user = "";

            foreach (XmlNode node in xRoot)
            {
                if (node.ChildNodes[0].Name == "id")
                {
                    if (Convert.ToInt32(node.ChildNodes[0].InnerText) == id)
                    {
                        user = node.ChildNodes[1].InnerText;
                    }
                }
                //foreach (XmlNode childnode in node.ChildNodes)
                //{
                //    if (childnode.Name == "id")
                //    {
                        
                //    }

                //    if (childnode.Name == "username")
                //    {
                //        user = childnode.InnerText;
                //    }
                //}
            }

            return user;
        }
    }
}
