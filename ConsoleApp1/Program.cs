using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();
            factory.UserName = "eaglevsandbox";
            factory.Password = "S4nboxv";
            factory.VirtualHost = "/eaglevsandbox";
            factory.HostName = "rmq1.pptik.id";
            factory.Port = 5672;
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "yolo",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");


                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    //JObject obj = JObject.Parse(message);
                    var path = @"C:\Users\ASUS\Desktop\"+message;
                    downloadftp(path,message);

                    Console.WriteLine(" [x] Received {0}", message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine(" [x] Done");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "yolo",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        private static bool downloadftp(String path,String filename)
        {
            try
            {

                var FTPHost = "ftp://eaglevsandboxftp.pptik.id/";
                var FTPResultDirectory = "eaglev_sandbox/";
                var FTPUsername = "eaglevsandbox";
                var FTPPassword = "S4nboxv";

                Console.WriteLine("Download :" + FTPHost + FTPResultDirectory + filename);
                WebClient request = new WebClient();
               
                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPHost + FTPResultDirectory + filename);
                //request.Method = WebRequestMethods.Ftp.DownloadFile;
                //request.UseBinary = true;
                //request.UsePassive = true;
                //request.KeepAlive = true;
                request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                byte[] fileData = request.DownloadData(FTPHost + FTPResultDirectory + filename);

                //FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //Stream responsetStream = response.GetResponseStream();

                using (FileStream file = File.Create(path))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                  
                    Console.WriteLine("Success Download FTP");
                }

                    return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed Download FTP");
                Console.WriteLine(e);
                return false;
            }

        }
    }
}
