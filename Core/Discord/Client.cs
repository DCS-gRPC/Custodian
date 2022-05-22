/* 
Custodian is a DCS server administration tool for Discord
Copyright (C) 2022 Jeffrey Jones

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
        private readonly InteractionHandler _interactionHandler;

        public Client(ILogger<Client> logger,
            IOptions<Configuration.Application> applicationConfiguration, DiscordSocketClient discordSocketClient, InteractionHandler interactionHandler)
        {
            _logger = logger;
            _guildId = applicationConfiguration.Value.Discord.GuildId;
            _token = applicationConfiguration.Value.Discord.Token;

            _interactionHandler = interactionHandler;

            _client = discordSocketClient;
            _client.Ready += RegisterInteractionModules;
            _client.Log += LogAsync;
        }

        private async Task RegisterInteractionModules()
        {
            try { 
                await _interactionHandler.InitializeAsync(_guildId);
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message, ex);
            }
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
