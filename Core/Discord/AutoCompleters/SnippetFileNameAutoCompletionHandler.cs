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

namespace RurouniJones.Custodian.Core.Discord.AutoCompleters
{
    public sealed class SnippetFileNameAutoCompleteHandler : AutocompleteHandler
    {
        private static readonly ActivitySource Source = new(nameof(SnippetFileNameAutoCompleteHandler));

        private readonly ILogger<SnippetFileNameAutoCompleteHandler> _logger;

        public SnippetFileNameAutoCompleteHandler(ILogger<SnippetFileNameAutoCompleteHandler> logger)
        {
            _logger = logger;
        }

        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services)
        {

            if (context.Interaction is not SocketAutocompleteInteraction interaction ||
                interaction.Data.Current.Value.ToString() is not string userInput)
            {
                return AutocompletionResult.FromSuccess();
            }

            List<AutocompleteResult> results = new();
            await Task.Run(() => {
                string[] allFiles = Directory.GetFiles("Snippets", "*.lua", SearchOption.AllDirectories);

                foreach (var filename in allFiles.Where(x => x.Split("\\").Last().StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase)))
                {
                    results.Add(new AutocompleteResult(filename, filename));
                }
            });

            return AutocompletionResult.FromSuccess(results.Take(25));
        }
    }
}
