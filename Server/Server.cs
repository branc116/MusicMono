    using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
namespace Server
{
    class Server
    {
        HttpListener lisenr;
        Pcs pcs;
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        public Server()
        {
            pcs = new Pcs();
            lisenr = new HttpListener();
            //foreach (var i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            //    foreach (var ua in i.GetIPProperties().UnicastAddresses) {
            //        try
            //        {
            //            lisenr.Prefixes.Add(string.Format("http://{0}:45678/main/", ua.Address));
            //        }
            //        catch { }
            //    }
            //lisenr.Prefixes.Add(string.Format("http://{0}:45678/main/", IPAddress.Loopback));
            lisenr.Prefixes.Add(string.Format("http://{0}:45678/main/", "*"));
            //lisenr.Prefixes = new HttpListenerPrefixCollection( string.Format("http://{0}:55678/main/", "*"));
            //lisenr.Prefixes.Add(string.Format("http://{0}:45678/main/", "192.168.1.2"));
            //lisenr.Prefixes.Add(string.Format("http://{0}:45678/main/", "93.142.208.160"));
            //lisenr.Prefixes.Add(string.Format("http://{0}:45678/main/", "192.168.1.1"));
            //lisenr.Prefixes.Add(string.Format("http://{0}:45678/main/", "10.103.132.27"));
            //lisenr.Prefixes.Add(string.Format("http://{0}:45678/main/", "xoxoxoms.ddns.net"));
            //  lisenr.Prefixes.Add(string.Format("http://{0}:45678/main/", IPAddress.IPv6Loopback));
            lisenr.Start();
            Console.WriteLine("Started Lisening on:");
            foreach (var ur in lisenr.Prefixes)
            {
                Console.WriteLine(ur);
            }
            Task t = ListenAsync();
        }
        public string ListUsers()
        {
            return pcs.ToString();
        }
        public Pc GimePc(string id)
        {
            return pcs[id];
        }
        public Pc GimePc(int i)
        {
            return pcs[i];
        }
        async Task ListenAsync()
        {
            while (true)
            {
                try
                {
                    var a = await lisenr.GetContextAsync();

                    Console.WriteLine("Its somethin " + a.Request.RemoteEndPoint.Address.ToString());
                    string id;
                    try
                    {
                        id = a.Request.Headers["id"];
                        a.Response.AppendHeader("id", id);
                        if (id == string.Empty)
                        {
                            id = Guid.NewGuid().ToString().Replace("-", string.Empty);
                            a.Response.AppendHeader("id", id);
                        }
                    }
                    catch
                    {
                        id = "fuckYou";
                    }
                    Pc cur;
                    if (id == null)
                        id = "FuckYou";
                    try
                    {
                        cur = pcs[id];
                    }
                    catch
                    {
                        pcs.AddPc(new Pc(id, a.Request.RemoteEndPoint.Address.ToString()));
                    }
                    a.Response.StatusCode = 200;
                    string msg = string.Format("Your url is: http://{0}:45678/main/{1}/",GetLocalIPAddress(), id);
                    await a.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(msg), 0, Encoding.UTF8.GetByteCount(msg));
                    a.Response.Close();
                    
                }catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

    }
    
}