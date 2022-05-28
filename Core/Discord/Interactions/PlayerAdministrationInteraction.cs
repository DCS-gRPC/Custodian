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
using Microsoft.Extensions.Logging;
using RurouniJones.Custodian.Core.Dcs;
using RurouniJones.Custodian.Core.Discord.AutoCompleters;

namespace RurouniJones.Custodian.Core.Discord.Interactions
{
    public class PlayerAdministrationInteraction : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly ActivitySource Source = new(nameof(PlayerAdministrationInteraction));

        private readonly ILogger<PlayerAdministrationInteraction> _logger;
        private readonly PlayerService _playerService;

        public PlayerAdministrationInteraction(ILogger<PlayerAdministrationInteraction> logger, PlayerService playerService)
        {
            _logger = logger;
            _playerService = playerService;
        }

        [SlashCommand("kick-player", "Kick a player from a DCS server")]
        public async Task KickPlayerCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Name of the player"), Autocomplete(typeof(PlayerNameAutoCompleteHandler))] string playerName,
            [Summary(description: "Reason for the kick")] string reason)
        {
            using var activity = Source.StartActivity(nameof(PlayerAdministrationInteraction));
            activity?.AddTag("Command", "kick-player");
            activity?.AddTag("Server", server.ToString());

            var player = await _playerService.KickPlayerAsync(server, playerName, reason);
            if (player == null) {
                await RespondAsync($"Could not find player with name of \"{playerName}\"");
                return;
            }
            await RespondAsync($"Kicked \"{player.Value.Name}\" (UCID: {player.Value.Ucid})");
        }
    }
}
