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
        private string replyQueueName;
        private QueueingBasicConsumer consumer;

        public RPCClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(replyQueueName, true, consumer);
            
        }

        //генерирует свойства сообщения: адрес обратой доставки и id сообщения,
        //отправляет и принимает ответ, с проверкой его id, блокируется пока не получит нужный ответ        
        public string Call(string message)
        {            
            var coreId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = coreId;
                        
            var messageByte = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", "rpc_queue", props, messageByte);

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
