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
using iRacingSDK;

namespace iGrudgeHolder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var iracing = new iRacingConnection();
            foreach (var data in iracing.GetDataFeed())
            {
                textBox1.Text = data.IsConnected.ToString() + Guid.NewGuid();
            }
        }
    }
}
