/*
 Copyright(C) 2022 Jeffrey Jones

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
using Microsoft.Extensions.Logging;
using RurouniJones.Dcs.Grpc.V0.Hook;
using RurouniJones.Dcs.Grpc.V0.Net;
using static RurouniJones.Custodian.Core.Enumerations;
using static RurouniJones.Dcs.Grpc.V0.Net.GetPlayersResponse.Types;

namespace RurouniJones.Custodian.Core.Dcs
{
    public sealed class PlayerService
    {
        private static readonly ActivitySource Source = new(nameof(Client));

        private readonly ILogger<Client> _logger;
        private readonly Client _client;

        public PlayerService(ILogger<Client> logger, Client client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<List<Player>> GetPlayersOnServerAsync(string server)
        {
            var channel = _client.GetServerChannel(server);
            if (channel == null) return new List<Player>();

            var service = new NetService.NetServiceClient(channel);

            var players = await service.GetPlayersAsync(new GetPlayersRequest{});

            return players.Players.Select(x => ToPlayer(x)).ToList();
        }

        public async Task<Player?> KickPlayerAsync(string server, string playerName, string reason)
        {
            var player = (await GetPlayersOnServerAsync(server)).FirstOrDefault(x => x.Name == playerName);

            var channel = _client.GetServerChannel(server);
            if (channel == null) return null;

            var service = new NetService.NetServiceClient(channel);

            await service.KickPlayerAsync(new KickPlayerRequest()
            {
                Id = player.Id,
                Message = reason
            });

            return player;
        }

        public async Task<Player?> BanPlayerAsync(string server, string playerName, string reason, int period, BanPeriodUnit periodUnit)
        {
            var player = (await GetPlayersOnServerAsync(server)).FirstOrDefault(x => x.Name == playerName);

            var channel = _client.GetServerChannel(server);
            if (channel == null) return null;

            var service = new HookService.HookServiceClient(channel);

            await service.BanPlayerAsync(new BanPlayerRequest()
            {
                Id = player.Id,
                Period = (uint) ToSeconds(period, periodUnit),
                Reason = reason
            });

            return player;
        }

        public async Task<PlayerBan?> UnbanPlayerAsync(string server, string playerName)
        {

            var player = (await GetBannedPlayersOnServerAsync(server)).FirstOrDefault(x => x.Name == playerName);

            var channel = _client.GetServerChannel(server);
            if (channel == null) return null;

            var service = new HookService.HookServiceClient(channel);

            var players = await service.GetBannedPlayersAsync(new GetBannedPlayersRequest());

            await service.UnbanPlayerAsync(new UnbanPlayerRequest()
            {
                Ucid = player.Ucid
            });

            return player;
        }

        public async Task<List<PlayerBan>> GetBannedPlayersOnServerAsync(string server)
        {
            var channel = _client.GetServerChannel(server);
            if (channel == null) return new List<PlayerBan>();

            var service = new HookService.HookServiceClient(channel);

            var players = await service.GetBannedPlayersAsync(new GetBannedPlayersRequest {});

            return players.Bans.Select(x => ToPlayerBan(x)).ToList();
        }

        private static int ToSeconds(int period, BanPeriodUnit periodUnit) => periodUnit switch
        {
            BanPeriodUnit.Minutes => (int) new TimeSpan(0, 0, period, 0).TotalSeconds,
            BanPeriodUnit.Hours => (int) new TimeSpan(0, period, 0, 0).TotalSeconds,
            BanPeriodUnit.Days => (int) new TimeSpan(period, 0, 0, 0).TotalSeconds,
            _ => (int) new TimeSpan(90, 0, 0, 0).TotalSeconds,
        };

        private static Player ToPlayer(GetPlayerInfo protoPlayer)
        {
            return new Player(protoPlayer.Name, protoPlayer.Id, protoPlayer.Ucid);
        }

        private static PlayerBan ToPlayerBan(BanDetails banDetails)
        {
            return new PlayerBan(banDetails.PlayerName, banDetails.Ucid, banDetails.IpAddress,
                banDetails.BannedFrom, banDetails.BannedUntil, banDetails.Reason);
        }

        public readonly record struct Player
        {
            public string Name { get; init; }
            public uint Id { get; init; }
            public string Ucid { get; init; }

            public Player(string name, uint id, string ucid)
            {
                Name = name;
                Id = id;
                Ucid = ucid;
            }
        }

        public readonly record struct PlayerBan
        {
            public string Name { get; init; }
            public string Ucid { get; init; }
            public string IpAddress { get; init; }
            public ulong BannedFrom { get; init; }
            public ulong BannedUntil { get; init; }
            public string Reason { get; init;}

            public PlayerBan(string name, string ucid, string ipAddress, ulong bannedFrom, ulong bannedUntil, string reason)
            {
                Name = name;
                Ucid = ucid;
                IpAddress = ipAddress;
                BannedFrom = bannedFrom;
                BannedUntil = bannedUntil;
                Reason = reason;
            }
        }
    }
}
