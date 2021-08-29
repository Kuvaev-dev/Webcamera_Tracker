using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsumerApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var port = int.Parse(ConfigurationManager.AppSettings.Get("port"));
            var client = new UdpClient(port);

            while (true)
            {
                var data = await client.ReceiveAsync();
                // Формирование картинки
                using (var ms = new MemoryStream(data.Buffer)) 
                {
                    pictureBox1.Image = new Bitmap(ms);
                }
                // Подсчёт количества байтов из поступающего пакета
                Text = $"Bytes received: {data.Buffer.Length * sizeof(byte)}";
            }
        }
        // Получение IP-адреса машины для отправки на него данных с камеры
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            // Через DNS-протокол получаем айпишник
            var host = Dns.GetHostEntry(Dns.GetHostName());
            MessageBox.Show(string.Join("\n", host.AddressList.Where(i => i.AddressFamily ==
            AddressFamily.InterNetwork).Select(i => i.ToString())));
        }
    }
}
