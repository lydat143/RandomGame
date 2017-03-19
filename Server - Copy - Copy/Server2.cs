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
    public partial class Server2 : Form
    {
        public Server2()
        {
            InitializeComponent();
        }

        private SocketModel[] socketlist1;
        private SocketModel[] socketlist2;
        private TCPModel tcp;
        private int numOfClient = 200;
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
            socketlist2 = new SocketModel[numOfClient];
            for (int i = 0; i < numOfClient; i++) 
            {
                ServeAClient();
               
            }
        }

        public void AcceptPlayer()
        {
            int status = -1;
            Socket s = tcp.SetUpANewConnection( ref status);
            socketlist1[currentClient] = new SocketModel(s);

            string st = socketlist1[currentClient].GetRemoteEndpoint();
            string st1 = "New connect from: " + st;

            Console.WriteLine(st1);
            lbManageConnect.Items.Add(st1);

        }

        public void AccceptOpponent()
        {
            int status = -1;
            Socket s = tcp.SetUpANewConnection(ref status);
            socketlist2[currentClient] = new SocketModel(s);

        }

        public void ServeAClient()
        {
            AcceptPlayer();
            AccceptOpponent();

            currentClient++;

            Thread th = new Thread(Communicated);
            th.Start(currentClient - 1);
            
        }

        public void Communicated(object o)
        {
            int pos = (Int32)o;
            while (true)
            {
                string result = "";
                string str = socketlist1[pos].ReceiveData();
                Console.WriteLine(str);
                Random rd = new Random();
                int frequency = rd.Next(30, 50);
                result += frequency.ToString() + " ";
                for (int i = 0; i < frequency; i++)
                {
                    int k = rd.Next(1, 7);
                    result += k.ToString() + " ";
                }
                Broastcast(pos, result);
            }
        }

        public void Broastcast(int pos, string result)
        {
            socketlist1[pos].SendData(result);
            int i = pos % 2;
            if (i == 0)
            {
                socketlist2[pos + 1].SendData(result);
            }
            if (i == 1)
            {
                socketlist2[pos - 1].SendData(result);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.Text = "SERVER 2";
        }

    }
}
