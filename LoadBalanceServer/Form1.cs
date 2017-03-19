using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;

// SERVER
namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private SocketModel[] socketlist1;
        private TCPModel tcp;
        private int numOfClient = 400;
        private int currentClient = 0;

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            StartServer();
            Thread th = new Thread(ServeClient);
            th.Start();
        }

        public void StartServer()
        {
            string IP = txtIP.Text;
            int Port = int.Parse(txtPort.Text);
            tcp = new TCPModel(IP, Port);
            tcp.Listen();
            btnStart.Enabled = false;
            Console.WriteLine("OK!");
        }

        public void ServeClient(){
            socketlist1 = new SocketModel[numOfClient];
            for (int i = 0; i < numOfClient; i++) 
            {
                ServeAClient();
               
            }
        }

        public void Accept()
        {
            int status = -1;
            Socket s = tcp.SetUpANewConnection( ref status);
            socketlist1[currentClient] = new SocketModel(s);

            string st = socketlist1[currentClient].GetRemoteEndpoint();
            string st1 = "New connect from: " + st;

            Console.WriteLine(st1);
            lbManageConnect.Items.Add(st1);

        }

        public void ServeAClient()
        {
            Accept();

            currentClient++;

            Thread th = new Thread(Communicated);
            th.Start(currentClient - 1);
            
        }

        public void Communicated(object o)
        {
            int pos = (Int32)o;
          
                int balance = pos % 4;
                if (balance == 0 || balance == 1)
                    socketlist1[pos].SendData("11000");
                if (balance == 2 || balance == 3)
                    socketlist1[pos].SendData("12000");
            
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.Text = "LOAD BALANCE SERVER";
        }

    }
}
