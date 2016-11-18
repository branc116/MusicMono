using Google.Apis.YouTube.v3.Data;
using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicMono.Portab.Helper.Converters;

namespace MusicMono.Portab.SearchObjects
{
    public class SongSearchObject : BaseSearchObject
    {
        public string Lyrics { get; set; }
        public TimeSpan Lenght { get; set; }
        public ArtistSearchObject MadeBy { get; set; }
        public AlbumSearchObject MadeIn { get; set; }
        public override string Type
        {
            get
            {
                return "Song";
            }
        }
        public override string Subtitle
        {
            get
            {
                return  MadeBy?.Name ?? string.Empty;
            }
        }
        public override SearchObjectTypes SearchObjectType
        {
            get
            {
                return SearchObjectTypes.Song;
            }
        }
        public SongSearchObject()
        {

        }
        public SongSearchObject(SearchResult YoutubeSR)
        {
            base.DownloadLink = YoutubeVideoIDToDownloadLink(YoutubeSR.Id.VideoId);
            base.ID = YoutubeSR.Id.VideoId;
            base.Name = FormatStringNicely(YoutubeSR.Snippet.Title);
            base.Image = YoutubeSR.Snippet.Thumbnails.Standard.Url;
        }
        public SongSearchObject(LastTrack track)
        {
            UpdateInfo(track);
        }
        public void UpdateInfo(LastTrack track)
        {
            try
            {
                //Petra je najbolja <3
                base.ID = track.Mbid ?? Guid.NewGuid().ToString("N");
                
                base.Tags = track.TopTags != null ? (from trackTag in track.TopTags
                                                     select new TagSearchObject() { Name = trackTag.Name }).ToList()
                                                  : null;
                base.Image = track.Images != null && track.Images.Any() 
                                             ? track.Images.Largest.AbsoluteUri != null 
                                                ? track.Images.Largest.AbsoluteUri 
                                                : track.Images.First().AbsoluteUri
                                             : string.Empty ; 

                base.Name = track.Name;
                this.MadeBy = new ArtistSearchObject() { Name = track.ArtistName };
                this.MadeIn = new AlbumSearchObject() { Name = track.AlbumName };
                this.Lenght = track.Duration ?? new TimeSpan(0);
            }catch(Exception ex)
            {
                if (ex != null)
                    ex = null;
            }
        }
        public void AddInfo(Video video)
        {
            base.Duration = Iso8601ToTimeSpan(video.ContentDetails.Duration);
        }
               
    }
}
