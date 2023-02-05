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
    [Group("chat", "Display a chat message in the DCS chat")]
    public class ChatInteraction : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly ActivitySource Source = new(nameof(ChatInteraction));

        private readonly ILogger<ChatInteraction> _logger;
        private readonly ChatService _chatService;

        public ChatInteraction(ILogger<ChatInteraction> logger, ChatService chatService)
        {
            _logger = logger;
            _chatService = chatService;
        }

        [SlashCommand("announce", "Display to everyone on all DCS servers")]
        public async Task AnnounceSubCommand(
            [Summary(description: "Message to display")] string message)
        {
            using var activity = Source.StartActivity(nameof(ChatInteraction));
            SetCommonTelemetryTags(activity, "announce", null, message);

            await _chatService.AnnounceToAllServersAsync(message);
            await RespondAsync($"Message sent");
        }

        [SlashCommand("all", "Display to everyone on a DCS server")]
        public async Task AllSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Message to display")] string message)
        {
            using var activity = Source.StartActivity(nameof(ChatInteraction));
            SetCommonTelemetryTags(activity, "all", server, message);

            await _chatService.ChatToAllPlayersOnServerAsync(server, message);
            await RespondAsync($"Message sent");
        }           

        [SlashCommand("coalition", "Display to a coalition on a DCS server")]
        public async Task CoalitionSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Coalition to send the message to")] Coalition coalition,
            [Summary(description: "Message to display")] string message)
        {
            using var activity = Source.StartActivity(nameof(ChatInteraction));
            SetCommonTelemetryTags(activity, "coalition", server, message);
            activity?.AddTag("Coalition", coalition.ToString());

            await _chatService.ChatToCoalitionOnServerAsync(server, (uint) coalition , message);
            await RespondAsync($"Message sent");
        }

        [SlashCommand("player", "Display to a player on a DCS server")]
        public async Task PlayerSubCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Player to send the message to"), Autocomplete(typeof(PlayerNameAutoCompleteHandler))] string playerName,
            [Summary(description: "Message to display")] string message)
        {
            using var activity = Source.StartActivity(nameof(ChatInteraction));
            SetCommonTelemetryTags(activity, "player", server, message);
            activity?.AddTag("PlayerName", playerName);

            await _chatService.ChatToPlayerOnServerAsync(server, playerName, message);
            await RespondAsync($"Message sent");
        }

        private static void SetCommonTelemetryTags(Activity? activity, string subCommand, string? server, string message)
        {
            activity?.AddTag("Command", "chat");
            activity?.AddTag("SubCommand", subCommand);
            if (server != null)
                activity?.AddTag("Server", server);
            activity?.AddTag("Message", message);
        }
    }
}
