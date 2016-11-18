using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMono.Portab.SearchObjects
{
    public class AlbumSearchObject : BaseSearchObject
    {
        public int NumberOfSongs { get; set; }
        public TimeSpan TotalLenght { get; set; }
        public ArtistSearchObject MadeBy { get; set; }
        public List<SongSearchObject> Songs { get; set; }
        public override string Type
        {
            get
            {
                return "Album";
            }
        }
        public override string Subtitle
        {
            get
            {
                return MadeBy?.Name ?? string.Empty;
            }
        }
        public override SearchObjectTypes SearchObjectType
        {
            get
            {
                return SearchObjectTypes.Album;
            }
        }
        public AlbumSearchObject()
        {

        }
        public AlbumSearchObject(LastAlbum lastAlbum)
        {
            AddInfo(lastAlbum);

        }
        public void AddInfo(LastAlbum lastAlbum)
        {
            base.ID = lastAlbum.Mbid;
            this.MadeBy = new ArtistSearchObject() { Name = lastAlbum.ArtistName};
            this.Songs = (from song in lastAlbum.Tracks
                         select new SongSearchObject(song)).ToList();
            this.TotalLenght = new TimeSpan(Songs.Sum(i => i.Duration.Ticks));
            base.Duration = this.TotalLenght;
            base.Image = (lastAlbum.Images != null && lastAlbum.Images.Any()) ? lastAlbum.Images.First().AbsoluteUri : string.Empty; 
            base.Name = lastAlbum.Name;
        }
    }
}
