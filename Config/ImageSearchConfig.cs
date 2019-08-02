using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ImageSearchBot.Config
{
    public class ImageSearchConfig
    {
        public string Type { get; set; }
        
        public string Query { get; set; }
        
        public bool? IncludeNsfw { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ImageType? ImageType { get;set; }
        
    }
}