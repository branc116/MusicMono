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

namespace MusicMono.Views
{
    class SearchObjectView : View
    {

        public ImageView Image { get; private set; }
        public TextView Name { get; private set; }
        public TextView Subtitle { get; private set; }
        public TextView Type { get; set; }
        public TextView Status { get; set; }
        public string PictureUrl { get; set; }
        public int ID { get; set; }
        public SearchObjectView(Context ctnx) : base(ctnx)
        {
        }
        public SearchObjectView(View View) : base(View.Context)
        {
            FillViews();
        }
        public void FillViews()
        {
            Image = base.FindViewById<ImageView>(Resource.Id.imageView2);
            Name = base.FindViewById<TextView>(Resource.Id.Title);
            Subtitle = base.FindViewById<TextView>(Resource.Id.Subtitle);
            Type = base.FindViewById<TextView>(Resource.Id.TypeName);
            Status = base.FindViewById<TextView>(Resource.Id.Status);
        }
    }
}