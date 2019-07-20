using System;
using System.Net;
using ImageSearchBot.Config;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;

namespace ImageSearchBot.ImageSearch
{
    public class BingImageSearch : ImageSearch
    {
        private readonly ImageSearchClient _client;

        public BingImageSearch(string botPrefix, ImageSearchConfig config) : base(botPrefix, config)
        {
            var subscriptionKey = Environment.GetEnvironmentVariable("BING_KEY") ?? Environment.GetEnvironmentVariable($"{BotPrefix}_BING_KEY");
            _client = new ImageSearchClient(new ApiKeyServiceClientCredentials(subscriptionKey));
        }

        public override void Dispose()
        {
            _client.Dispose();
        }

        public override byte[] GetImage(int index, int count)
        {
            var imageResults = _client.Images.SearchAsync(
                Config.Query, 
                safeSearch: Config.IncludeNsfw ?? false ? "Off" : "Moderate",
                count: count).Result; //search query

            var clampedIndex = Math.Clamp(index, 0, imageResults.Value.Count-1);
            
            var imageUrl = imageResults.Value[clampedIndex].ContentUrl;
            using (var webClient = new WebClient())
            {
                return webClient.DownloadData(imageUrl);
            }   
        }
    }
}