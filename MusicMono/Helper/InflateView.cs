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
using MusicMono.Portab.SearchObjects;
using MusicMono.Views;

namespace MusicMono.Helper
{
    static class InflateView
    {
        public static SearchObjectView SearchObjectInflateView(BaseSearchObject SearchObject, Context Context, ViewGroup viewGroup)
        {
            var item = LayoutInflater.From(Context).Inflate(Resource.Layout.SearchObject, viewGroup, false);
            var searchObjectView = new SearchObjectView(item);
            return searchObjectView;
        }

        public static SearchObjectSearchSugestion SearchObjectSugestionInflateView(BaseSearchObject SearchObject, Context Context)
        {
            var item = new SearchObjectSearchSugestion(Context, SearchObject.Name);
            var image = new ImageView(Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            };
            int id = 0;
            switch (SearchObject.SearchObjectType)
            {
                case SearchObjectTypes.Album:
                    id = Resource.Drawable.ic_album_black_24dp;
                    break;
                case SearchObjectTypes.Artist:
                    id = Resource.Drawable.ic_person_black_48dp;
                    break;
                case SearchObjectTypes.Song:
                    id = Resource.Drawable.ic_audiotrack_black_24dp;
                    break;
                default:
                    id = Resource.Drawable.ic_audiotrack_black_24dp;
                    break;
            }
            id = Resource.Drawable.PlaceHolder;
            image.SetImageResource(id);
            item.SetLeftIcon(image);
            return item;
        }
    }
    
}