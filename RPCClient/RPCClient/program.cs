using System;
using RPCClient;
using System.IO;

namespace RPCClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //ввод ip адресса, логина и пароля, проверка логина и пароля
            Console.Write("Enter IP server:");
            string IP = Console.ReadLine();
            FileInfo fd = new FileInfo("users.txt");
            int i = 0;
            StreamReader sr = fd.OpenText();

            Console.WriteLine("Enter username and password");
            string userAndPswd = Console.ReadLine();
            
            int c = userAndPswd.IndexOf(" ");
            string user = userAndPswd.Remove(c);
            while (i == 0){
                var str = sr.ReadLine();

                if (str == userAndPswd) i = 1;
                else if (sr.Peek() == -1)
                {
                    Console.WriteLine("Error login or password");
                    return;
                }
            }

            var RPCClient = new RPCClient(IP, user);
            string path = "";
            var response = "";

            while (true)
            {
               
                Console.WriteLine("Enter the path");
                path = Console.ReadLine();

                if (path == "end") break;

                response = RPCClient.Call(path);
                Console.WriteLine(response);                
            }

            RPCClient.Close();
        }
    }
}