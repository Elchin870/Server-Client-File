using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ip = IPAddress.Loopback;
            var port = 27001;
            var ep = new IPEndPoint(ip, port);
            Console.WriteLine("Client");
            var client = new TcpClient();

            try
            {
                client.Connect(ep);
                if (client.Connected)
                {
                    Console.Write("Enter file path: ");
                    var filePath = Console.ReadLine()!;
                    using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        var fileName = Path.GetFileName(filePath);
                        var networkStream = client.GetStream();
                        var fileNameBytes = Encoding.UTF8.GetBytes(fileName + "\n");
                        networkStream.Write(fileNameBytes, 0, fileNameBytes.Length);
                        var bytes=new byte[1024];
                        var len = 0;


                        while ((len = fs.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            networkStream.Write(bytes, 0, len);
                        }

                        networkStream.Close();
                        client.Close();
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
