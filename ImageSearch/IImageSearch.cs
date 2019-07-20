using System;
using ImageSearchBot.Config;

namespace ImageSearchBot.ImageSearch
{
    public interface IImageSearch : IDisposable
    {
        string BotPrefix { get; }
        
        byte[] GetImage(int index, int count);
    }

    public abstract class ImageSearch : IImageSearch
    {
        public string BotPrefix { get; }

        protected ImageSearchConfig Config  { get; }

        protected ImageSearch(string botPrefix, ImageSearchConfig config)
        {
            BotPrefix = botPrefix;
            Config = config;
        }
        
        public abstract void Dispose();

        public abstract byte[] GetImage(int index, int count);
    }
}