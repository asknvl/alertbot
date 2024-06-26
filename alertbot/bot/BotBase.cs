﻿using alertbot.logger;
using alertbot.rest;
using alertbot.server;
using alertbot.users;
using Newtonsoft.Json.Linq;
using servicecontrolhub.config;
using servicecontrolhub.monitors.protocol.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace alertbot.bot
{
    public abstract class BotBase : IDiagnosticsPresenter
    {
        #region const      
        string TAG = "BOT";
        #endregion

        #region vars
        protected ITelegramBotClient bot;        
        CancellationTokenSource cts;
        UserManager userManager = new();

        Settings settings;
        ILogger logger;
        IHubApi hubApi;
        System.Timers.Timer keepAliveTimer;

        int keepAliveCounter = 0;
        #endregion

        #region properties
        #endregion

        public BotBase(Settings settings)
        {
            this.settings = settings;
            logger = new Logger("bot");
            hubApi = new HubApi(settings.config.keepalive.url);

            keepAliveTimer = new System.Timers.Timer();
            keepAliveTimer.Interval = settings.config.keepalive.period * 1000;
            keepAliveTimer.Elapsed += KeepAliveTimer_Elapsed;
            keepAliveTimer.AutoReset = true;
            keepAliveTimer.Start();
        }

        private async void KeepAliveTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            bool res = false;
            try
            {
                keepAliveCounter = await hubApi.KeepAliveRequest();
                res = true;                     

            } catch (Exception ex)
            {
            }


            if (!res)
            {
                var users = userManager.Get();
                foreach (var user in users)
                {

                    try
                    {
                        string message = $"*❌ Хаб-сервис недоступен*";
                        await bot.SendTextMessageAsync(user.tg_id, message, parseMode: ParseMode.Markdown);
                    }
                    catch (Exception ex)
                    {
                        logger.err(TAG, $"KeepAliveTimer: {ex.Message}");
                    }
                }
            }

        }

        #region helpers
        #endregion

        #region private
        async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update == null)
                return;

            switch (update.Type)
            {
                case UpdateType.Message:
                    try
                    {
                        var chat = update.Message.Chat.Id;
                        var un = update.Message.Chat.Username;
                        var fn = update.Message.Chat.FirstName;
                        var ln = update.Message.Chat.LastName;

                        if (update.Message.Text != null)
                        {
                            string text = update.Message.Text;

                            switch (text)
                            {
                                case var _ when text == settings.config.bot.user_password:
                                    userManager.Add(chat, un: un, fn: fn, ln: ln);
                                    await bot.SendTextMessageAsync(chat, "Теперь вам будут приходить оповещения об ошибках в сервисах");
                                    break;

                                case var _ when text == settings.config.bot.admin_password:
                                    userManager.Add(chat, un: un, fn: fn, ln: ln, is_admin: true);
                                    await bot.SendTextMessageAsync(chat, "Теперь вам будут приходить оповещения об ошибках в сервисах");
                                    break;

                                case "/check":
                                    try
                                    {
                                        await bot.SendTextMessageAsync(chat, $"keepAliveCounter={keepAliveCounter}");
                                    } catch (Exception ex)
                                    {
                                    }
                                    break;

                                default:
                                    break;  
                            }
                        }

                    } catch (Exception ex)
                    {

                    }
                    break;

                default:
                    break;
            }
        }

        virtual protected Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };            
            return Task.CompletedTask;
        }
        #endregion

        #region public
        public virtual async Task Start()
        {

#if DEBUG
            bot = new TelegramBotClient(settings.token);
            
#else     
            bot = new TelegramBotClient(new TelegramBotClientOptions(settings.config.bot.token, "http://localhost:8081/bot/"));
#endif

            cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message }
            };

            bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);

            try
            {
                var me = await bot.GetMeAsync();
                logger.inf(TAG, $"{me.Username} started");
                
            } catch (Exception ex)
            {
                logger.err(TAG, $"{settings.config.bot.token} {ex.Message}");
            }

        }

        public void Stop()
        {
        }

        public async Task PresentDiagnosicsData(serviceDiagnosticsDto data)
        {
            try
            {

                var users = userManager.Get();

                foreach (var user in users)
                {
                    try
                    {

                        string message = $"*❌{data.service_name}*\n";

                        foreach (var item in data.errors)
                        {
                            message += $"*{item.entity}*: {item.description}";
                        }

                        await bot.SendTextMessageAsync(user.tg_id, message, parseMode: ParseMode.Markdown );

                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
