using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\ASUS\Pictures\kucing-anjing.jpg";
            var fname = "kucing-anjing.jpg";

        var factory = new ConnectionFactory();
            factory.UserName = "aa";
            factory.Password = "aa";
            factory.VirtualHost = "/aa";
            factory.HostName = "rmq1.aa.id";
            factory.Port = 5672;
            using (var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "yolo",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
          
            var message = fname;
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: "yolo",
                                 basicProperties: properties,
                                 body: body);
            Console.WriteLine("Sent ", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();


            uploadtoftp(path, fname);
        }

        //private static string GetMessage(string[] args)
        //{
            //return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        //}

        private static bool uploadtoftp(string localfile, String filename)
        {
            try
            {

                var FTPHost = "ftp://eaglevsandboxftp.pptik.id/";
                var FTPResultDirectory = "eaglev_sandbox/";
                var FTPUsername = "eaglevsandbox";
                var FTPPassword = "S4nboxv";

                Console.WriteLine("Uploading file to :" + FTPHost + FTPResultDirectory + filename);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPHost + FTPResultDirectory + filename);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = true;
                request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);


                using (FileStream fs = File.OpenRead(@localfile))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(buffer, 0, buffer.Length);
                    requestStream.Flush();
                    requestStream.Close();
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                    response.Close();
                    Console.WriteLine("Success Upload File to FTP");
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed Upload File to FTP");
                Console.WriteLine(e);
                return false;
            }

        }
    }
}
