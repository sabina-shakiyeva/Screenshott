using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


var port = 5000;
var ipAddress = IPAddress.Parse("127.0.0.1");
var ep = new IPEndPoint(ipAddress, port);
var client = new TcpClient();

try
{
    client.Connect(ep);
    if (client.Connected)
    {
        var networkStream = client.GetStream();

        while (true)
        {

            Console.Write("Taking screenshot. Write 'exit' to stop or press Enter to continue:");

            string input = Console.ReadLine();
            if (input == "exit")
            {
                networkStream.Close();  
                client.Close(); 
                break;
                break;
            }


            string path = "screenshot";
            using (Bitmap bitmap = new Bitmap(1920, 1080))
            {
                using Graphics g = Graphics.FromImage(bitmap);
                g.CopyFromScreen(Point.Empty, Point.Empty, new Size(1920, 1080));
                bitmap.Save($"{path}.png", ImageFormat.Png);
            }


            var screenshotPath = $"{path}.png";
            using (var readFs = new FileStream(screenshotPath, FileMode.Open, FileAccess.Read))
            {
                int len = 0;
                var buffer = new byte[1024];
                while ((len = readFs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    networkStream.Write(buffer, 0, len);
                }
            }

            Console.WriteLine("Screenshot sent to server.");


            Thread.Sleep(10000);
        }


        networkStream.Close();
        client.Close();
        Console.WriteLine("Disconnected from server.");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}