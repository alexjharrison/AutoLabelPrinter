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
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        public string CodesoftLabelTemplatesLocationGlobal, CodesoftLabelTemplatesLocationMicrohite, CodesoftLabelTemplateLocation;
        public int intervalSeconds;
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        ProcessStartInfo theDruck = new ProcessStartInfo();
        ProcessStartInfo csStart = new ProcessStartInfo();
        
        


        public Form1()
        {
            InitializeComponent();
            t.Tick += new EventHandler(timer_Tick);
            theDruck.CreateNoWindow = true;
            theDruck.UseShellExecute = false;
            theDruck.WindowStyle = ProcessWindowStyle.Hidden;
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
                CodesoftLabelTemplateLocation = CodesoftLabelTemplatesLocationGlobal;

            }
            else
            {
                CMMID = MicrohiteID;
                CodeSoftWatchFolder = MicrohiteCodesoftWatchFolder;
                CodesoftExe = MicrohiteCodesoftExeLocation;
                DruckerStation = MicroHiteDruckerStationLocation;
                BoxPrinter = MicroHiteBoxLabelPrinter;
                BarcodePrinter = MicroHiteBarcodeLabelPrinter;
                CodesoftLabelTemplateLocation = CodesoftLabelTemplatesLocationMicrohite;
            }
            statusLabel.Text = "Monitoring...";
            statusLabel.BackColor = System.Drawing.Color.LightGreen;
            onOffButton.Text = "Press to Stop";
            radioGlobal.AutoCheck = false;
            radioMicroHite.AutoCheck = false;
            
            
            //timer start
            timer_Tick(null,null);
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
            t.Stop();

            //run codesoft if turned off
            if(!IsCSOpen())
            {
                csStart.CreateNoWindow = true;
                csStart.UseShellExecute = false;
                csStart.WindowStyle = ProcessWindowStyle.Hidden;
                csStart.FileName = CodesoftExe;
                csStart.Arguments = " /CMD " + CodeSoftWatchFolder;
                using (Process exeProcess = Process.Start(csStart))
                    exeProcess.WaitForExit(100);
            }


            statusLabel.Text = "Processing Barcode/Box Labels";
            string batchID = fileToPrint.Substring( fileToPrint.IndexOf("usso-box-") + 9 , 6);
            string[] lines = System.IO.File.ReadAllLines(fileToPrint);
            lines[1] = BoxPrinter;
            string labelTemplate = lines[0].Substring(lines[0].LastIndexOf('\\') + 1);
            lines[0] = "LABELNAME = \"" + CodesoftLabelTemplateLocation + labelTemplate;
            string fileToPrintFileNameOnly = fileToPrint.Substring(fileToPrint.LastIndexOf('\\') + 1);
            System.IO.File.WriteAllLines(fileToPrint,lines);
            System.IO.File.Copy(fileToPrint, boxLabelLocation + "\\archiv\\" + fileToPrintFileNameOnly,true);
            System.IO.File.Move(fileToPrint, CodeSoftWatchFolder + "\\" + fileToPrintFileNameOnly);

            //see if corresponding rfid file exists, loop until there
            string rfidFiletoPrint = "";
            while (rfidFiletoPrint=="")
            {
                System.Threading.Thread.Sleep(10000);
                string[] folderContents = Directory.GetFiles(RFIDLabelLocation);
                for (int i = 0; i < folderContents.Length; i++)
                    if (folderContents[i].Contains(batchID))
                        rfidFiletoPrint = (folderContents[i]);
            }

            string RFIDfileToPrintFileNameOnly = rfidFiletoPrint.Substring(rfidFiletoPrint.LastIndexOf('\\') + 1);
            string[] rfidLines = System.IO.File.ReadAllLines(rfidFiletoPrint);
            
            //if zenostar process/print barcode label
            if(rfidLines[1].Contains("ZENOSTAR"))
            {
                string barcodeFiletoPrint = "";
                while (barcodeFiletoPrint == "")
                {
                    System.Threading.Thread.Sleep(2000);
                    string[] barcodeFolderContents = Directory.GetFiles(barcodeLabelLocation);
                    for (int i = 0; i < barcodeFolderContents.Length; i++)
                        if (barcodeFolderContents[i].Contains(batchID))
                            barcodeFiletoPrint = (barcodeFolderContents[i]);
                }
                string barcodeFileToPrintNameOnly = barcodeFiletoPrint.Substring(barcodeFiletoPrint.LastIndexOf('\\') + 1);
                string[] barcodeLines = System.IO.File.ReadAllLines(barcodeFiletoPrint);
                barcodeLines[1] = BarcodePrinter;

                string labelTemplate2 = barcodeLines[0].Substring(barcodeLines[0].LastIndexOf('\\') + 1);
                barcodeLines[0] = "LABELNAME = \"" + CodesoftLabelTemplateLocation + labelTemplate2;

                System.IO.File.WriteAllLines(barcodeFiletoPrint,barcodeLines);
                System.IO.File.Copy(barcodeFiletoPrint, barcodeLabelLocation + "\\archiv\\" + barcodeFileToPrintNameOnly,true);
                System.IO.File.Move(barcodeFiletoPrint, CodeSoftWatchFolder + "\\" + barcodeFileToPrintNameOnly);
            }

            statusLabel.Text = "Printing RFID Labels";
            string firstDisc = rfidLines[1].Substring(rfidLines[1].IndexOf(batchID) + 7, 3);
            string lastDisc = rfidLines[rfidLines.Length - 1].Substring(rfidLines[rfidLines.Length - 1].IndexOf(batchID) + 7, 3);
            System.IO.File.Copy(rfidFiletoPrint, RFIDLabelLocation + "\\archiv\\" + RFIDfileToPrintFileNameOnly.Substring(0,RFIDfileToPrintFileNameOnly.Length-4) + " #" + firstDisc + "-" + lastDisc + ".csv",true);
            for (int i = 1; i < rfidLines.Length; i++)
            {
                string[] singleRFIDtoWrite = {rfidLines[0],rfidLines[i]};
                if (rfidLines[i].Contains("ZENOSTAR_RETAIN"))
                    theDruck.Arguments = " /P \"G:\\Prod_Labels\\Zirconia\\RFID Labels\\Microhite\\Wieland_CE0123_Retain.txt\" /RFID On  /B \"" + RFIDLabelLocation + "\\TemporaryCopy.csv\"  /start /hidden";
                else if (rfidLines[i].Contains("ZENOSTAR"))
                    theDruck.Arguments = " /P \"G:\\Prod_Labels\\Zirconia\\RFID Labels\\Microhite\\Wieland_CE0123.txt\" /RFID On  /B \"" + RFIDLabelLocation + "\\TemporaryCopy.csv\"  /start /hidden";
                else if (rfidLines[i].Contains("ZIRCAD_RETAIN"))
                    theDruck.Arguments = " /P \"G:\\Prod_Labels\\Zirconia\\RFID Labels\\Microhite\\Ivoclar_CE0123_retain.txt\" /RFID On  /B \"" + RFIDLabelLocation + "\\TemporaryCopy.csv\"  /start /hidden";
                else if (rfidLines[i].Contains("ZIRCAD"))
                    theDruck.Arguments = " /P \"G:\\Prod_Labels\\Zirconia\\RFID Labels\\Microhite\\Ivoclar_CE0123.txt\" /RFID On  /B \"" + RFIDLabelLocation + "\\TemporaryCopy.csv\"  /start /hidden";
                else if (rfidLines[i].Contains("ZIRLUX_RETAIN"))
                    theDruck.Arguments = " /P \"G:\\Prod_Labels\\Zirconia\\RFID Labels\\Microhite\\Zirlux FC2 With Ring_Retain.txt\" /RFID Off  /B \"" + RFIDLabelLocation + "\\TemporaryCopy.csv\"  /start /hidden";
                else if (rfidLines[i].Contains("ZIRLUX"))
                    theDruck.Arguments = " /P \"G:\\Prod_Labels\\Zirconia\\RFID Labels\\Microhite\\Zirlux FC2 With Ring.txt\" /RFID Off  /B \"" + RFIDLabelLocation + "\\TemporaryCopy.csv\"  /start /hidden";

                System.IO.File.WriteAllLines(RFIDLabelLocation + "\\TemporaryCopy.csv", singleRFIDtoWrite);
                theDruck.FileName = DruckerStation;
                using (Process exeProcess = Process.Start(theDruck))
                    exeProcess.WaitForExit();
                



            }
            
            System.IO.File.Delete(RFIDLabelLocation + "\\TemporaryCopy.csv");
            System.IO.File.Delete(rfidFiletoPrint);



            statusLabel.Text = "Monitoring...";
            t.Start();
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
            intervalSeconds = Convert.ToInt32(lines[46]);
            CodesoftLabelTemplatesLocationGlobal = lines[49];
            CodesoftLabelTemplatesLocationMicrohite = lines[52];

            t.Interval = intervalSeconds * 1000; // specify interval time as you want
        }

        


        private void label4_DoubleClick(object sender, EventArgs e)
        {
            Form squak = new Form2();
            squak.Show();

        }

        public bool IsCSOpen()
        {
            foreach (Process clsProcess in Process.GetProcesses())
                if (clsProcess.ProcessName==("CS"))
                    return true;
            return false;
        } 

        
    }
}
