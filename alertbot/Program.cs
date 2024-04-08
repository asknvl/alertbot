using alertbot.bot;
using aviatorbot.rest;
using servicecontrolhub.config;
using servicecontrolhub.rest;

namespace alertbot
{
    public class Program
    {
        static void Main(string[] args)
        {
            RestService restService = new RestService();   
            PresentDiagnosticsRequestProcessor presentDiagnosticsRequestProcessor = new PresentDiagnosticsRequestProcessor();
            restService.RequestProcessors.Add(presentDiagnosticsRequestProcessor);

            Settings settings = Settings.getInstance();
            BotBase bot = new alertbot_v0(settings.config.bot);

            restService.Listen();

            bot.Start();

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}