# image-search-bot
Configurable apps for running Telegram Bots that responds to photo Image Queries

## Bots using this framework

 * [@mikesmotherbot](https://telegram.me/mikesmother_bot) - A joke bot that sends images of Cara Buono, Mike's mother in Stranger Things.

## Deveopment
 
 * Register your bot(s) using [@botfather](https://telegram.me/botfather).
 * Add a configuration file inside the project directory (use the one provided as an example)
 * Choose a prefix for your bot, it will be used to find the secure settings needed using environment variables
 * Add the environment variable `<BOT_PREFIX>_TELEGRAM_KEY` and set its value to the configuration token given by @botfather
 * Add any required environment variables for the chosen Image Search Provider; at the moment only Bing is supported so you should set the value of `<BOT_PREFIX>_BING_KEY` to your API key (you can read more [here](https://docs.microsoft.com/en-us/azure/cognitive-services/bing-web-search/))
 * Compile and run the project.
    * If needed you can override the config directory by passing it as a command-line parameter, i.e: `./image-search-bot.exe /opt/myconfigs`
