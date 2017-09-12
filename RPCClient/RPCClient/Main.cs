using System;

namespace RPCClient
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

            var RPCClient = new RPCClient(ip, user, passwd);
            if (RPCClient.Flag == -1) return;
                        
            while (fl)
            {
                Console.WriteLine("Enter the path: ");
                string command = Console.ReadLine();

                if (command == "exit")
                {
                    fl = false;
                }
                else
                {
                    Console.WriteLine(RPCClient.Call(command));
                }
            }

            RPCClient.Close();
            
            
        }
    }
}