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
        
        public string boxLabelLocation,barcodeLabelLocation,RFIDLabelLocation;
        public string MicrohiteID, GlobalID;
        public string GlobalCodesoftWatchFolder, MicrohiteCodesoftWatchFolder, GlobalCodesoftExeLocation, MicrohiteCodesoftExeLocation;
        public string GlobalDruckerStationLocation, MicroHiteDruckerStationLocation;
        public string GlobalBoxLabelPrinter, MicroHiteBoxLabelPrinter, GlobalBarcodePrinter, MicroHiteBarcodeLabelPrinter;
        public string CMMID, CodeSoftWatchFolder, CodesoftExe, DruckerStation, BoxPrinter, BarcodePrinter;
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
            readConfigFile();
            //set specifics for codesoft watched folder and program details here
            if (radioGlobal.Checked == true)
            {
                CMMID = GlobalID; 
                CodeSoftWatchFolder = GlobalCodesoftWatchFolder; 
                CodesoftExe = GlobalCodesoftExeLocation; 
                DruckerStation = GlobalDruckerStationLocation; 
                BoxPrinter = GlobalBoxLabelPrinter; 
                BarcodePrinter = GlobalBarcodePrinter;
            }
            else
            {
                CMMID = MicrohiteID;
                CodeSoftWatchFolder = MicrohiteCodesoftWatchFolder;
                CodesoftExe = MicrohiteCodesoftExeLocation;
                DruckerStation = MicroHiteDruckerStationLocation;
                BoxPrinter = MicroHiteBoxLabelPrinter;
                BarcodePrinter = MicroHiteBarcodeLabelPrinter;
            }
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
            
            string batchID = fileToPrint.Substring( fileToPrint.IndexOf("usso-box-") + 9 , 6);
            MessageBox.Show(batchID);
        }

        private void readConfigFile()
        {
            string[] lines;
            try { lines = System.IO.File.ReadAllLines("InitialConfiguration.txt"); }
            catch { 
                MessageBox.Show("InitialConfiguration.txt can not be found");
                Application.Exit();
            }
            lines = System.IO.File.ReadAllLines("InitialConfiguration.txt");
            boxLabelLocation = lines[1];
            barcodeLabelLocation = lines[4];
            RFIDLabelLocation = lines[7];
            MicrohiteID = lines[10];
            GlobalID = lines[13];
            GlobalCodesoftWatchFolder = lines[16];
            MicrohiteCodesoftWatchFolder = lines[19];
            GlobalCodesoftExeLocation = lines[22];
            MicrohiteCodesoftExeLocation = lines[25];
            GlobalDruckerStationLocation = lines[28];
            MicroHiteDruckerStationLocation = lines[31];
            GlobalBoxLabelPrinter = lines[34];
            MicroHiteBoxLabelPrinter = lines[37];
            GlobalBarcodePrinter = lines[40];
            MicroHiteBarcodeLabelPrinter = lines[43];
        }




        private void label4_DoubleClick(object sender, EventArgs e)
        {
            Form squak = new Form2();
            squak.Show();

        }
        
    }
}
