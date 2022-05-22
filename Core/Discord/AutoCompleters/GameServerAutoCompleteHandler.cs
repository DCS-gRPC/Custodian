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
using Microsoft.Extensions.Options;
using RurouniJones.Custodian.Configuration;

namespace RurouniJones.Custodian.Core.Discord.AutoCompleters
{
    public sealed class GameServerAutoCompleteHandler : AutocompleteHandler
    {
        private static readonly ActivitySource Source = new(nameof(GameServerAutoCompleteHandler));

        private readonly ILogger<GameServerAutoCompleteHandler> _logger;
        private readonly List<AutocompleteResult> _servers = new();

        public GameServerAutoCompleteHandler(ILogger<GameServerAutoCompleteHandler> logger, IOptions<Application> applicationConfiguration)
        {
            _logger = logger;

            foreach(var gameServer in applicationConfiguration.Value.GameServers)
            {
                _servers.Add(new AutocompleteResult(gameServer.Name, gameServer.ShortName));
            }
        }

        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services)
        {
            var userInput = (context.Interaction as SocketAutocompleteInteraction).Data.Current.Value.ToString();
            var results = _servers.Where(x => x.Name.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase));

            return AutocompletionResult.FromSuccess(results.Take(25));
        }
    }
}
