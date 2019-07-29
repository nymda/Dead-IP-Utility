using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Dead_IP_Utility
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Dead IP Testing Utility. Made by knedit.");
            Console.WriteLine("A backup of the origional file will be stored in %appdata%.");
            Console.WriteLine("Overwriting will begin instantly.");
            string back = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/DeadIpBackup.txt";

            try
            {
                if (args[0] == ""){ }
            }
            catch
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nSpecify a file (or drag into executable).");
                Console.WriteLine("Program will exit.");
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(0);
            }

            File.Copy(args[0], back, true);

            List<String> IPs = new List<String> { };
            List<String> good = new List<String> { };

            int count = 0;

            int startpos = 1;

            IPs = File.ReadAllLines(args[0]).ToList();

            int len = IPs.Count();

            try
            {
                if (IPs[0].StartsWith("#"))
                {
                    Console.WriteLine("Last test: " + IPs[0].Substring(1));
                }
                else
                {
                    Console.WriteLine("Last test time not found.");
                    IPs.Insert(0, ("#" + DateTime.Now.ToString()));
                }

                Console.WriteLine("");

                good.Add("#" + DateTime.Now.ToString());

                for (int i = startpos; i < len; i++)
                {

                    string[] cur0 = IPs[i].Split(',');
                    string cur;
                    string port;
                    string user;
                    string pass;

                    if(cur0.Length == 2)
                    {
                        //probably ip:port,user,pass
                        string ipport = cur0[0];
                        user = cur0[1];
                        pass = cur0[2];
                        string[] ipporttemp = ipport.Split(':');
                        port = ipporttemp[1];
                        cur = ipporttemp[0];
                    }
                    else
                    {
                        //something else, probably ip:port
                        string[] ipporttemp = cur0[0].Split(':');
                        port = ipporttemp[1];
                        cur = ipporttemp[0];
                    }

                    Ping ping = new Ping();
                    PingReply PR = ping.Send(cur, 500);
                    if(PR.Status.ToString().Equals("Success"))
                    {
                        //good ip
                        good.Add(cur + ":" + port);
                        IPs[i] = cur + ":" + port;
                        File.WriteAllLines(args[0], IPs.ToArray());
                        Console.Write("\r" + i + "/" + len + "  (" + cur + ":" + port + " -> LIVE)     ");
                    }
                    else
                    {
                        //dead ip
                        count++;
                        IPs.RemoveAt(i);
                        File.WriteAllLines(args[0], IPs.ToArray());
                        Console.Write("\r" + i + "/" + len + "  (" + cur + ":" + port + " -> DEAD)     ");
                    }
                }

                File.WriteAllLines(args[0], good.ToArray());

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("\nTask complete. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
