using System;
using RPCClient;

namespace RPCClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var RPCClient = new RPCClient();
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