using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePieceBot
{
    public class MOFetcher // One Piece Manga Online
    {

        static readonly HttpClient client = new HttpClient();

        public static string Fetch(string url)
        {
            // TODO error handling
            string text = client.GetStringAsync(url).Result;
            return text;
        }

        public void Run()
        {
            var chapter = Data.Instance.Chapter;
            var url = $"https://w5.onepiece-manga-online.net/manga/one-piece-chapter-{chapter}/";
            var found_chapter = Fetch(url).Contains("This Chapter is not available Yet.");

            // TODO: error handling in case bot loses connection
            if(found_chapter && Data.Instance.NextChapter(chapter))
            {
                Bot.Instance.NotifyAll(url);
            }
        }
    }
}
