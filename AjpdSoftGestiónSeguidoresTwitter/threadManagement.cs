using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestiónSeguidoresTwitter
{
    public partial class threadManagement : Form
    {
        public threadManagement()
        {
            InitializeComponent();
        }

        private void initManagement_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxNroThread.Text))
            {
                List<Thread> listProcess = new List<Thread>();
                List<string> listUsers = new List<string>();
                
                int i = 0;
                
                string text = System.IO.File.ReadAllText(textBoxNroThread.Text + "\\acces.txt");
                var accounts = Convert.ToString(text).Split(new string[] { "</b>" }, StringSplitOptions.None);

                foreach (var account in accounts)
                    listUsers.Add((account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] : account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[1]).Split(new string[] { "username:" }, StringSplitOptions.None)[1]);

                foreach (var account in accounts)
                {
                    List<string> listUsersAux = new List<string>(listUsers);
                    string username = (account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] : account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[1]).Split(new string[] { "username:" }, StringSplitOptions.None)[1];
                    string consumer_key = (account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[1] : account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[2]).Split(new string[] { "Consumer Key:" }, StringSplitOptions.None)[1];
                    string consumer_secret = (account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[2] : account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[3]).Split(new string[] { "Consumer Secret:" }, StringSplitOptions.None)[1];
                    string access_token = (account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[3] : account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[4]).Split(new string[] { "Access Token:" }, StringSplitOptions.None)[1];
                    string access_token_secret = (account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[4] : account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[5]).Split(new string[] { "Access Token Secret:" }, StringSplitOptions.None)[1];
                    listProcess.Add(new Thread(() => aplication1(listUsersAux, username, consumer_key, consumer_secret, access_token, access_token_secret)));
                
                }

                foreach (var process in listProcess)
                {
                    process.SetApartmentState(System.Threading.ApartmentState.STA);
                    process.Start();
                }
                this.Visible = false;
            }
            else
            {
                MessageBox.Show("Debe ingresar una cantidad de cuentas" +
                   Environment.NewLine + Environment.NewLine,
                   "Mensaje",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        static void aplication1(List<string> listUsersAux, string username, string consumer_key, string consumer_secret, string access_token, string access_token_secret)
        {
            Application.Run(new formTwitter(listUsersAux, username,consumer_key,consumer_secret,access_token,access_token_secret));
        }
    }
}
