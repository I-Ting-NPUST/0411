using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualBasic.PowerPacks;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        UdpClient U;
        Thread Th;

        ShapeContainer C;//畫布物件
        ShapeContainer D;//畫布物件
        Point stP;//繪圖起點
        string p;//筆畫座標字串

        public Form1()
        {
            InitializeComponent();
        }



        private void Listen()
        {   
            //接聽
            int Port = int.Parse(textBox3.Text);
            U = new UdpClient(Port);

            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);

            while (true)
            {
                byte[] B = U.Receive(ref EP);
                string A = Encoding.Default.GetString(B);   //接受到的B 翻譯陣列為字串A
                string[] Q = A.Split('/');  // 因用 / 分開所以用此做為分隔
                Point[] R = new Point[Q.Length];

                for(int i = 0; i < Q.Length; i++)
                {
                    string[] K = Q[i].Split(',');
                    R[i].X = int.Parse(K[0]);   //取得X座標
                    R[i].Y = int.Parse(K[1]);   //取得Y座標
                }

                for(int i = 0; i < Q.Length - 1; i++)
                {
                    LineShape L = new LineShape();//把Point畫到畫布上
                    L.StartPoint = R[i];
                    L.EndPoint = R[i + 1];
                    L.Parent = D;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Th.Abort();//關閉監聽執行續
                U.Close();//關閉監聽器
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);//建立監聽執行續
            Th.Start();
            button1.Enabled = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            C = new ShapeContainer();//建立畫布
            this.Controls.Add(C);//加入畫布C 到Form
            D = new ShapeContainer();
            this.Controls.Add(D);//加入畫布D 到Form
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            stP = e.Location;
            p = stP.X.ToString() + "," + stP.Y.ToString();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LineShape L = new LineShape();//建立線段物件
                L.StartPoint = stP;//線段起點
                L.EndPoint = e.Location;//線段終點
                L.Parent = C;//線段加入畫布C
                stP = e.Location;//終點變起點
                p += "/" + stP.X.ToString() + "," + stP.Y.ToString();//持續記錄座標
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {   
            int Port = int.Parse(textBox_Port.Text);
            UdpClient S = new UdpClient(textBox_IP.Text, Port);
            byte[] B = Encoding.Default.GetBytes(p);
            S.Send(B, B.Length);
            S.Close();

        }
    }
}
