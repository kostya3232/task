using System;
using RPCClient;
using System.IO;

namespace RPCClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string user = "";
            //ввод ip адресса, логина и пароля, проверка логина и пароля
            Console.Write("Enter IP server:");
            string IP = Console.ReadLine();

            //ацтентификация
            while (true)
            {
                Console.WriteLine("Enter username and password");
                string command = Console.ReadLine();

                int c = command.IndexOf(" ");
                user = command.Remove(c);
                var Client = new RPCClient(IP, user);
                if (command == "exit") break;
                var resp = Client.Call("auth " + command);
                if (resp == "success")
                {
                    Client.Close();
                    break;
                }
                else
                {
                    Console.WriteLine(resp);
                }
                Client.Close();
            }

            var RPCClient = new RPCClient(IP, user);
            while (true)
            {
                Console.WriteLine("Enter the command");
                string command = Console.ReadLine();

                int k = command.IndexOf(" ");
                if (command.Remove(k) == "auth") RPCClient.IfAuth(command.Substring(k + 1, command.Substring(k + 1).IndexOf(" ")));
                if (command == "exit") break;
                Console.WriteLine(RPCClient.Call(command));
            }

            RPCClient.Close();

        }
    }
}