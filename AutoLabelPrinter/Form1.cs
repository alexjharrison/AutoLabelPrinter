using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AutoLabelPrinter
{
    public partial class Form1 : Form
    {
        /*
        public string boxLabelLocation = @"\\lisc-lpq01\Interfaces\LabsQ\outbound\3090\usso-boxlabel";
        public string barcodeLabelLocation = @"\\lisc-lpq01\Interfaces\LabsQ\outbound\3090\usso-barcodelabel";
        public string RFIDLabelLocation = @"\\lisc-lpq01\Interfaces\LabsQ\outbound\3090\usso-csvexport";
         */

        public string boxLabelLocation = @"C:\Users\harrale\Documents\usso-boxlabel";
        public string barcodeLabelLocation = @"C:\Users\harrale\Documents\usso-barcodelabel";
        public string RFIDLabelLocation = @"C:\Users\harrale\Documents\usso-csvexport";
        public string CMMID;
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        


        public Form1()
        {
            InitializeComponent();
            t.Interval = 5000; // specify interval time as you want
            t.Tick += new EventHandler(timer_Tick);
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
            if (radioGlobal.Checked == true) CMMID = "TE-59";
            else CMMID = "TE-43";
            statusLabel.Text = "Monitoring...";
            statusLabel.BackColor = System.Drawing.Color.LightGreen;
            onOffButton.Text = "Press to Stop";
            radioGlobal.AutoCheck = false;
            radioMicroHite.AutoCheck = false;
            
            //timer start
            t.Start();

        }

        private void turnOff()
        {
            t.Stop();
            statusLabel.Text = "Off";
            statusLabel.BackColor = SystemColors.Control;
            onOffButton.Text = "Press to Start";
            radioGlobal.AutoCheck = true;
            radioMicroHite.AutoCheck = true;
        }

        private string checkIfFileThere()
        {
            string[] folderContents = Directory.GetFiles(boxLabelLocation);
            for (int i = 0; i < folderContents.Length; i++)
                if (folderContents[i].Contains(CMMID))
                    return folderContents[i];
            return "";
        }

        void timer_Tick(object sender, EventArgs e)
        {
            string fileToPrint = checkIfFileThere();
            if (fileToPrint != "")
                printFiles(fileToPrint);
        }

        
        private void printFiles(string fileToPrint)
        {

            
        }






        private void label4_DoubleClick(object sender, EventArgs e)
        {
            Form squak = new Form2();
            squak.Show();

        }
        
    }
}
