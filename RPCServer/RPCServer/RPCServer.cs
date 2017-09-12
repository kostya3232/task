using System;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Collections.Generic;

namespace RPCServer
{
    class RPCServer
    {
        protected IConnection _connection;
        protected IModel _channel;
        protected List<string> _message = new List<string>();

        protected EventingBasicConsumer _consumer;
        protected string _user;
        protected string _ip;
        public int Flag { get; } = 0;

        public RPCServer(string ip, string user, string pass)
        {
            _ip = ip;
            _user = user;

            var _factory = new ConnectionFactory();

            _factory.UserName = _user;
            _factory.Password = pass;
            _factory.HostName = _ip;

            try
            {
                _connection = _factory.CreateConnection();
            }
            catch (Exception)
            {
                Console.WriteLine("error in ip, username or password");
                Flag = 0;
                return;
            }

            _channel = _connection.CreateModel();

            //создаем очередь с отправлением сообщения незанятому работнику
            _channel.QueueDeclare(_ip, false, false, false, null);
            _channel.BasicQos(0, 1, false);

            //прием сообщения
            _consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(ip, false, _consumer);
            Console.WriteLine("waiting for RPC requests");
            _consumer.Received += (model, ea) =>
            {
                
                string response = "";

                var body = ea.Body;
                var props = ea.BasicProperties;
                //id ответного сообщения
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var mess = Encoding.UTF8.GetString(body);
                                
                _message.Add(mess);
                Console.WriteLine(ea.BasicProperties.ReplyTo + " " + mess);
                
                response = ListFiles(_message);
                _message.Clear();

                var responseByte = Encoding.UTF8.GetBytes(response);
                _channel.BasicPublish("", props.ReplyTo, replyProps, responseByte);
                _channel.BasicAck(ea.DeliveryTag, false);
            };
        }

       
        protected string ListFiles(List<string> mess)
        {
            var resp = "";
            string path = String.Join(null,mess);

            try
            {
                var response = Directory.GetDirectories(path);
                int n = response.Length;
                resp += "Directories:" + Environment.NewLine;
                for (int i = 0; i < n; i++)
                {
                    resp += response[i] + Environment.NewLine;
                }
                response = Directory.GetFiles(path);
                n = response.Length;
                resp += "Files:" + Environment.NewLine;
                for (int i = 0; i < n; i++)
                {
                    resp += response[i] + Environment.NewLine;
                }
            }
            catch (Exception)
            {
                resp = "failed";
            }

            return resp;

        }

        public void RPCClose()
        {
            _connection.Close();
        }
    }
}
