using System;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization;
using System.Windows.Media;
using Color = System.Drawing.Color;
using System.Windows.Forms.DataVisualization.Charting;

namespace Client
{
    public partial class Form1 : Form
    {
        public TcpClient client;
        long i=0;
        string savePath = @"C:\Users\serha\Desktop\GUNCEL PROJELER\OWLOOP\6.topl\SaveChart";


        public Form1()
        {
            InitializeComponent();
            client = new TcpClient();
            solidGauge1.From = 0;
            solidGauge1.To = 200;
            solidGauge2.From = 0;
            solidGauge2.To = 80;
            solidGauge3.To = 20;
            solidGauge3.From = 10;

            chart3.ChartAreas[0].AxisY.Maximum = 80;
            chart3.ChartAreas[0].AxisY.Minimum = -20;
            chart1.ChartAreas[0].AxisY.Maximum = 200;
            chart1.ChartAreas[0].AxisY.Minimum = 0;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (client == null || !client.Connected)
            {
                return;
            }

            try
            {
                NetworkStream stream = client.GetStream();

                if (stream.DataAvailable)
                {

                    double result;
                    byte[] data = new byte[2048];
                    int bytesRead = stream.Read(data, 0, data.Length);
                    string receivedData = Encoding.ASCII.GetString(data, 0, bytesRead);

                    string path = @"C:\Users\serha\Desktop\GUNCEL PROJELER\OWLOOP\6.topl\data.txt";
                    using (StreamWriter writer = new StreamWriter(path, true))
                    {
                        writer.WriteLine(receivedData);
                    }

                    // Parse the JSON string
                    var obj = JObject.Parse(receivedData);
                    var temperature = obj?["temperature"]?.ToString();
                    var denemesend = obj?["denemesend"]?.ToString();
                    var status = obj?["status"]?.ToString();
                    var speed = obj?["speed"]?.ToString();
                    //result = double.Parse(speed);


                    // Set the temperature value to label1
                    sicaklik.Text = temperature;
                    randomjson.Text = denemesend;
                    hiz.Text = speed;

                    //solidGauge1.Value = result;
                    statusLabel.Text = status;
                    listBox1.Items.Insert(0, receivedData);
                    if (status == "Connected")
                    {
                        panel2.BackColor = Color.LawnGreen;
                        panel2.Refresh();
                    }
                    solidGauge2.Value = double.Parse(temperature);
                    solidGauge3.Value = double.Parse(denemesend);
                    if (status == "Started")
                    {
                        panel2.BackColor = Color.Yellow;
                        panel2.Refresh();
                    }
                    if (status == "Moved")
                    {
                        panel2.BackColor = Color.Cyan;
                        panel2.Refresh();
                    }
                    if (status == "Break")
                    {
                        panel2.BackColor = Color.Red;
                        panel2.Refresh();
                    }

                    if(status == "All Stopped")
                    {
                        panel2.BackColor = Color.Red;
                        panel2.Refresh();

                        if (status == "Pod Stopped")
                        {
                            panel2.BackColor = Color.Red;
                            panel2.Refresh();
                            // Yeni eklenecek kodlar

                        }

                    }

                    solidGauge1.Value = double.Parse(speed);

                    this.chart1.Series[0].Points.AddXY(i,Int32.Parse(speed));
                    this.chart2.Series[0].Points.AddXY(i,denemesend);
                    this.chart3.Series[0].Points.AddXY(i, double.Parse(temperature));
                    i++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            string ipAddress = textBox1.Text;
            int portNumber;

            if (int.TryParse(textBox2.Text, out portNumber))
            {
                if (client == null || !client.Connected)
                {
                    try
                    {
                        client = new TcpClient(ipAddress, portNumber);
                        MessageBox.Show("Connected");
                        NetworkStream stream = client.GetStream();
                        string message = "connected";
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                        timer1.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid port number");
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (client == null || !client.Connected)
            {
                MessageBox.Show("Not connected");
                return;
            }

            NetworkStream stream = client.GetStream();
            string message = "Start";
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void moveBtn_Click(object sender, EventArgs e)
        {
            if (client == null || !client.Connected)
            {
                MessageBox.Show("Not connected");
                return;
            }

            NetworkStream stream = client.GetStream();
            string message = "Move";
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            if (client == null || !client.Connected)
            {
                MessageBox.Show("Not connected");
                return;
            }

            NetworkStream stream = client.GetStream();
            string message = "Stop";
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            //timer1.Stop();
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            if (client == null || !client.Connected)
            {
                MessageBox.Show("Not connected");
                return;
            }

            NetworkStream stream = client.GetStream();
            string message = "EStop";
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private void restartBtn_Click(object sender, EventArgs e)
        {
            if (client == null || !client.Connected)
            {
                Application.Restart(); // Uygulamayı yeniden başlat
                Environment.Exit(0); // Önceki uygulamayı sonlandır;
            }
            if (client != null && client.Connected)
            {
                NetworkStream stream = client.GetStream();
                string message = "Restart";
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                client.Close();
                MessageBox.Show("Disconnected");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // SaveFileDialog oluşturulması
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp";
            saveFileDialog1.Title = "Grafikleri Kaydet";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                // Her bir grafik nesnesi için ayrı bir dosya adı oluşturulması
                string file1 = Path.GetDirectoryName(saveFileDialog1.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + "_speed" + Path.GetExtension(saveFileDialog1.FileName);
                string file2 = Path.GetDirectoryName(saveFileDialog1.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + "_random" + Path.GetExtension(saveFileDialog1.FileName);
                string file3 = Path.GetDirectoryName(saveFileDialog1.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + "_temp" + Path.GetExtension(saveFileDialog1.FileName);

                // Her bir grafik nesnesinin ayrı ayrı kaydedilmesi
                if (saveFileDialog1.FilterIndex == 1)
                {
                    chart1.SaveImage(file1, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Jpeg);
                    chart2.SaveImage(file2, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Jpeg);
                    chart3.SaveImage(file3, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Jpeg);
                }
                else if (saveFileDialog1.FilterIndex == 2)
                {
                    chart1.SaveImage(file1, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
                    chart2.SaveImage(file2, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
                    chart3.SaveImage(file3, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
                }
                else if (saveFileDialog1.FilterIndex == 3)
                {
                    chart1.SaveImage(file1, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Bmp);
                    chart2.SaveImage(file2, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Bmp);
                    chart3.SaveImage(file3, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Bmp);
                }

                MessageBox.Show("Grafikler kaydedildi!");
            }
        }
    }
}
