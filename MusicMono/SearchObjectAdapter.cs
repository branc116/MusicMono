using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicMono.Portab.Search;
using MusicMono.Portab.SearchObjects;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.RecyclerView;
using Android.Support.V7.Widget;
using System.Threading.Tasks;
using Java.IO;
using Android.Graphics;
using System.Net;

namespace MusicMono
{
    class SearchObjectAdapter : RecyclerView.Adapter
    {
        public LastFmSearch LastFmSearch { get; private set; }
        public event EventHandler<EventArgs> OnChanged;
        

        private Activity _acty;
        
        public SearchObjectAdapter(Activity acty)
        {
            LastFmSearch = new LastFmSearch();
            LastFmSearch.OnChange += LastFmSearch_OnChange;
            this._acty = acty;
        }

        private void LastFmSearch_OnChange(object sender, EventArgs e)
        {
            _acty.RunOnUiThread(() =>
            {
                base.NotifyDataSetChanged();
            });
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        public override int ItemCount
        {
            get
            {
                return LastFmSearch.CurrentList.Count == 0 ? 1 : LastFmSearch.CurrentList.Count;
            }
        }

        public async override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {   
            try
            {
                if (LastFmSearch.CurrentList.Count != 0) {
                    await (holder as SearchObjectViewHolder).UpdateViewHolderAsync(_acty,this,LastFmSearch.CurrentList[position],  position );

                }
                
            }catch (Exception ex)
            {
                if (ex != null)
                    ex = null;
            }
        
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            bool nothingToShow = LastFmSearch.CurrentList.Count == 0;
            View item = LayoutInflater.From(parent.Context).Inflate(nothingToShow ? Resource.Layout.NothingToShow : Resource.Layout.SearchObject, parent, false);
            RecyclerView.ViewHolder ReturnObj = null;
            if (nothingToShow)
                ReturnObj = new NothingToShowViewHolder(item);
            else
            {
                ReturnObj = new SearchObjectViewHolder(item);
                var so = ReturnObj as SearchObjectViewHolder;
                item.Click += async (sender, argz) =>
                {
                    if (so.ID >= 0 && LastFmSearch.CurrentList[so.ID].SearchObjectState != SearchObjectState.Downloading && LastFmSearch.CurrentList[so.ID].SearchObjectState != SearchObjectState.Downloaded)
                        await Helper.DownloadMusic.DownalodMusicAsync(LastFmSearch.CurrentList[so.ID]);
                };
                if (!LastFmSearch.CurrentList[so.ID].IsAnybodyLisening)
                    LastFmSearch.CurrentList[so.ID].OnChange += LastFmSearch_OnChange;
            }
            return ReturnObj;
        }
        public async Task QAsync(string q)
        {
            await LastFmSearch.SearchAsync(q);
        }
    }

    public class SearchObjectViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Name { get; private set; }
        public TextView Subtitle { get; private set; }
        public TextView Type { get; set; }
        public TextView Status { get; set; }
        public string PictureUrl { get; set; }
        public int ID { get; set; }
        public SearchObjectViewHolder(View itemView) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView2);
            Name = itemView.FindViewById<TextView>(Resource.Id.Title);
            Subtitle = itemView.FindViewById<TextView>(Resource.Id.Subtitle);
            Type = ItemView.FindViewById<TextView>(Resource.Id.TypeName);
            Status = itemView.FindViewById<TextView>(Resource.Id.Status);
        }
        internal async Task UpdateViewHolderAsync(Activity Acty, SearchObjectAdapter searchObjectAdapter, BaseSearchObject baseSearchObject, int position)
        {
            this.Name.Text = baseSearchObject.Name;
            this.Subtitle.Text = baseSearchObject.Subtitle;
            this.Status.Text = baseSearchObject.Status;
            this.Type.Text = baseSearchObject.Type;
            ID = position;
            if (baseSearchObject.RealImage != null)
            {
                var bitmap = BitmapFactory.DecodeByteArray(baseSearchObject.RealImage, 0, baseSearchObject.RealImage.Length);
                this.Image.SetImageBitmap(bitmap);
            }
            else
            {
                this.Image.SetImageResource(Resource.Drawable.PlaceHolder);
                await Helper.DownloadImage.DownloadImageAsync(Acty, searchObjectAdapter, position, baseSearchObject);
            }
        }
    }

    public class NothingToShowViewHolder : RecyclerView.ViewHolder
    {
        public NothingToShowViewHolder(View itemView) : base(itemView)
        {

        }
    }

}