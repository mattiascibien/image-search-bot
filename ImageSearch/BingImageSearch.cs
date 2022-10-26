using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ImageSearchBot.Config;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;

namespace ImageSearchBot.ImageSearch
{
    [Obsolete]
    public class BingImageSearch : ImageSearch
    {
        private readonly ImageSearchClient _client;

        public override int MaxImages => 150;
        
        public BingImageSearch(string botPrefix, ImageSearchConfig config) : base(botPrefix, config)
        {
            var subscriptionKey = Environment.GetEnvironmentVariable("BING_KEY") ?? Environment.GetEnvironmentVariable($"{BotPrefix}_BING_KEY");
            _client = new ImageSearchClient(new ApiKeyServiceClientCredentials(subscriptionKey));
        }

        public override void Dispose()
        {
            _client.Dispose();
        }
        
        public override async Task<byte[]> GetImageAsync(int index)
        {
            var imageResults = await _client.Images.SearchAsync(
                Config.Query, 
                safeSearch: Config.IncludeNsfw ?? false ? "Off" : "Moderate",
                count: MaxImages); //search query

            var clampedIndex = Math.Clamp(index, 0, imageResults.Value.Count);
            
            var imageUrl = imageResults.Value[clampedIndex].ContentUrl;
            using var webClient = new HttpClient();

            return await webClient.GetByteArrayAsync(imageUrl);
        }
    }
}