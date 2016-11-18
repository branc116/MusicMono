using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using FloatingSearchViews;
using MusicMono.Portab.Search;
using System.Linq;
using FloatingSearchViews.Suggestions.Models;
using MusicMono.Helper;

namespace MusicMono
{
    [Activity(Label = "MusicMono", MainLauncher = true, Icon = "@drawable/MusicNote")]
    public class MainActivity : Activity
    {
        private LastFmSearch _lastFmSearch = new LastFmSearch();
        protected override void OnCreate(Bundle bundle)
        {
#region StartUp
            Intent inten = new Intent(this, typeof(Notif));
            StartService(inten);

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
#endregion
            //var res = FindViewById<RecyclerView>(Resource.Id.Resoults);
            var resoults = new SearchObjectAdapter(this);
            var layoutManager = new LinearLayoutManager(this);
            var SearchBar = FindViewById<FloatingSearchView>(Resource.Id.Searchbar);

            
            
            //res.SetLayoutManager(layoutManager);
            //res.HasFixedSize = false;
            
            
            //res.SetAdapter(resoults);
            SearchBar.QueryChange += async (sender, argz) =>
            {
                //await resoults.Q((sender as FloatingSearchView).Query);
                if (argz.NewQuery != argz.OldQuery) {
                    if (!string.IsNullOrWhiteSpace(argz.NewQuery))
                    {
                        this.RunOnUiThread(() =>
                        {
                            SearchBar.ShowProgress();
                            SearchBar.ClearSuggestions();
                        });
                        await _lastFmSearch.SearchAsync(argz.NewQuery);
                    }
                    else
                    {
                        this.RunOnUiThread(() =>
                        {
                            SearchBar.ClearSuggestions();
                        });
                    }
                }
            };
            _lastFmSearch.OnChange += (sender, argz) =>
            {
                try
                {

                        var SearchObjects = (from Obj in _lastFmSearch.CurrentList
                                         select InflateView.SearchObjectSugestionInflateView(Obj, this)  as ISearchSuggestion).ToList();
                    
                    this.RunOnUiThread(() =>
                    {
                        SearchBar.SwapSuggestions(SearchObjects );
                        SearchBar.HideProgress();
                    });
                }
                catch (Exception ex)
                {
                    if (ex != null)
                        ex = null;
                }
            };
            //res.
            //res.Scroll += (a, b) =>
            //{
            //    if (b.FirstVisibleItem + b.VisibleItemCount +1 >= b.TotalItemCount)
            //    {
            //        Task t = (b.View.Adapter as MyTextAdapter).endReched();
            //    }
            //};

        }
    }
}

