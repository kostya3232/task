using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RPCServer
{
    class Program
    {
        private const string DEFAULT_ADDR = "localhost";
        private const string DEAULT_USERNAME = "guest";
        private const string DEFAULT_PASSWORD = "guest";

        static void Main(string[] args)
        {
            string user = DEAULT_USERNAME;
            string passwd = DEFAULT_PASSWORD;
            string ip = DEFAULT_ADDR;
            bool fl = true;
            
            if (args.Length == 3)
            {
                ip = args[0];
                user = args[1];
                passwd = args[2];
            }

            var rpcserver = new RPCServer(ip, user, passwd);
            if (rpcserver.Flag == -1) return;

            Console.WriteLine("Enter 'end' to exit");
            while (fl)
            {
                string str = "";
                str = Console.ReadLine();
                if (str == "end")
                {
                    fl = false;
                    rpcserver.RPCClose();
                }
            }
            
        }


    }
}