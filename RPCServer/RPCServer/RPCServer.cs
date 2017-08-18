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
            string IP = "";
            string user = "";
            string passwd = "";

            var factory = new ConnectionFactory();
            IConnection connection;

            if (args.Length != 3)
            {
                user = "guest";
                passwd = "guest";
                IP = "localhost";
            }
            else
            {
                IP = args[0];
                user = args[1];
                passwd = args[2];
            }

            factory.UserName = user;
            factory.Password = passwd;
            factory.HostName = IP;

            try
            {
                connection = factory.CreateConnection();
            }
            catch(Exception e)
            {
                Console.WriteLine("error in ip, username or password");
                return;
            }
            
            using (var channel = connection.CreateModel())
            {                
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
                    string response = "";

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    //id ответного сообщения
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(ea.BasicProperties.ReplyTo + " " + message);

                    response = Proc.ListFiles(message);

                    var responseByte = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish("", props.ReplyTo, replyProps, responseByte);
                    channel.BasicAck(ea.DeliveryTag, false);
                };

                Console.WriteLine("Press [enter] to exit");
                Console.ReadLine();
                
            }
            
        }


    }
}