using System.Collections;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace OnePieceBot
{
    public sealed class Bot // OnePieceBot
    {
        // API 
        // getter
        public static Bot Instance { get { return m_instance; } }

        // methods
        public void NotifyAll(string message)
        {
            foreach(Int64 u in m_data.UserIds)
            {
                m_bot.SendTextMessageAsync(u, message);
            }
        }

        // member 
        private static readonly Bot m_instance = new Bot();
        Data m_data;
        private TelegramBotClient m_bot;
        private CancellationTokenSource m_cts;

        // INTERNAL 
        public void Run()
        {
            m_bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
                cancellationToken: m_cts.Token
            );
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            String text = message.Text;
            String cmd = "";
            String tail = "";
            if(text.Contains(" "))
            {
                cmd = text.Substring(0, text.IndexOf(" "));
                tail = text.Substring(text.IndexOf(" ") + 1);
            }
            else
            {
                cmd = text;
            }

            switch (cmd)
            {
                case "/start":
                    m_data.AddUser(message.Chat.Id);
                    break;
                case "/end":
                    m_data.RemoveUser(message.Chat.Id);
                    break;
                case "/announce":
                    NotifyAll("Die neueste Verlautung lautet:\n" + tail);
                    break;
                default:
                    break;
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        // initialization
        private Bot()
        {
            m_bot = new TelegramBotClient("5642757471:AAH3Mi7W3B4DBcQPCTljCOdarZZiWtr7_K4");
            m_cts = new CancellationTokenSource();

            m_data = Data.Instance;
        }
        ~Bot() { m_cts.Cancel(); }
    }
}
