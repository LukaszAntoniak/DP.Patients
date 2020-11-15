using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace DP.Notifications.Services
{
    public class ServiceBusConsumer
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger _logger;

        public ServiceBusConsumer(IConfiguration configuration, ILogger logger)
        {
            _queueClient = new QueueClient(configuration.GetConnectionString("ServiceBusConnectionString"), "messages");
            _logger = logger;
        }

        public void Register()
        {
            var options = new MessageHandlerOptions(ExceptionHandler)
            {
                AutoComplete = false
            };

            _queueClient.RegisterMessageHandler(ProcessMessage, options);
        }

        private async Task ProcessMessage(Message message, CancellationToken token)
        {
            try
            {
                _logger.Information("Processing message " + System.Text.Json.JsonSerializer.Serialize(message));

                var payload = JsonConvert.DeserializeObject<MessagePayload>(Encoding.UTF8.GetString(message.Body));

                if (payload.EventName == "NewUserRegistered")
                {
                    EmailSenderService sender = new EmailSenderService();

                    sender.SendMessage(payload.UserEmail, "Covid19", "Proszę się udać na kwarantannę.");
                }

                await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }catch(Exception e)
            {
                _logger.Error(e, "Something went wrong during message processing");
                throw;
            }
        }

        private Task ExceptionHandler(ExceptionReceivedEventArgs e)
        {

            return Task.CompletedTask;
        }
    }
    public class MessagePayload
    {
        public string EventName { get; set; }

        public string UserEmail { get; set; }
    }
}
