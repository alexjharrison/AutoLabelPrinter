using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoLabelPrinter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label4_DoubleClick(object sender, EventArgs e)
        {
            Form squak = new Form2();
            squak.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((radioGlobal.Checked==false) && (radioMicroHite.Checked==false))
            {
                MessageBox.Show("Please Choose CMM to Monitor","Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            if (statusLabel.Text.Equals("Off")==true)
                turnOn();
            else turnOff();
        }

        private void turnOn()
        {
            statusLabel.Text = "Monitoring...";
            statusLabel.BackColor = System.Drawing.Color.LightGreen;
            onOffButton.Text = "Press to Stop";
            radioGlobal.AutoCheck = false;
            radioMicroHite.AutoCheck = false;
        }

        private void turnOff()
        {
            statusLabel.Text = "Off";
            statusLabel.BackColor = SystemColors.Control;
            onOffButton.Text = "Press to Start";
            radioGlobal.AutoCheck = true;
            radioMicroHite.AutoCheck = true;
        }

    
        
    }
}
