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
    [Group("eval", "evaluate arbitrary lua inside the DCS server")]
    public class EvalInteraction : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly ActivitySource Source = new(nameof(ChatInteraction));

        private readonly ILogger<EvalInteraction> _logger;
        private readonly EvalService _evalService;

        public EvalInteraction(ILogger<EvalInteraction> logger, EvalService evalService)
        {
            _logger = logger;
            _evalService = evalService;
        }


        [SlashCommand("code", "provide code via discord")]
        public async Task CodeSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "code")] string code)
        {
            using var activity = Source.StartActivity(nameof(EvalInteraction));
            SetCommonTelemetryTags(activity, "code", server);

            var response = await _evalService.EvalCodeOnServerAsync(server, code);
            await RespondAsync(response);
        }

        [SlashCommand("snippet", "provide code via a snippet in a file")]
        public async Task PlayerSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "snippet"), Autocomplete(typeof(SnippetFileNameAutoCompleteHandler))] string fileName)
        {
            var code = File.ReadAllText(fileName);

            using var activity = Source.StartActivity(nameof(EvalInteraction));
            SetCommonTelemetryTags(activity, "snippet", server);

            var response = await _evalService.EvalCodeOnServerAsync(server, code);
            await RespondAsync(response);
        }

        private static void SetCommonTelemetryTags(Activity? activity, string subCommand, string? server)
        {
            activity?.AddTag("Command", "eval");
            activity?.AddTag("SubCommand", subCommand);
            if (server != null)
                activity?.AddTag("Server", server);
        }
    }
}
