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

using Microsoft.Extensions.Logging;
using RurouniJones.Dcs.Grpc.V0.Net;

namespace RurouniJones.Custodian.Core.Dcs
{
    public class ChatService
    {
        private readonly ILogger<Client> _logger;
        private readonly Client _client;

        public ChatService(ILogger<Client> logger, Client client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task AnnounceToAllServersAsync(string message)
        {
            foreach (var channel in _client.GameServers.Values)
            {
                var service = new NetService.NetServiceClient(channel);
                await service.SendChatAsync(new SendChatRequest()
                {
                    Coalition = RurouniJones.Dcs.Grpc.V0.Common.Coalition.All,
                    Message= message
                });
            }
        }

        public async Task ChatToAllPlayersOnServerAsync(string shortName, string message)
        {
            var channel = _client.GetServerChannel(shortName);
            var service = new NetService.NetServiceClient(channel);
            await service.SendChatAsync(new SendChatRequest()
            {
                Coalition = RurouniJones.Dcs.Grpc.V0.Common.Coalition.All,
                Message = message
            });
        }

        public async Task ChatToCoalitionOnServerAsync(string shortName, uint coalitionId, string message)
        {
            var channel = _client.GetServerChannel(shortName);
            var service = new NetService.NetServiceClient(channel);
            await service.SendChatAsync(new SendChatRequest()
            {
                Coalition = (RurouniJones.Dcs.Grpc.V0.Common.Coalition) coalitionId,
                Message = message
            });
        }

        public async Task ChatToPlayerOnServerAsync(string shortName, string playerName, string message)
        {
            var channel = _client.GetServerChannel(shortName);

            var service = new NetService.NetServiceClient(channel);

            var players = (await service.GetPlayersAsync(new GetPlayersRequest())).Players.ToList();
            var playerId = players.FirstOrDefault(x => x.Name == playerName)?.Id;

            if (playerId == null)
            {
                _logger.LogWarning("No Player ID found for player \"{player}\". Skipping message send.", playerName);
                return;
            }

            await service.SendChatToAsync(new SendChatToRequest()
            {
                TargetPlayerId = (uint) playerId,
                Message = message
            });
        }

    }
}
