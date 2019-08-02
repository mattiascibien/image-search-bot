using System;
using System.Net;
using ImageSearchBot.Config;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;

namespace ImageSearchBot.ImageSearch
{
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
        
        public override byte[] GetImage(int index)
        {
            string imageType = null;

            if(Config.ImageType != null && Config.ImageType != ImageType.All)
            {
                if(Config.ImageType == ImageType.Animated)
                    imageType = "AnimatedGif";

                if(Config.ImageType == ImageType.Animated)
                    imageType = "Photo";
            }

            var imageResults = _client.Images.SearchAsync(
                Config.Query, 
                safeSearch: Config.IncludeNsfw ?? false ? "Off" : "Moderate",
                imageType: imageType, 
                count: MaxImages).Result; //search query

            var clampedIndex = Math.Clamp(index, 0, imageResults.Value.Count);
            
            var imageUrl = imageResults.Value[clampedIndex].ContentUrl;
            using (var webClient = new WebClient())
            {
                return webClient.DownloadData(imageUrl);
            }   
        }
    }
}