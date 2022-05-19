using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RurouniJones.Custodian.Core.Discord
{
    public sealed class Client
    {
        private static readonly ActivitySource Source = new(nameof(Client));

        private readonly ILogger<Client> _logger;
        private readonly ulong _guildId;
        private readonly string _token;

        private readonly DiscordSocketClient _client;

        public Client(ILogger<Client> logger, IOptions<Configuration.Application> applicationConfiguration)
        {
            _logger = logger;
            _guildId = applicationConfiguration.Value.Discord.GuildId;
            _token = applicationConfiguration.Value.Discord.Token;

            var socketConfig = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged
            };

            _client = new DiscordSocketClient(socketConfig);
            _client.Log += LogAsync;

        }
        private async Task LogAsync(LogMessage message)
        {
            var logLevel = message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                _ => LogLevel.Information
            };

            _logger.Log(logLevel, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
            await Task.CompletedTask;
        }


        public async Task StartAsync(CancellationToken stoppingToken)
        {
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
