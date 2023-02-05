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
using Microsoft.Extensions.Logging;
using RurouniJones.Dcs.Grpc.V0.Coalition;
using RurouniJones.Dcs.Grpc.V0.Trigger;

namespace RurouniJones.Custodian.Core.Dcs
{
    public sealed class OutTextService
    {
        private static readonly ActivitySource Source = new(nameof(Client));

        private readonly ILogger<Client> _logger;
        private readonly Client _client;

        public OutTextService(ILogger<Client> logger, Client client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task AnnounceToAllServersAsync(string message, uint displayTime)
        {
            foreach(var channel in _client.GameServers.Values) { 
                var service = new TriggerService.TriggerServiceClient(channel);
                await service.OutTextAsync(new OutTextRequest() {
                    Text = message,
                    DisplayTime = (int) displayTime,
                    ClearView = true,
                });
            }
        }

        public async Task MessageToAllPlayersOnServerAsync(string shortName, string message, uint displayTime)
        {
            var channel = _client.GetServerChannel(shortName);
            var service = new TriggerService.TriggerServiceClient(channel);
            await service.OutTextAsync(new OutTextRequest()
            {
                Text = message,
                DisplayTime = (int) displayTime,
                ClearView = true,
            });
        }

        public async Task MessageToCoalitionOnServerAsync(string shortName, uint coalitionId, string message, uint displayTime)
        {
            var channel = _client.GetServerChannel(shortName);
            var service = new TriggerService.TriggerServiceClient(channel);
            await service.OutTextForCoalitionAsync(new OutTextForCoalitionRequest()
            {
                Coalition = (RurouniJones.Dcs.Grpc.V0.Common.Coalition) coalitionId,
                Text = message,
                DisplayTime = (int) displayTime,
                ClearView = true,
            });
        }

        public async Task MessageToPlayerOnServerAsync(string shortName, string playerName, string message, uint displayTime)
        {
            var channel = _client.GetServerChannel(shortName);

            var coalitionService = new CoalitionService.CoalitionServiceClient(channel);
            var units = new List<RurouniJones.Dcs.Grpc.V0.Common.Unit>();

            units.AddRange((await coalitionService.GetPlayerUnitsAsync(new GetPlayerUnitsRequest()
            {
                Coalition = RurouniJones.Dcs.Grpc.V0.Common.Coalition.Blue
            })).Units.ToList());

            units.AddRange((await coalitionService.GetPlayerUnitsAsync(new GetPlayerUnitsRequest()
            {
                Coalition = RurouniJones.Dcs.Grpc.V0.Common.Coalition.Red
            })).Units.ToList());

            var unitId = units.FirstOrDefault(x => x.PlayerName == playerName)?.Id;

            if(unitId == null) {
                _logger.LogWarning("No Unit ID found for player \"{player}\". Skipping message send.", playerName);
                return;
            }
    
            var service = new TriggerService.TriggerServiceClient(channel);
            await service.OutTextForUnitAsync(new OutTextForUnitRequest()
            {
                Text = message,
                UnitId = (uint) unitId,
                DisplayTime = (int) displayTime,
                ClearView = true,
            });
        }
    }
}
