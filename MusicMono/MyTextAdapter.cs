
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;

using MusicMono.Portab.Search;
using MusicMono.Portab.SearchObjects;
namespace MusicMono
{
    class MyTextAdapter : BaseAdapter
    {

        EditText _searchBar;        
        Context cntx;
        public List<BaseSearchObject> CurrentList {
            get
            {
                return _search.CurrentList;
            }
        }
        private BaseSearch _search;
        public static Dictionary<SearchObjectState, Android.Graphics.Color> SongStateToColor = new Dictionary<SearchObjectState, Android.Graphics.Color>()
        {
            {SearchObjectState.Nothing, Android.Graphics.Color.Black },
            {SearchObjectState.Downloading, Android.Graphics.Color.Orange },
            {SearchObjectState.Downloaded, Android.Graphics.Color.Green },
            {SearchObjectState.DownloadError, Android.Graphics.Color.Red },
            {SearchObjectState.Deleted, Android.Graphics.Color.Gray },
        };
        public MyTextAdapter (Context cntx)
        {
            this.cntx = cntx;
            InitSerchBar();
            _search = new LastFmSearch();
            _search.OnChange += Search_OnChange;
        }

        private void Search_OnChange(object sender, EventArgs e)
        {
            (cntx as Activity).RunOnUiThread(() =>
            {
                NotifyDataSetChanged();
            });
        }

        public override int Count
        {
            get
            {
                    return CurrentList!=null ? CurrentList.Count + 1 : 1;
            }
        }

        public async Task FindItemsAsync(string Q)
        {
            if (Q.Length > 1)
                await _search.SearchAsync(Q);
            
        }
        public async Task NextPageAsync()
        {
            await _search.NextPageAsync();
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }
        bool inside = false;
        public async Task EndRechedAsync()
        {

            if (!inside && CurrentList.Count > 0)
            {
                inside = true;
                await NextPageAsync();
                Search_OnChange(this, EventArgs.Empty);
                inside = false;
            }
            

        }
        public void InitSerchBar()
        {
            _searchBar = new EditText(cntx)
            {
                LayoutParameters = new AbsListView.LayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)),
                Hint = "Search"
            };
            _searchBar.TextChanged += async (e, o) =>
            {
                if ((e as EditText).Text.Length > 2)
                    await this.FindItemsAsync((e as EditText).Text);
            };
            _searchBar.SetBackgroundColor(Android.Graphics.Color.AliceBlue);
            _searchBar.SetTextColor(Android.Graphics.Color.Black);
            _searchBar.SetHintTextColor(Android.Graphics.Color.DarkGray);
            _searchBar.SetShadowLayer(10, 5, 5, Android.Graphics.Color.Red);
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            
            if (position == 0)
            {
                return _searchBar;
            }
            TextView tv;
            if (convertView != null && convertView.GetType() == typeof(TextView))
                tv = convertView as TextView;
            else
                tv = new TextView(cntx);

            //(cntx as Activity).RunOnUiThread(() =>
            //{
            SongSearchObject CurrentSong = CurrentList[position - 1] as SongSearchObject;
            tv.LayoutParameters = new AbsListView.LayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
            tv.SetBackgroundColor(Android.Graphics.Color.DeepSkyBlue);
            tv.Text = CurrentSong.ToString();
            tv.SetTextSize(Android.Util.ComplexUnitType.Dip, 16);
            tv.SetTextColor(SongStateToColor[CurrentSong.SearchObjectState]);
            tv.Click += async (sener, evarg) =>
            {

                if (CurrentSong.SearchObjectState != SearchObjectState.Downloading && CurrentSong.SearchObjectState != SearchObjectState.Downloaded)
                {
                    try
                    {
                        Task down = Task.Run(async () =>
                        {

                            await Helper.DownloadMusic.DownalodMusicAsync(CurrentSong);
                        });
                        await down;
                    }
                    catch (Exception ex)
                    {
                        if (ex != null)
                            ex = null;
                    }
                }
            };
            //});
            return tv;
        }
    }
}
