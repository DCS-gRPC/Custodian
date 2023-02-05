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
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using RurouniJones.Custodian.Core.Discord.Interactions;

namespace RurouniJones.Custodian.Core.Discord
{
    public sealed class InteractionHandler
    {
        private static readonly ActivitySource Source = new(nameof(InteractionHandler));

        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InteractionHandler> _logger;

        public InteractionHandler(IServiceProvider services, ILogger<InteractionHandler> logger, DiscordSocketClient client, InteractionService interactionService)
        {
            _client = client;
            _interactionService = new InteractionService(client); ;
            _serviceProvider = services;
            _logger = logger;
        }

        public async Task InitializeAsync(ulong guildId)
        {
            await _interactionService.AddModuleAsync<ChatInteraction>(_serviceProvider);
            await _interactionService.AddModuleAsync<OutTextInteraction>(_serviceProvider);
            await _interactionService.AddModuleAsync<PlayerAdministrationInteraction>(_serviceProvider);
            await _interactionService.RegisterCommandsToGuildAsync(guildId);
            _logger.LogInformation("Registered {count} slash commands", _interactionService.SlashCommands.Count);
            
            _client.InteractionCreated += HandleInteraction;
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            using var activity = Source.StartActivity(nameof(InteractionHandler), ActivityKind.Server);

            var Context = new SocketInteractionContext(_client, interaction);

            SocketInteraction command;
            switch (interaction)
            {
                case SocketAutocompleteInteraction:
                    command = (SocketAutocompleteInteraction) interaction;
                    break;
                case SocketSlashCommand:
                    command = (SocketSlashCommand) interaction;
                    break;
                default:
                    throw new Exception("Unexpected interaction");
            }

            await _interactionService.ExecuteCommandAsync(Context, _serviceProvider);
        }
    }
}
