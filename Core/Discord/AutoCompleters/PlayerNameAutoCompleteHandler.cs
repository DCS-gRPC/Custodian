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
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using RurouniJones.Custodian.Core.Dcs;

namespace RurouniJones.Custodian.Core.Discord.AutoCompleters
{
    public sealed class PlayerNameAutoCompleteHandler : AutocompleteHandler
    {
        private static readonly ActivitySource Source = new(nameof(PlayerNameAutoCompleteHandler));

        private readonly ILogger<PlayerNameAutoCompleteHandler> _logger;
        private readonly PlayerService _playerService;

        public PlayerNameAutoCompleteHandler(ILogger<PlayerNameAutoCompleteHandler> logger, PlayerService playerService)
        {
            _logger = logger;
            _playerService = playerService;
        }

        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            if (context.Interaction is not SocketAutocompleteInteraction interaction ||
                interaction.Data.Current.Value.ToString() is not string userInput) { 
                return AutocompletionResult.FromSuccess();
            }

            var server = interaction.Data.Options.First(x => x.Name == "server").Value.ToString();
            if(string.IsNullOrEmpty(server))
            {
                return AutocompletionResult.FromSuccess();
            }

            var players = await _playerService.GetPlayersOnServerAsync(server);
            players.RemoveAll(x => x.Id == 1); // 1 is always the server "player". Never an actual player.

            List <AutocompleteResult> results = new();

            foreach (var player in players.Where(x => x.Name.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase)))
            {
                results.Add(new AutocompleteResult(player.Name, player.Name));
            }

            return AutocompletionResult.FromSuccess(results.Take(25));
        }
    }
}
