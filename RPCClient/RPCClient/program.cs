using System;

namespace RPCClient
{
    class Program
    {
        static void Main(string[] args)
        {            
            string user = "";
            string passwd = "";
            string IP = "";

            if (args.Length != 3)
            {
                IP = "localhost";
                user = "guest";
                passwd = "guest";
            }
            else
            {
                IP = args[0];
                user = args[1];
                passwd = args[2];
            }

            var RPCClient = new RPCClient(IP, user, passwd);
            if (RPCClient.flag == 1) return;
                        
            while (true)
            {
                Console.WriteLine("Enter the path: ");
                string command = Console.ReadLine();
               
                if (command == "exit") break;
                Console.WriteLine(RPCClient.Call(command));
            }

            RPCClient.Close();
            
            
        }
    }
}