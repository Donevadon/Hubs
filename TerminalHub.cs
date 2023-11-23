using System;
using System.Text;
using System.Threading.Tasks;
using Common.Messaging;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using RigAgent.Contracts;

namespace User.Service.Hubs
{
    [Authorize]
    public class TerminalHub : HubBase<ITerminalHubClient>
    {
        private readonly ILogger<TerminalHub> _logger;
        private readonly IPublishService _publishService;
        private readonly String _rigCommandTopic;

        public TerminalHub(
            IPublishService publishService,
            ILogger<TerminalHub> logger,
            KafkaConfig kafkaConfig
        )
        {
            if (publishService == null)
            {
                throw new ArgumentNullException(nameof(publishService));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (kafkaConfig == null)
            {
                throw new ArgumentNullException(nameof(kafkaConfig));
            }

            _publishService = publishService;
            _logger = logger;

            _rigCommandTopic = kafkaConfig.TopicPrefix + nameof(RigCommand);
        }

        public async Task SendTerminalCommand(Guid rigID, String command)
        {
            try
            {
                var terminalCommand = new TerminalCommand
                {
                    Id = Encoding.UTF8.GetBytes(Context.ConnectionId),
                    RigID = rigID,
                    Command = Encoding.UTF8.GetBytes(command)
                };

                var rigCommand = new RigCommand
                {
                    Data = MessagePackSerializer.Serialize(terminalCommand),
                    RigID = rigID,
                    Type = RigCommandType.TerminalCommand
                };

                var data = MessagePackSerializer.Serialize(rigCommand);

                await _publishService.Publish(data, _rigCommandTopic);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(SendTerminalCommand)} failed");

                throw;
            }
        }

        public async Task StartTerminal(Guid rigID)
        {
            try
            {
                await SendTerminalCommand(rigID, String.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(StartTerminal)} failed");

                throw;
            }
        }
    }
}