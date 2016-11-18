using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using MusicMono.Portab.SearchObjects;
using System.Threading.Tasks;

namespace MusicMono.Helper
{
    static class DownloadMusic
    {
        public static async Task DownalodMusicAsync(BaseSearchObject CurrentSong)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    CurrentSong.SearchObjectState = SearchObjectState.Downloading;
                    wc.DownloadProgressChanged += (a, b) =>
                    {
                        CurrentSong.DownloadPercente = b.ProgressPercentage;
                    };
                    CurrentSong.PathToSearchObjectOnDisc = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, "Music", CurrentSong.Name + ".mp3");
                    var DownloadLink = await CurrentSong.FealLuckyDownloadLinkAsync();
                    await wc.DownloadFileTaskAsync(DownloadLink, CurrentSong.PathToSearchObjectOnDisc);
                        if ((new Java.IO.File(CurrentSong.PathToSearchObjectOnDisc)).Length() > 50000)
                    {
                        CurrentSong.SearchObjectState = SearchObjectState.Downloaded;
                    }
                    else
                    {
                        (new Java.IO.File(CurrentSong.PathToSearchObjectOnDisc)).Delete();
                        CurrentSong.SearchObjectState = SearchObjectState.DownloadError;
                    }
                }
            }catch(Exception ex)
            {
                CurrentSong.SearchObjectState = SearchObjectState.DownloadError;
                if (ex != null)
                    ex = null;
            }
        }
    }
}