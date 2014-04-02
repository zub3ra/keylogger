using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        int time;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer1.Interval = 1000;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (time % 2 == 1)
            {
                label1.BackColor = Color.Black;
                label1.ForeColor = Color.Red;
            }
            else
            {
                label1.BackColor = Color.Red;
                label1.ForeColor = Color.Black;
            }
            time++;
        }
    }
}
