//using Google.Apis.YouTube.v3.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MusicMono.Portab.Search
//{
//    public class Song : SearchResult
//    {
//        public Song()
//        {
//            DateAdded = DateTime.Now;
//        }
//        public Song(SearchResult SearchResoult)
//        {
//            base.ETag = SearchResoult.ETag;
//            base.Id = SearchResoult.Id;
//            base.Kind = SearchResoult.Kind;
//            base.Snippet = SearchResoult.Snippet;
//            DateAdded = DateTime.Now;
//        }
//        public string SongName
//        {
//            get
//            {
//                return base.Snippet.Title;
//            }
//        }
//        public string ArtistName { get; set; }
//        public string SongNameOnDisk
//        {
//            get
//            {
//                return base.Snippet.Title + ".mp3";
//            }
//        }
//        public string PathToSongOnDisc { get; set; }
//        public TimeSpan Duration { get; set; }
//        public DateTime DateAdded { get; set; }
//        public int DownloadPercent
//        {
//            get
//            {
//                return _downloadPercent;
//            }
//            set
//            {
//                _downloadPercent = value;
//                OnChange?.Invoke(this, EventArgs.Empty);
//            }
//        }
//        public string DownloadURL
//        {
//            get
//            {
//                return "http://www.youtubeinmp3.com/fetch/?video=https://www.youtube.com/watch?v=" + this.Id.VideoId;
//            }
//        }
//        public event EventHandler<EventArgs> OnChange;

//        public SongState State
//        {
//            get
//            {
//                return _state;
//            }
//            set
//            {
//                _state = value;
//                OnChange?.Invoke(this, EventArgs.Empty);
//            }
//        }

//        private SongState _state;
//        private int _downloadPercent;
//        public override string ToString()
//        {
//            switch (State)
//            {
//                case SongState.Nothing:
//                    return $"{SongName}{Environment.NewLine}{Duration.ToString()}";
//                case SongState.Downloading:
//                    return $"{SongName}{Environment.NewLine}{DownloadPercent}%";
//                case SongState.Downloaded:
//                    return $"{SongName}{Environment.NewLine}{Duration.ToString()}";
//                case SongState.DownloadError:
//                    return $"{SongName}{Environment.NewLine}Download error";
//                case SongState.Deleted:
//                    return $"{SongName}{Environment.NewLine}{Duration.ToString()}";
//                default:
//                    return SongName;
//            }
//        }

//    }


//}
