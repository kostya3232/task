using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RPCClient
{
    class RPCClient
    {

        protected IConnection _connection;
        protected IModel _channel;

        protected QueueingBasicConsumer _consumer;
        protected string _user;
        protected string _ip;
        public int Flag { get; } = 0;


        public RPCClient(string ip, string us, string pass)
        {
            _ip = ip;
            _user = us;
            var _factory = new ConnectionFactory();
            _factory.HostName = _ip;
            _factory.UserName = _user;
            _factory.Password = pass;
            try
            {
                _connection = _factory.CreateConnection();
                Flag = 0;
            }
            catch (Exception)
            {
                Console.WriteLine("error in login or password");
                Flag = -1;
                return;
            }
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_user, false, false, false, null);
            _consumer = new QueueingBasicConsumer(_channel);
            _channel.BasicConsume(_user, true, _consumer);

        }



        //генерирует свойства сообщения: адрес обратой доставки и id сообщения,
        //отправляет и принимает ответ, с проверкой его id, блокируется пока не получит нужный ответ        
        public string Call(string message)
        {
            var coreId = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.ReplyTo = _user;
            props.CorrelationId = coreId;

            var messageByte = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish("", _ip, props, messageByte);

            while (true)
            {
                var _ea = _consumer.Queue.Dequeue();
                if (_ea.BasicProperties.CorrelationId == coreId)
                {
                    return Encoding.UTF8.GetString(_ea.Body);
                }
            }
        }

        public void Close()
        {
            _connection.Close();
        }

        
    }
}
