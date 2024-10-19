using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_Client_File
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            var ip = IPAddress.Loopback;
            var port = 27001;
            var ep = new IPEndPoint(ip, port);
            var listener = new TcpListener(ep);
            Console.WriteLine("Server");
            
            try
            {
                listener.Start();
                while (true)
                {
                    var client= listener.AcceptTcpClient();
                    _ = Task.Run(() =>
                    {
                        Console.WriteLine($"{client.Client.RemoteEndPoint} is connected");
                        var networkStream = client.GetStream();
                        var remoteEp = client.Client.RemoteEndPoint as IPEndPoint;
                        var directoryPath = Path.Combine(Environment.CurrentDirectory, remoteEp!.Address.ToString());

                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);

                        var buffer = new byte[1024];
                        var fileNameBuilder = new StringBuilder();
                        int bytesRead;
                        while ((bytesRead = networkStream.Read(buffer, 0, 1)) > 0)
                        {
                            var character = Encoding.UTF8.GetString(buffer, 0, 1);
                            if (character == "\n")
                                break;
                            fileNameBuilder.Append(character);
                        }

                        var fileName = Path.Combine(directoryPath, fileNameBuilder.ToString().Trim());

                        using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            int len = 0;
                            var bytes = new byte[1024];

                            while ((len = networkStream.Read(bytes, 0, bytes.Length)) > 0)
                            {
                                fs.Write(bytes, 0, len);
                            }
                        };

                        Console.WriteLine("File recieved");
                        client.Close();

                    });


                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
