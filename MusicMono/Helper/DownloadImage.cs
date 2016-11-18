using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MusicMono.Portab.SearchObjects;
using System.Net;
using Android.Support.V7.Widget;

namespace MusicMono.Helper
{
    static class DownloadImage
    { 
        public static async Task DownloadImageAsync(Activity acty, RecyclerView.Adapter Adapter, int position, BaseSearchObject SearchObject)
        {
            if (!string.IsNullOrWhiteSpace(SearchObject.Image))
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {


                        SearchObject.RealImage = await wc.DownloadDataTaskAsync(new Uri(SearchObject.Image));
                        acty.RunOnUiThread(() =>
                        {
                            Adapter.NotifyDataSetChanged();
                        });
                    }
                }
                catch (Exception ex)
                {
                    if (ex != null)
                        ex = null;
                }
            }
        }
    }
}