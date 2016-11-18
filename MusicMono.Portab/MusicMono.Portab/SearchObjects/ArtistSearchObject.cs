using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMono.Portab.SearchObjects
{
    public class ArtistSearchObject : BaseSearchObject
    {
        public int NumberOfAlbums { get; set; }
        public int NumberOfSongs { get; set; }
        public List<SongSearchObject> Songs { get; set; }
        public List<AlbumSearchObject> Albums { get; set; }
        public override string Type
        {
            get
            {
                return "Artist";
            }
        }
        public override SearchObjectTypes SearchObjectType
        {
            get
            {
                return SearchObjectTypes.Artist;
            }
        }
        public override string Subtitle
        {
            get
            {
                return string.Empty;
            }
        }
        public ArtistSearchObject()
        {

        }
        public ArtistSearchObject(LastArtist lastArtist)
        {
            AddInfo(lastArtist);
        }
        public void AddInfo(LastArtist lastArtist)
        {
            base.ID = lastArtist.Mbid;
            base.Name = lastArtist.Name;
            base.Image = lastArtist.MainImage != null && lastArtist.MainImage.Any() ? lastArtist.MainImage.First().AbsoluteUri : string.Empty;
            base.Tags = (from tag in lastArtist.Tags
                        select new TagSearchObject(tag)).ToList();
        }
    }
}
