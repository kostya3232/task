using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.IO;

namespace RPCServer
{
    class RPCServer
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //ввод ip адресса, логина и пароля, проверка логина и пароля
                    Console.Write("Enter IP server:");
                    string IP = Console.ReadLine();
                    FileInfo fd = new FileInfo("usernames.txt");
                    int i = 0;
                    StreamReader sr = fd.OpenText();

                    Console.WriteLine("Enter username and password");
                    string userAndPswd = Console.ReadLine();

                    int c = userAndPswd.IndexOf(" ");
                    string user = userAndPswd.Remove(c);
                    while (i == 0)
                    {
                        var str = sr.ReadLine();

                        if (str == userAndPswd) i = 1;
                        else if (sr.Peek() == -1)
                        {
                            Console.WriteLine("Error login or password");
                            return;
                        }
                    }

                    //создаем очередь с отправлением сообщения незанятому работнику
                    channel.QueueDeclare(IP, false, false, false, null);
                    channel.BasicQos(0, 1, false);

                    //прием сообщения
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(IP, false, consumer);
                    Console.WriteLine("waiting for RPC requests");

                    consumer.Received += (model, ea) =>
                    {
                        var Proc = new Procedures();
                        
                        

                        var body = ea.Body;
                        var props = ea.BasicProperties;                        
                        //id ответного сообщения
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;
                        
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(ea.BasicProperties.ReplyTo + " " + message);

                        var responseByte = Encoding.UTF8.GetBytes(Proc.ListFiles(message));
                        channel.BasicPublish("", props.ReplyTo, replyProps, responseByte);
                        channel.BasicAck(ea.DeliveryTag, false);                        

                    };
                    
                    Console.WriteLine("Press [enter] to exit");
                    Console.ReadLine();

                    

                }
            }
        }

        
    }
}