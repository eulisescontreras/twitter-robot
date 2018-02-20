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


                for (int i = 0; i < Convert.ToInt32(textBoxNroThread.Text); i++)
                {
                    listProcess.Add(new Thread(() => aplication1()));
                    listProcess[i].SetApartmentState(System.Threading.ApartmentState.STA);
                    listProcess[i].Start();
                    
                }
            }
            else
            {
                MessageBox.Show("Debe ingresar una cantidad de cuentas" +
                   Environment.NewLine + Environment.NewLine,
                   "Mensaje",
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        static void aplication1()
        {
            Application.Run(new formTwitter());
        }
    }
}
