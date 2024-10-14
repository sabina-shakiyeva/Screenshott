using System.IO;
using System.Net;
using System.Net.Sockets;


var port = 5000;
var ipAddress = IPAddress.Parse("127.0.0.1");
var ep = new IPEndPoint(ipAddress, port);

using var listener = new TcpListener(ep);
try
{
    listener.Start();
    Console.WriteLine("Server started...");

    while (true)
    {
        var client = listener.AcceptTcpClient();
        _ = Task.Run(() =>
        {
            Console.WriteLine($"{client.Client.RemoteEndPoint} connected");
            var stream = client.GetStream();


            int count = 1;
            try
            {
                while (true)
                {

                    string screenshotPath = $@"C:\Users\shaki\source\repos\Server\Server\ScreenPhotos\screenshot_{count++}.png";

                    using (var fs = new FileStream(screenshotPath, FileMode.Create, FileAccess.Write))
                    {
                        int len;
                        var buffer = new byte[1024];
                        while ((len = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, len);


                            if (len < buffer.Length)
                            {
                                break;
                            }
                          
                        }
                        if (len == 0)
                        {
                            Console.WriteLine("Client disconnected.");
                            break;
                        }
                    }

                    Console.WriteLine($"Screenshot saved at {screenshotPath}");
                    //try
                    //{
                    //    byte[] testBuffer = new byte[1];
                    //    stream.Write(testBuffer, 0, testBuffer.Length);
                    //}
                    //catch (IOException)
                    //{
                    //    Console.WriteLine("Client disconnected (detected via stream.Write).");
                    //    break;
                    //}


                    if (!client.Connected)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine($"{client.Client.RemoteEndPoint} disconnected");
            }
        });
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
