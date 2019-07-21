using System.Collections;
using System.Collections.Generic;

namespace ImageSearchBot.Config
{
    public class BotConfig
    {
        public string Prefix { get; set; }
        
        public List<string> GreetingsMessages { get; set; }
        
        public List<string> Responses { get; set; }
        
        public List<string> Keywords { get; set; }
    }
}