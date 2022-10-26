using GScraper;
using GScraper.DuckDuckGo;
using GScraper.Google;
using ImageSearchBot.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ImageSearchBot.ImageSearch
{
    public class GScraperImageSearch : ImageSearch
    {
        private GoogleScraper _scraper;

        public GScraperImageSearch(string botPrefix, ImageSearchConfig config) : base(botPrefix, config)
        {
            _scraper = new GoogleScraper();
        }

        public override int MaxImages => 150;

        public override void Dispose()
        {
            _scraper.Dispose();
        }

        public override async Task<byte[]> GetImageAsync(int index)
        {
            var imageResults = (await _scraper.GetImagesAsync(Config.Query, Config.IncludeNsfw ?? false ? SafeSearchLevel.Off : SafeSearchLevel.Moderate)).Take(MaxImages).ToList();

            var clampedIndex = Math.Clamp(index, 0, imageResults.Count-1);
            
            var imageUrl = imageResults[clampedIndex].Url;
            using var webClient = new HttpClient();

            return await webClient.GetByteArrayAsync(imageUrl);
        }
    }
}
