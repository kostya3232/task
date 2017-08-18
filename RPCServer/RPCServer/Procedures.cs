using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPCServer
{
    class Procedures
    {
        public Procedures() { }
       
        public string ListFiles(string path)
        {
            var resp = "";

            try
            {
                var response = Directory.GetDirectories(path);
                int n = response.Length;
                resp += "Directories:\n";
                for (int i = 0; i < n; i++)
                {
                    resp += response[i] + "\n";
                }
                response = Directory.GetFiles(path);
                n = response.Length;
                resp += "Files:\n";
                for (int i = 0; i < n; i++)
                {
                    resp += response[i] + "\n";
                }
            }
            catch (Exception e)
            {
                resp = "failed";
            }

            return resp;

        }
    }
}
