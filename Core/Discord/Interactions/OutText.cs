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

namespace RurouniJones.Custodian.Core.Discord.Interactions
{
    [Group("out-text", "Display a DCS OutText message box to players")]
    public class OutText : InteractionModuleBase<SocketInteractionContext>
    {
        private static readonly ActivitySource Source = new(nameof(OutText));

        private readonly ILogger<OutText> _logger;

        public enum Coalition
        {
            RedFor = 1,
            Bluefor = 2,
            Neutral = 3
        }

        public OutText(ILogger<OutText> logger)
        {
            _logger = logger;
        }

        [SlashCommand("announce", "Display to everyone on all servers")]
        public async Task AllSubCommand(
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] int displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutText));
            activity?.AddTag("Command", "out-text");
            activity?.AddTag("SubCommand", "announce");
            activity?.AddTag("Message", message);
            activity?.AddTag("DisplayTime", displayTime);

            await RespondAsync($"out-text announce to all servers with the message '{message}' and a {displayTime} second display time");
        }

        [SlashCommand("all", "Display to everyone on a server")]
        public async Task AllSubCommand(
            [Summary(description: "Shortname of the server")] string server,
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] int displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutText));
            activity?.AddTag("Command", "out-text");
            activity?.AddTag("SubCommand", "all");
            activity?.AddTag("Server", server);
            activity?.AddTag("Message", message);
            activity?.AddTag("DisplayTime", displayTime);

            await RespondAsync($"out-text all called for the '{server}' server with the message '{message}' and a {displayTime} second display time");
        }           

        [SlashCommand("coalition", "Display to a coalition on a server")]
        public async Task CoalitionSubCommand(
            [Summary(description: "Shortname of the server")] string server,
            [Summary(description: "Coalition to send the message to")] Coalition coalition,
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] int displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutText));
            activity?.AddTag("Command", "out-text");
            activity?.AddTag("SubCommand", "coalition");
            activity?.AddTag("Server", server);
            activity?.AddTag("Coalition", coalition.ToString());
            activity?.AddTag("Message", message);
            activity?.AddTag("DisplayTime", displayTime);

            await RespondAsync($"out-text coalition called for '{server}' server to the {coalition} coalition with the message '{message}' and a {displayTime} second display time");
        }

        [SlashCommand("group", "Display to a group on a server")]
        public async Task GroupSubCommand(
            [Summary(description: "Shortname of the server")] string server,
            [Summary(description: "Group name to send the message to")] string group,
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] int displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutText));
            activity?.AddTag("Command", "out-text");
            activity?.AddTag("SubCommand", "group");
            activity?.AddTag("Server", server);
            activity?.AddTag("Group", group);
            activity?.AddTag("Message", message);
            activity?.AddTag("DisplayTime", displayTime);

            await RespondAsync($"out-text group called for '{server}' to the '{group}' group with the message '{message}' and a {displayTime} second display time");
        }

        [SlashCommand("unit", "Display to a unit on a server")]
        public async Task UnitSubCommand(
            [Summary(description: "Shortname of the server")] string server,
            [Summary(description: "Unit name to send the message to")] string unit,
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] int displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutText));
            activity?.AddTag("Command", "out-text");
            activity?.AddTag("SubCommand", "unit");
            activity?.AddTag("Server", server);
            activity?.AddTag("Unit", unit);
            activity?.AddTag("Message", message);
            activity?.AddTag("DisplayTime", displayTime);

            await RespondAsync($"out-text unit called for '{server}' server to the '{unit}' unit with the message '{message}' and a {displayTime} second display time");
        }

        [SlashCommand("player", "Display to a player on a server")]
        public async Task PlayerSubCommand(
            [Summary(description: "Shortname of the server")] string server,
            [Summary(description: "Player name to send the message to")] string player,
            [Summary(description: "Message to display")] string message,
            [Summary(description: "Time in seconds to display the message. Default 15")] int displayTime = 15)
        {
            using var activity = Source.StartActivity(nameof(OutText));
            activity?.AddTag("Command", "out-text");
            activity?.AddTag("SubCommand", "player");
            activity?.AddTag("Server", server);
            activity?.AddTag("Player", player);
            activity?.AddTag("Message", message);
            activity?.AddTag("DisplayTime", displayTime);

            await RespondAsync($"out-text player called for '{server}' to the player '{player}' with the message '{message}' and a {displayTime} second display time");
        }
    }
}
