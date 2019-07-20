using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using ImageSearchBot.Config;
using ImageSearchBot.ImageSearch;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ImageSearchBot
{
    public static class Program
    {
        private static readonly List<ImgBot> Bots = new List<ImgBot>();
        
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
        
        public static void Main(string[] args)
        {
            DotEnv.Config(false);
            
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            var dir = args.Length <= 0 ? Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) : args[0];

            // retrieve bots config inside the directory
            var configs = Directory.GetFiles(dir)
                .Where(file => Path.GetFileName(file).ToLower().EndsWith(".botconfig.json")).ToList();
            if (configs.Count == 0)
            {
                Console.Error.WriteLine("No bots configured. Add one like this mybot.botconfig.json");
                Environment.Exit(1);
                return;
            }

            foreach (var config in configs)
            {
                var contents = File.ReadAllText(config);
                var cfg = JsonConvert.DeserializeObject<RootConfig>(contents, 
                    new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver() 
                    });
                var imageSearchType = Type.GetType(cfg.ImageSearchConfig.Type);
                var imageSearch = (IImageSearch)Activator.CreateInstance(imageSearchType, cfg.BotConfig.Prefix, cfg.ImageSearchConfig);
                var imgBot = new ImgBot(cfg.BotConfig, imageSearch);
                Bots.Add(imgBot);
                Task.Run(() => imgBot.Run());
            }
            
            ExitEvent.WaitOne();
            Console.CancelKeyPress -= ConsoleOnCancelKeyPress;
            Bots.ForEach(b => b.Stop());
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ExitEvent.Set();
        }
    }
}