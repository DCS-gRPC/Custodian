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
using Microsoft.Extensions.Logging;
using RurouniJones.Custodian.Core.Dcs;
using RurouniJones.Custodian.Core.Discord.AutoCompleters;
using static RurouniJones.Custodian.Core.Dcs.PlayerService;
using static RurouniJones.Custodian.Core.Enumerations;

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
                activity?.SetStatus(ActivityStatusCode.Error);
                await RespondAsync($"Could not find player with name of \"{playerName}\"");
                return;
            }

            activity?.AddTag("Player Name", player.Value.Name);
            activity?.AddTag("Player UCID", player.Value.Ucid);

            await RespondAsync($"Kicked \"{player.Value.Name}\" (UCID: {player.Value.Ucid})");
        }

        [SlashCommand("ban-player", "Ban a player from a DCS server. Longest ban duration is 24830 days (Approx 68 years)")]
        public async Task BanPlayerCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Name of the player"), Autocomplete(typeof(PlayerNameAutoCompleteHandler))] string playerName,
            [Summary(description: "Reason for the ban")] string reason,
            [Summary(description: "Period for the ban. Default: 90)")] int period = 90,
            [Summary(description: "Unit of the ban period. Default: Days")] BanPeriodUnit periodUnit = BanPeriodUnit.Days)
        {
            using var activity = Source.StartActivity(nameof(PlayerAdministrationInteraction));
            activity?.AddTag("Command", "ban-player");
            activity?.AddTag("Server", server.ToString());

            var player = await _playerService.BanPlayerAsync(server, playerName, reason, period, periodUnit);
            if (player == null)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                await RespondAsync($"Could not find player with name of \"{playerName}\"");
                return;
            }

            activity?.AddTag("Player Name", player.Value.Name);
            activity?.AddTag("Player UCID", player.Value.Ucid);

            await RespondAsync($"Banned \"{player.Value.Name}\" (UCID: {player.Value.Ucid})");
        }

        [SlashCommand("unban-player", "Unban a player from a DCS server")]
        public async Task UnbanPlayerCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server,
            [Summary(description: "Name of the player"), Autocomplete(typeof(BannedPlayerNameAutoCompleteHandler))] string playerName)
        {
            using var activity = Source.StartActivity(nameof(PlayerAdministrationInteraction));
            activity?.AddTag("Command", "unban-player");
            activity?.AddTag("Server", server.ToString());

            var player = await _playerService.UnbanPlayerAsync(server, playerName);
            if (player == null)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                await RespondAsync($"Could not find player with name of \"{playerName}\"");
                return;
            }

            activity?.AddTag("Player Name", player.Value.Name);
            activity?.AddTag("Player UCID", player.Value.Ucid);

            await RespondAsync($"Unbanned \"{player.Value.Name}\" (UCID: {player.Value.Ucid})");
        }

        [SlashCommand("list-banned-players", "List banned players on a DCS server")]
        public async Task UnbanPlayerCommand(
            [Summary(description: "Name of the server"), Autocomplete(typeof(GameServerAutoCompleteHandler))] string server)
        {
            using var activity = Source.StartActivity(nameof(PlayerAdministrationInteraction));
            activity?.AddTag("Command", "list-banned-players");
            activity?.AddTag("Server", server.ToString());

            var players = (await _playerService.GetBannedPlayersOnServerAsync(server)).ToList();
            if (players == null || players.Count == 0)
            {
                await RespondAsync($"No players found");
                return;
            }

            var embed = BuildBannedPlayersEmbed(players);
            await RespondAsync(embed: embed);
        }

        private static Embed BuildBannedPlayersEmbed(List<PlayerBan> players)
        {
            string[] names = players.Select(x => x.Name).ToArray();
            string[] until = players.Select(x => ToDateString(x.BannedUntil)).ToArray();
            string[] reason = players.Select(x => x.Reason).ToArray();

            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>()
            {
                new EmbedFieldBuilder().WithName("Name").WithValue(string.Join(Environment.NewLine, names)).WithIsInline(true),
                new EmbedFieldBuilder().WithName("Until").WithValue(string.Join(Environment.NewLine, until)).WithIsInline(true),
                new EmbedFieldBuilder().WithName("Reason").WithValue(string.Join(Environment.NewLine, reason)).WithIsInline(true),
            };

            var embed = new EmbedBuilder()
                .WithFields(fields.ToArray())
                .WithCurrentTimestamp();

            return embed.Build();
        }

        private static string ToDateString(ulong unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds((long)unixTime).ToString("yyyy-MM-dd");
        }
    }
}
