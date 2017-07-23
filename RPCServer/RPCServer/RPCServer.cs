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
                    //создаем очередь с отправлением сообщения незанятому работнику
                    channel.QueueDeclare("rpc_queue", false, false, false, null);
                    channel.BasicQos(0, 1, false);

                    //прием сообщения
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume("rpc_queue", false, consumer);
                    Console.WriteLine("waiting for RPC requests");

                    consumer.Received += (model, ea) =>
                    {
                        
                        int n = 0;
                        string resp = "";

                        var body = ea.Body;
                        var props = ea.BasicProperties;                        
                        //id ответного сообщения
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;

                        //получаем список файлов в директории

                        try
                        {
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(ea.BasicProperties.ReplyTo + " " + message);
                            var response = Directory.GetDirectories(message);
                            n = response.Length;
                            resp += "Directories:\n";
                            for (int i = 0; i < n; i++)
                            {
                                resp += response[i] + "\n";
                            }
                            response = Directory.GetFiles(message);
                            n = response.Length;
                            resp += "Files:\n";
                            for (int i = 0; i < n; i++)
                            {
                                resp += response[i] + "\n";
                            }
                        }
                        catch (Exception e)
                        {
                            resp = "failed";
                        }

                        finally
                        {
                            var responseByte = Encoding.UTF8.GetBytes(resp);
                            channel.BasicPublish("", props.ReplyTo, replyProps, responseByte);
                            channel.BasicAck(ea.DeliveryTag, false);
                        }

                    };
                    
                    Console.WriteLine("Press [enter] to exit");
                    Console.ReadLine();

                    

                }
            }
        }

        
    }
}