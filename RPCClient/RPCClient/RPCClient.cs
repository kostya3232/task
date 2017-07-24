using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace RPCClient
{
    class RPCClient
    {
        
        private IConnection connection;
        private IModel channel;
        
        private QueueingBasicConsumer consumer;
        private string user;
        private string ip;

        public RPCClient(string IP, string us)
        {
            ip = IP;
            user = us;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(user, false, false, false, null);
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(user, true, consumer);
            
        }

      

        //генерирует свойства сообщения: адрес обратой доставки и id сообщения,
        //отправляет и принимает ответ, с проверкой его id, блокируется пока не получит нужный ответ        
        public string Call(string message)
        {            
            var coreId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.ReplyTo = user;
            props.CorrelationId = coreId;
                        
            var messageByte = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", ip, props, messageByte);

            while (true)
            {
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                if (ea.BasicProperties.CorrelationId == coreId) return Encoding.UTF8.GetString(ea.Body);
            }
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
