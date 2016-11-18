using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api;
using MusicMono.Portab.SearchObjects;

namespace MusicMono.Portab.Helper
{
    static class Converters
    {
        public static string ReservedChars = "|\\?*<\":>+[]/'";
        public static string SupportedChars = "1234567890abcčćdđefghijklmnopqwrsštuvwxyzžABCČĆDĐEFGHIJKLMNOPQRSŠTUVWXYZŽ -";
        public static string[] Phrases = new string[] { "HD", "hd", "Hd", "lyrics", "Lyrics", "1080p", "1080", "720p", "720", "Spot", "official", "Official", "video", "Video", "VIDEO", "HQ", "hq", "Hq" };
        public static string FormatStringNicely(string UnformatedString)
        {
            return UnformatedString
                .Split(Phrases, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate((i, j) =>
                {
                    i = i.Split(ReservedChars.ToCharArray()).Aggregate((k, g) => k + g);
                    j = j.Split(ReservedChars.ToCharArray()).Aggregate((k, g) => k + g);
                    if (string.IsNullOrWhiteSpace(i) || string.IsNullOrWhiteSpace(j))
                        return i + j;
                    if (i[i.Length - 1] == ' ' && j[0] == ' ')
                        return i.Remove(i.Length - 1) + j;
                    else if (i[i.Length - 1] == ' ' || j[0] == ' ')
                        return i + j;
                    return $"{i} {j}";
                });
        }
        /// <summary>
        /// Iso8601 is defined as PT#nummM#numsS where is element of <0, 2<<31>, and nums is element of <0, 60> 
        /// </summary>
        /// <param name="Iso8601"></param>
        /// <returns></returns>
        public static TimeSpan Iso8601ToTimeSpan(string Iso8601)
        {
            
            int m, s, h;
            TimeSpan span;
            var MinAndSec = Iso8601.Split("PTHM#S".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            try
            {
                if (MinAndSec.Length == 3)
                {
                    h = Convert.ToInt32(MinAndSec[0]);
                    m = Convert.ToInt32(MinAndSec[1]);
                    s = Convert.ToInt32(MinAndSec[2]);

                    span = new TimeSpan(h + m / 60, m % 60, s);
                }
                else if (MinAndSec.Length == 2)
                {
                    m = Convert.ToInt32(MinAndSec[0]);
                    s = Convert.ToInt32(MinAndSec[1]);

                    span = new TimeSpan(m / 60, m % 60, s);
                }
                else if (MinAndSec.Length == 1)
                {
                    s = Convert.ToInt32(MinAndSec[0]);

                    span = new TimeSpan(0, s / 60, s % 60);
                }
                else
                    span = new TimeSpan(0);
            }
            catch(Exception ex)
            {
                span = new TimeSpan(0);
                if (ex != null)
                    ex = null;
            }
            return span;
        }
        public static SongSearchObject  VideoToSong(Video video)
        {
            var song = new SongSearchObject()
            {
                Duration = Iso8601ToTimeSpan(video.ContentDetails.Duration),
                Name = FormatStringNicely(video.Snippet.Title),
                ID = video.Id
            };
            return song;

        }
        public static string YoutubeVideoIDToDownloadLink(string VideoID)
        {
            return "http://www.youtubeinmp3.com/fetch/?video=https://www.youtube.com/watch?v=" + VideoID;
        }
    }
}
