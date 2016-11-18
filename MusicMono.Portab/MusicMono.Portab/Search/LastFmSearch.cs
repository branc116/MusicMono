using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using MusicMono.Portab.SearchObjects;
using System.Net.Http;

namespace MusicMono.Portab.Search
{
    public class LastFmSearch : BaseSearch
    {
        private const string APPLICATION_NAME = "MonoMusic";
        private const string API_KEY = "eaa4ce341f3c820504b126be7aeeb1c5";
        private const string SECRET = "89a8596fc58e33344c1fb826152ed9e0";
        private const string REGISTED_TO = "branc116";

        private LastAuth _lastAuth = new LastAuth(API_KEY, SECRET);
        private TrackApi _trackApi;
        private ArtistApi _artistApi;
        private AlbumApi _albumApi;

        public LastFmSearch()
        {
            _trackApi = new TrackApi(_lastAuth);
            _artistApi = new ArtistApi(_lastAuth);
            _albumApi = new AlbumApi(_lastAuth);
        }
        protected override Task QAsync()
        {
            throw new NotImplementedException();
        }

        protected async override Task QAsync(string Name)
        {
            try
            {
                List<SongSearchObject> Songz = await FindSongsAsync(Name);
                List<AlbumSearchObject> Albumz = await FindAlbumsAsync(Name);
                List<ArtistSearchObject> Artistz = await FindArtistsAsync(Name);
                foreach (var so in Songz)
                    if (!_history.ContainsKey(so.ID))
                        _history.Add(so.ID, so);
                foreach (var so in Albumz)
                    if (!_history.ContainsKey(so.ID))
                        _history.Add(so.ID, so);
                foreach (var so in Artistz)
                    if (!_history.ContainsKey(so.ID))
                        _history.Add(so.ID, so);

                AddActiveSearchObjects(Songz);
                AddActiveSearchObjects(Albumz);
                AddActiveSearchObjects(Artistz);
                Song_OnChange(this, EventArgs.Empty);
            }catch(Exception ex)
            {
                if (ex != null)
                    ex = null;
            }
        }

        protected async override Task QAsync(string Name, SearchObject SearchObject)
        {
            switch (SearchObject)
            {
                case SearchObject.Album:
                    List<AlbumSearchObject> Albumz = await FindAlbumsAsync(Name);
                    foreach (var so in Albumz)
                        if (!_history.ContainsKey(so.ID))
                            _history.Add(so.ID, so);
                    AddActiveSearchObjects(Albumz);
                    break;
                case SearchObject.Song:
                    List<SongSearchObject> Songz = await FindSongsAsync(Name);
                    foreach (var so in Songz)
                        if (!_history.ContainsKey(so.ID))
                            _history.Add(so.ID, so);
                    AddActiveSearchObjects(Songz);
                    break;
                case SearchObject.Artist:
                    List<ArtistSearchObject> Artistz = await FindArtistsAsync(Name);
                    foreach (var so in Artistz)
                        if (!_history.ContainsKey(so.ID))
                            _history.Add(so.ID, so);
                    AddActiveSearchObjects(Artistz);
                    break;
                case SearchObject.Any:
                    await QAsync(Name);
                    break;
            }
        
        }

        private async Task<List<SongSearchObject>> FindSongsAsync(string Name)
        {
            var Tracks = await _trackApi.SearchAsync(Name);
            return (from track in Tracks
                    let _song = new SongSearchObject(track)
                    select _song).ToList();
        }
        private async Task<List<AlbumSearchObject>> FindAlbumsAsync(string Name)
        {
            var Albums = await _albumApi.SearchAsync(Name);
            List<AlbumSearchObject> ret = (from album in Albums
                                           select new AlbumSearchObject(album)).ToList();
            return ret;

        }
        private async Task<List<ArtistSearchObject>> FindArtistsAsync(string Name)
        {
            var Artists = await _artistApi.SearchAsync(Name);
            return (from artist in Artists
                    select new ArtistSearchObject(artist)).ToList();
        }
    }
}
