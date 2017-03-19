using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;


//CLIENT
namespace gamesucxac
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }

        private TCPModel tcpforplayer;
        private TCPModel tcpforOpponent;
        private TCPModel tcptochooseServer;
        string kq = "";
        int port;
        string IP;

        public void DetectServer()
        {          
            tcptochooseServer = new TCPModel(IP, port);
            tcptochooseServer.ConnectToServer();
            string Port = tcptochooseServer.ReadData();
            port = int.Parse(Port);
        }

        public int Connect()
        {
            try {
                tcpforplayer = new TCPModel(IP, port);
                int error = tcpforplayer.ConnectToServer();
                if (error == -1)
                {
                    port = 8080; // port root server
                    Connect();
                }
                else
                {
                    this.Text = tcpforplayer.UpdateInformation();
                    tcpforOpponent = new TCPModel(IP, port);
                    tcpforOpponent.ConnectToServer();
                }           
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR......" + e.StackTrace);
                return -1;
            }
            return 1;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            IP = txtIP.Text; // IP balance server
            port = int.Parse(txtPort.Text); // Port balance server

            DetectServer(); // connect to balance server and choose port
            Connect(); // connect to server 1 or 2
            
            btnPlay.Enabled = true;
            btnConnect.Enabled = false;
            Thread t = new Thread( ListenOpponent );
            t.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnNext.Enabled = false;
            btnPlay.Enabled = false;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            txtPointOfOpponent.Text = "";
            txtMam.Text = "";
            txtPointOfPlayer.Text = "";
            btnPlay.Enabled = true;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (txtMoney.Text != "")
            {
                btnPlay.Enabled = false;
                btnNext.Enabled = true;
                int error = tcpforplayer.SendData("tung");
                if (error == -1)
                {
                    port = 8080; // port root server
                    Connect();
                }
                string result = tcpforplayer.ReadData();
                Thread th = new Thread(mamquay);
                th.Start(result);
                th.Join();
                txtPointOfPlayer.Text = kq;
                if (txtPointOfOpponent.Text != "" && txtPointOfPlayer.Text != "")
                    UpdatePrize();
            }
        }

        public void mamquay(object obj)
        {
            string r = (string)obj;
            string[] result = r.Split(' ');
            int num = int.Parse(result[0]);
            kq = result[num];
            for (int i = 0; i < num + 1; i++)
            {
                txtMam.Text = result[i];
                Thread.Sleep(10);
            }
        }

        public void ListenOpponent()
        {
            while(true){
                string result = tcpforOpponent.ReadData();
                Thread th = new Thread(mamquay);
                th.Start(result);
                th.Join();
                txtPointOfOpponent.Text = kq;
                if (txtPointOfOpponent.Text != "" && txtPointOfPlayer.Text != "")
                    UpdatePrize();
            }
        }

        public void UpdatePrize()
        {
            int prize = int.Parse(txtMoney.Text);
            int pointofplayer = int.Parse(txtPointOfPlayer.Text);
            int pointofOpponent = int.Parse(txtPointOfOpponent.Text);
            
                if (pointofplayer > pointofOpponent)
                {
                    prize += 200;
                }
                if (pointofplayer < pointofOpponent)
                {
                    prize -= 200;
                }
            
            txtMoney.Text = prize.ToString();

            if (prize == 0)
                MessageBox.Show("Bạn đã thua!");
        }

    }
}
