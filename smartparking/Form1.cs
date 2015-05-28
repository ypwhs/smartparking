using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace smartparking
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TcpListener tcplistener8080;

        public void Send(string a, TcpClient tcpclient)
        {
            try
            {
                byte[] bs = Encoding.UTF8.GetBytes(a);
                tcpclient.Client.Send(bs, bs.Length, 0);
            }
            catch (Exception e)
            {

            }
        }
        TcpClient client;
        private void spy8080()
        {
            try
            {
                tcplistener8080 = new TcpListener(IPAddress.Any, 8080);
                tcplistener8080.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Spy8080 Failed.");
                return;
            }

            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    TcpClient tcpclient = tcplistener8080.AcceptTcpClient();
                    //NetworkStream stream = tcpclient.GetStream();
                    if (tcpclient != null)
                    {
                        client = tcpclient;
                        //client进入
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        Thread tSpy8080;

        private void Form1_Load(object sender, EventArgs e)
        {
            tSpy8080 = new Thread(new ThreadStart(spy8080));
            tSpy8080.IsBackground = true;
            tSpy8080.Start();

            Control.CheckForIllegalCrossThreadCalls = false;
            string[] ports = SerialPort.GetPortNames();
            int i;
            for (i = 0; i < ports.Length; i++) if (ports[i].Length < 6) comboBox1.Items.Add(ports[i]);

            if (ports.Length == 0)
            {
                MessageBox.Show("请连接接收器");
                //Close();
            }
            else
            {
                try
                {
                    comboBox1.SelectedIndex = 0;
                    button1_Click(null, null);
                }
                catch (Exception ex)
                {
                }

            }
            control = trackBar1.BackColor;

        }

        public void Show(String a)
        {
            MessageBox.Show(a);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == false)
            {
                serialPort1.PortName = comboBox1.Text;
                try
                {
                    serialPort1.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                button1.Text = "关闭";
                comboBox1.Enabled = false;
            }
            else
            {
                try
                {
                    serialPort1.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                button1.Text = "打开";
                comboBox1.Enabled = true;
            }
        }
        Color control;

        double[] juli = { 0, 0, 0, 0 };

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String rec = serialPort1.ReadLine();
            label1.Text += rec;
            if (label1.Text.Length > 200) label1.Text = "";
            Regex regex = new Regex(@"\[(\d*)\](.*)cm");
            MatchCollection mc = regex.Matches(rec);
            try
            {
                String reg = mc[0].Groups[1].Value;
                int id = Convert.ToInt32(reg);
                reg = mc[0].Groups[2].Value;
                double d = Convert.ToDouble(reg);
                juli[id - 1] = d;
                Send(rec + "\n", client);
            }catch(Exception ex)
            {

            }

            String x = "";
            int i; for (i = 0; i < 4; i++)
            {
                x += (i + 1) + "号车数据:" + juli[i] + "\n";
            }
            label2.Text = x;

            
            try
            {
                trackBar1.Value = (int)juli[0];
                if (trackBar1.Value < 7) trackBar1.BackColor = Color.Red;
                else trackBar1.BackColor = control;
                trackBar2.Value = (int)juli[1];
                if (trackBar2.Value < 7) trackBar2.BackColor = Color.Red;
                else trackBar2.BackColor = control;
                trackBar3.Value = (int)juli[2];
                if (trackBar3.Value < 7) trackBar3.BackColor = Color.Red;
                else trackBar3.BackColor = control;
                trackBar4.Value = (int)juli[3];
                if (trackBar4.Value < 7) trackBar4.BackColor = Color.Red;
                else trackBar4.BackColor = control;

            }
            catch (Exception ex)
            { }
        }

        private void killall(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

    }
}
