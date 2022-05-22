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
    [Group("out-text", "Display a DCS OutText message box to players")]
    public class OutTextInteraction : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly ActivitySource Source = new(nameof(OutTextInteraction));

        private readonly ILogger<OutTextInteraction> _logger;
        private readonly MessageService _messageService;

        public enum Coalition
        {
            Bluefor = 3,
            RedFor = 2,
            Neutral = 1
        }

        public OutTextInteraction(ILogger<OutTextInteraction> logger, MessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        [SlashCommand("announce", "Display to everyone on all servers")]
        public async Task AnnounceSubCommand(
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] uint displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutTextInteraction));
            SetCommonTelemetryTags(activity, "announce", null, message, displayTime);

            await _messageService.AnnounceToAllServersAsync(message, displayTime);
            await RespondAsync($"Message sent");
        }

        [SlashCommand("all", "Display to everyone on a server")]
        public async Task AllSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] uint displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutTextInteraction));
            SetCommonTelemetryTags(activity, "all", server, message, displayTime);

            await _messageService.MessageToAllPlayersOnServerAsync(server, message, displayTime);
            await RespondAsync($"Message sent");
        }           

        [SlashCommand("coalition", "Display to a coalition on a server")]
        public async Task CoalitionSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Coalition to send the message to")] Coalition coalition,
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] uint displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutTextInteraction));
            SetCommonTelemetryTags(activity, "coalition", server, message, displayTime);
            activity?.AddTag("Coalition", coalition.ToString());

            await _messageService.MessageToCoalitionOnServerAsync(server, (uint) coalition , message, displayTime);
            await RespondAsync($"Message sent");
        }

        [SlashCommand("player", "Display to a player on a server")]
        public async Task PlayerSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Player to send the message to"), Autocomplete(typeof(PlayerNameAutoCompleteHandler))] string playerName,
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] uint displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutTextInteraction));
            SetCommonTelemetryTags(activity, "player", server, message, displayTime);
            activity?.AddTag("PlayerName", playerName);

            await _messageService.MessageToPlayerOnServerAsync(server, playerName, message, displayTime);
            await RespondAsync($"Message sent");
        }

        private static void SetCommonTelemetryTags(Activity? activity, string subCommand, string? server, string message, uint displayTime)
        {
            activity?.AddTag("Command", "out-text");
            activity?.AddTag("SubCommand", subCommand);
            if (server != null)
                activity?.AddTag("Server", server);
            activity?.AddTag("Message", message);
            activity?.AddTag("DisplayTime", displayTime);
        }
    }
}
