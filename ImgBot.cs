using System;
using System.IO;
using ImageSearchBot.Config;
using ImageSearchBot.ImageSearch;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ImageSearchBot
{
    public class ImgBot
    {
        private const int MaxImages = 150;
        
        private readonly TelegramBotClient _bot;
        private readonly User _me;
        private readonly BotConfig _config;
        private readonly IImageSearch _imageSearch;

        public ImgBot(BotConfig config, IImageSearch imageSearch)
        {
            _config = config;
            _imageSearch = imageSearch;
            _bot = new TelegramBotClient(Environment.GetEnvironmentVariable($"{_config.Prefix}_TELEGRAM_KEY"));
            
            _me = _bot.GetMeAsync().Result;
            
            _bot.OnMessage += BotOnMessageReceived;
            _bot.OnMessageEdited += BotOnMessageReceived;
            _bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            _bot.OnInlineQuery += BotOnInlineQueryReceived;
            _bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            _bot.OnReceiveError += BotOnReceiveError;
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            
        }

        private void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs e)
        {
            
        }

        private void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs e)
        {
            
        }

        private void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            var isPrivate = e.Message.Chat.Type == ChatType.Private;

            if (isPrivate || message.Text.Contains($"@{_me.Username}"))
            {
                var hasKeyword = false;

                // check if we have at least one keyword
                foreach (var keyword in _config.Keywords)
                {
                    hasKeyword = message.Text.ToLower().Contains(keyword.ToLower());
                    if (hasKeyword)
                        break;
                }

                var random = new Random((int)DateTime.Now.Ticks);
                if (hasKeyword)
                {
                    var image = _imageSearch.GetImage(random.Next(MaxImages), MaxImages);

                    using (var memoryStream = new MemoryStream(image))
                    {
                        await _bot.SendPhotoAsync(
                            message.Chat.Id,
                            memoryStream,
                            _config.Responses[random.Next(_config.Responses.Count)]);
                    }
                }
                else
                {
                    await _bot.SendTextMessageAsync(message.Chat.Id,
                        _config.GreetingsMessages[random.Next(_config.GreetingsMessages.Count)]);
                }
            }
        }

        public void Run()
        {
            _bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"@{_me.Username}: started");
        }

        public void Stop()
        {
            Console.WriteLine($"@{_me.Username}: stopped");
            _bot.StopReceiving();
        }
    }
}