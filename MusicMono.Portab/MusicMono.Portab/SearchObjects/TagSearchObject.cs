using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicMono.Portab.SearchObjects
{
    public class TagSearchObject : BaseSearchObject
    {
        public List<SongSearchObject> Songs { get; set; }
        public List<AlbumSearchObject> Albums { get; set; }
        public List<ArtistSearchObject> Artist { get; set; }
        public override string Type
        {
            get
            {
                return "Tag";
            }
        }
        public TagSearchObject()
        {

        }
        public TagSearchObject(LastTag lastTag)
        {

        }
        public void AddInfo(LastTag lastTag)
        {
            base.Name = lastTag.Name;
        }
    }
}