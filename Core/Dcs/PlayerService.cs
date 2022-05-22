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
using RurouniJones.Dcs.Grpc.V0.Net;
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

        public async Task<List<Player>> GetPlayersOnServer(string server)
        {
            var channel = _client.GetServerChannel(server);
            if (channel == null) return new List<Player>();

            var service = new NetService.NetServiceClient(channel);

            var players = await service.GetPlayersAsync(new GetPlayersRequest{});

            return players.Players.Select(x => ToPlayer(x)).ToList();
        }

        private static Player ToPlayer(GetPlayerInfo protoPlayer)
        {
            return new Player(protoPlayer.Name, protoPlayer.Id, protoPlayer.Ucid);
        }

        public record struct Player
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
    }
}
