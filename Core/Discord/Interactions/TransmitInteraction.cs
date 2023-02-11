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
using static RurouniJones.Custodian.Core.Enumerations;

namespace RurouniJones.Custodian.Core.Discord.Interactions
{
    [Group("transmit", "Transmit a message using Text-To-Speech on an SRS frequency")]
    public class TransmitInteraction : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly ActivitySource Source = new(nameof(ChatInteraction));

        private readonly ILogger<ChatInteraction> _logger;
        private readonly TransmitService _transmitService;

        public TransmitInteraction(ILogger<ChatInteraction> logger, TransmitService transmitService)
        {
            _logger = logger;
            _transmitService = transmitService;
        }

        [SlashCommand("announce", "Transmit to everyone on all DCS servers")]
        public async Task AnnounceSubCommand(
            [Summary(description: "Message to transmit")] string message,
            [Summary(description: "Frequency in MHz")] decimal frequency
            )
        {
            using var activity = Source.StartActivity(nameof(ChatInteraction));
            SetCommonTelemetryTags(activity, "announce", null, message);

            await _transmitService.AnnounceToAllServersAsync(message, message, frequency);
            await RespondAsync($"Message sent");
        }

        [SlashCommand("all", "Transmit to everyone on a DCS server")]
        public async Task AllSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Message to transmit")] string message,
            [Summary(description: "Frequency in MHz")] decimal frequency)
        {
            using var activity = Source.StartActivity(nameof(ChatInteraction));
            SetCommonTelemetryTags(activity, "all", server, message);

            await _transmitService.TransmitToAllPlayersOnServerAsync(server, message, message, frequency);
            await RespondAsync($"Message sent");
        }           

        [SlashCommand("coalition", "Transmit to a coalition on a DCS server")]
        public async Task CoalitionSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Coalition to transmit to")] Coalition coalition,
            [Summary(description: "Message to transmit")] string message,
            [Summary(description: "Frequency in MHz")] decimal frequency
            )
        {
            using var activity = Source.StartActivity(nameof(ChatInteraction));
            SetCommonTelemetryTags(activity, "coalition", server, message);
            activity?.AddTag("Coalition", coalition.ToString());

            await _transmitService.TransmitToCoalitionOnServerAsync(server, (uint)coalition, message, message, frequency);
            await RespondAsync($"Message sent");
        }

        private static void SetCommonTelemetryTags(Activity? activity, string subCommand, string? server, string message)
        {
            activity?.AddTag("Command", "transmit");
            activity?.AddTag("SubCommand", subCommand);
            if (server != null)
                activity?.AddTag("Server", server);
            activity?.AddTag("Message", message);
        }
    }
}
