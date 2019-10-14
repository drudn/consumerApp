using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace consumer_app
{
    class Program
    {
        static void Main(string[] args)
        {

            String fAPath, fBPath;
            List<String> qAlines = new List<String>(); //Temporary list to store each message in queue A
            List<String> qBlines = new List<String>(); //Temporary list to store each message in queue B

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "qA",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    qAlines.Add(message);

                };

                channel.BasicConsume(queue: "qA",
                                    autoAck: true,
                                    consumer: consumer);
                Console.WriteLine("Finished copying from queue A. Press [enter] to continue.");
                Console.ReadLine();
            }

            //Write each value in list for qA to text file "WriteLinesQA.txt"
            using (StreamWriter fileA = new StreamWriter(@"WriteLinesQA.txt"))
            {
                fAPath = Path.GetFullPath(@"WriteLinesQA.txt");
                int i = 0;
                foreach (String line in qAlines)
                {
                    fileA.WriteLine(++i + ".     " + line);
                }
            }

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "qB",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    qBlines.Add(message);

                };

                channel.BasicConsume(queue: "qB",
                                    autoAck: true,
                                    consumer: consumer);

                Console.WriteLine("Finished copying from queue B. Press [enter] to continue.");
                Console.ReadLine();
            }
            
                //Write each value in list for qB to text file "WriteLinesQB.txt"
                using (StreamWriter fileB = new StreamWriter(@"WriteLinesQB.txt"))
                {
                    fBPath = Path.GetFullPath(@"WriteLinesQB.txt");
                    int i = 0;
                    foreach (String line in qBlines)
                    {
                        fileB.WriteLine(++i + ".     " + line);
                    }
                }

                Console.WriteLine("Finished writing to files:");
                Console.WriteLine(fAPath);
                Console.WriteLine(fBPath);

                Console.ReadLine();
            }
        }
    }

