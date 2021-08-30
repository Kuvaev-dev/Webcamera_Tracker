using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;

namespace ProducerApp
{
    class Program
    {
        // Скрытие консоли от пользователя
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hwnd, int nCMDShow);

        private static IPEndPoint consumerIpEndPoint;
        private static UdpClient client = new UdpClient();
        static void Main(string[] args)
        {
            // Получаем все порты
            var consumerIP = ConfigurationManager.AppSettings.Get("consumerIP");
            var consumerPort = int.Parse(ConfigurationManager.AppSettings.Get("consumerPort"));
            consumerIpEndPoint = new IPEndPoint(IPAddress.Parse(consumerIP), consumerPort);
            // Console.WriteLine(consumerIpEndPoint);

            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();

            // Скрытие консоли
            Console.WriteLine($"Port: {consumerPort}, ip: {consumerIP}");
            Console.WriteLine("\nPress Enter to hide the console...");
            Console.ReadLine();
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }
        // Отображение
        private static void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var bmp = new Bitmap(eventArgs.Frame, 800, 600);
            try
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();
                    client.Send(bytes, bytes.Length, consumerIpEndPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
