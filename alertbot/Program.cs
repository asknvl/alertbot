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
            Settings settings = Settings.getInstance();

            RestService restService = new RestService(settings.config.receiver.port);   
            PresentDiagnosticsRequestProcessor presentDiagnosticsRequestProcessor = new PresentDiagnosticsRequestProcessor();
            restService.RequestProcessors.Add(presentDiagnosticsRequestProcessor);

            
            BotBase bot = new alertbot_v0(settings);
            presentDiagnosticsRequestProcessor.Add(bot);

            restService.Listen();

            bot.Start();

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}