using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Xamarin.Forms;
using System.Net;
using MusicMono.Portab.Search;

namespace MusicMono.Portab.SearchObjects
{
    public class BaseSearchObject : IDisposable
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string DownloadLink { get; protected set; }
        public string PathToSearchObjectOnDisc { get; set; }
        public string Status
        {
            get
            {
                switch (SearchObjectState)
                {
                    case SearchObjectState.Nothing:
                        return string.Empty;
                    case SearchObjectState.Downloading:
                        return $"Downloading: {DownloadPercente}%";
                    case SearchObjectState.Downloaded:
                        return $"Downloaded: {PathToSearchObjectOnDisc}";
                    case SearchObjectState.DownloadError:
                        return $"Download error";
                    case SearchObjectState.Deleted:
                        return $"Song deleted";
                    default:
                        return string.Empty; ;
                }
            }
        }
        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
               
                _image = value;
            }
        }
        public byte[] RealImage { get; set;}
        public bool WasDownaloaded { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime AddedDate { get; private set; }
        public List<TagSearchObject> Tags { get; set; }
        public SearchObjectState SearchObjectState {
            get
            {
                return _searchObjectState;
            }
            set
            {
                OnChange?.Invoke(this, EventArgs.Empty);
                _searchObjectState = value;
            }
        }    
        public bool IsAnybodyLisening
        {
            get
            {
                return !(OnChange == null);
            }
        }   
        public int DownloadPercente
        {
            get
            {
                return _downloadPrecente;
            }
            set
            {
                OnChange?.Invoke(this, EventArgs.Empty);
                
                _downloadPrecente = value;
            }

        }
        public virtual string Type { get; }
        public virtual string Subtitle { get; }
        public virtual SearchObjectTypes SearchObjectType { get; }
        public event EventHandler<EventArgs> OnChange;

        private SearchObjectState _searchObjectState;
        private int _downloadPrecente;
        private bool _shuldShutown = false;
        private string _image;
        public BaseSearchObject()
        {
            AddedDate = DateTime.Now;
        }
        public async Task<string> FealLuckyDownloadLinkAsync()
        {
            var youSearch = new YoutubeSearch();
            var VidID = await youSearch.FealLuckyAsync($"{Name} {Subtitle} {Type}");
            return "http://www.youtubeinmp3.com/fetch/?video=https://www.youtube.com/watch?v=" + VidID;
        }
        public void Dispose()
        {
            _shuldShutown = true;
        }
        public override string ToString()
        {
            int tgc = (Tags == null) ? (0) : (Tags.Count);
            return $"{Type}: {Name}{Environment.NewLine}Tags count: {tgc}";
        }
        private async Task CheckForUpdatesAsync()
        {
            while (!_shuldShutown)
            {
                if (WasDownaloaded && SearchObjectState == SearchObjectState.Downloaded)
                {
                    var file = await FileSystem.Current.GetFileFromPathAsync(PathToSearchObjectOnDisc);
                    if (file == null)
                        SearchObjectState = SearchObjectState.Deleted;
                }
                await Task.Delay(10000);
            }
        }
    }
    public enum SearchObjectState
    {
        Nothing,
        Downloading,
        Downloaded,
        DownloadError,
        Deleted
    }
    public enum SearchObjectTypes
    {
        Song,
        Artist,
        Album,
        Era
    }
    public enum SearchObject
    {
        Song,
        Album,
        Artist,
        Tag,
        Any
    }
}
