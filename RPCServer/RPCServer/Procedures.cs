using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RPCServer
{
    class Procedures
    {
        public Procedures() { }

        public string auth(string userAndPswd)
        {
            FileInfo fd = new FileInfo("users.txt");
            int i = 0;
            StreamReader sr = fd.OpenText();

            int c = userAndPswd.IndexOf(" ");
            string user = userAndPswd.Remove(c);
            while (i == 0)
            {
                var str = sr.ReadLine();

                if (str == userAndPswd) i = 1;
                else if (sr.Peek() == -1)
                {
                    sr.Dispose();
                    return "Error login or password";
                }

            }
            sr.Dispose();
            return "success";
        }

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
