using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MusicMono.Portab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using System.Net;
using System.Net.Http;
using MusicMono.Portab.SearchObjects;
using static MusicMono.Portab.Helper.Converters;
namespace MusicMono.Portab.Search
{
    public class YoutubeSearch : BaseSearch
    {
        private YouTubeService youService;
        private SearchResource.ListRequest searchResoults;


        private string _nextPageToken;
        private int _pageNum;
        
        private const int DEFAULT_NUM_OF_RESOULTS = 10;

        private Task<SearchListResponse> SearchListResponseTask;
        
        public YoutubeSearch()
        {
            _pageNum = 0;
            _nextPageToken = "";
            OnChange += YoutubeSearch_OnChange;
            youService = new YouTubeService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCEB5eOXNpdbX4qXBrd2YIUTHTZpb0uzyk",
                ApplicationName = "numeric-dialect-136518"
            });
            searchResoults = youService.Search.List("snippet");
            searchResoults.VideoCategoryId = "10";
            //searchReoults.VideoType = SearchResource.ListRequest.VideoTypeEnum.Any;
            searchResoults.Type = "video";
            searchResoults.MaxResults = 40;

        }

        private void YoutubeSearch_OnChange(object sender, EventArgs e)
        {
            _changed = true;
        }

        
        protected override async Task QAsync()
        {
            if (string.IsNullOrWhiteSpace(_nextPageToken))
            {
                _pageNum++;
                await QAsync(string.Empty, (int)(DEFAULT_NUM_OF_RESOULTS * Math.Pow(2, _pageNum)), _nextPageToken);
            }else
            {
                throw new Exception("Can't do it, please include search paramater");
            }
        }
        protected override async Task QAsync(string Name)
        {
            _pageNum = 0;
            await QAsync(Name, DEFAULT_NUM_OF_RESOULTS, string.Empty);
        }
        protected override async Task QAsync(string Name, SearchObject SearchObject)
        {
            _pageNum = 0;
            await QAsync(Name, DEFAULT_NUM_OF_RESOULTS, string.Empty);
        }
        private async Task QAsync(string Name, int Count, string nextpageToken )
        {
            if (string.IsNullOrWhiteSpace(Name)||string.IsNullOrWhiteSpace(nextpageToken ))
            {
                string q = Name.Replace(" ", string.Empty);

                Name = q;
                //Check if the user wants the next page or different request
                if (string.IsNullOrWhiteSpace(nextpageToken))
                    searchResoults.Q = Name;
                else
                    searchResoults.PageToken = nextpageToken;

                SearchListResponseTask = searchResoults.ExecuteAsync();

                var SearchList = await SearchListResponseTask;
                //check if the request succesfuly completed
                if (SearchListResponseTask.Status == TaskStatus.RanToCompletion)
                {
                    string ids = HandleResponseFromYoutue(SearchList);
                    Song_OnChange(this, EventArgs.Empty);
                    await GetInfoOnVideosAsync(ids);
                    Song_OnChange(this, EventArgs.Empty);
                }
                    
            }
        }
        
        public async Task<string> FealLuckyAsync(string name)
        {
            searchResoults.Q = name;
            var SearchList = await searchResoults.ExecuteAsync();
            return SearchList.Items.FirstOrDefault()?.Id?.VideoId ?? null;
        }
        /// <summary>
        /// Handle Respones from yotube and return list of ids sperated by ',' 
        /// </summary>
        /// <param name="SearchList"></param>
        /// <returns>string of ids seperated by ','</returns>
        private string HandleResponseFromYoutue(SearchListResponse SearchList)
        {
            _nextPageToken = SearchList.NextPageToken;

            string ids = string.Empty;
            foreach (var sr in SearchList.Items)
            {
                BaseSearchObject song = null;
                if (!_history.Any(i => sr.Id.VideoId == i.Key))
                {
                    ids += sr.Id.VideoId + ",";
                    sr.Snippet.Title = FormatStringNicely(sr.Snippet.Title);
                    song = new SongSearchObject(sr);
                    song.OnChange += Song_OnChange;
                    _history.Add(sr.Id.VideoId, song);
                }
                AddActiveSearchObject(song);
            }
            return ids;
            
        }
        private async Task GetInfoOnVideosAsync(string IDs)
        {
            var rq = youService.Videos.List("contentDetails");
            rq.Id = IDs;
            try
            {
                var reqe = await rq.ExecuteAsync(ct);
                int ind = 0;
                foreach (var sr in IDs.Split(','))
                {
                    if (_history.ContainsKey(sr) && reqe.Items.Count > ind)
                        (_history[sr] as SongSearchObject).AddInfo(reqe.Items[ind]);
                    ind++;
                }

                Song_OnChange(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (ex != null)
                    ex = null;
            }

        }
    }
}
