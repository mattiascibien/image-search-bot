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
                .Where(file => Path.GetFileName(file).EndsWith(".botconfig.json", StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (!configs.Any())
            {
                Console.Error.WriteLine("No bots configured. Add one like this mybot.botconfig.json");
                Environment.Exit(1);
                return;
            }

            foreach (var config in configs)
            {
                try
                {
                    CreateAndRunBot(config);
                }
                catch(Exception ex)
                {
                    // Let's prevent any wrong configuration to crash the app
                    Console.WriteLine($"Bot run failed: {ex.Message}");
                }
            }
            
            ExitEvent.WaitOne();
            Console.CancelKeyPress -= ConsoleOnCancelKeyPress;
            Bots.ForEach(b => b.Stop());
        }

        private static void CreateAndRunBot(string config)
        {
            var contents = File.ReadAllText(config);
            var cfg = JsonConvert.DeserializeObject<RootConfig>(contents, 
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver() 
                });

            Assert.Check(cfg != null, "Root configuration must be set");
            Assert.Check(cfg.ImageSearchConfig != null, "Bot image search configuration must be set");
            Assert.Check(cfg.BotConfig  != null, "Bot config must be set");

            var imageSearchType = Type.GetType(cfg.ImageSearchConfig.Type);
            var imageSearch = (IImageSearch)Activator.CreateInstance(imageSearchType, cfg.BotConfig.Prefix, cfg.ImageSearchConfig);
            var imgBot = new ImgBot(cfg.BotConfig, imageSearch);
            Bots.Add(imgBot);

            Task.Run(() => imgBot.Run());
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ExitEvent.Set();
        }
    }
}