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
using RurouniJones.Dcs.Grpc.V0.Tts;

namespace RurouniJones.Custodian.Core.Dcs
{
    public class TransmitService
    {
        private readonly ILogger<Client> _logger;
        private readonly Client _client;

        public TransmitService(ILogger<Client> logger, Client client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task AnnounceToAllServersAsync(string ssml, string plaintext, decimal frequency)
        {
            foreach (var channel in _client.GameServers.Values)
            {
                var service = new TtsService.TtsServiceClient(channel);
                await service.TransmitAsync(new TransmitRequest()
                {
                    Coalition = RurouniJones.Dcs.Grpc.V0.Common.Coalition.Blue,
                    Frequency = (ulong)frequency * 1_000_000,
                    Ssml = ssml,
                    Plaintext = plaintext,
                    SrsClientName = "Custodian"
                });

            }
        }

        public async Task TransmitToAllPlayersOnServerAsync(string shortName, string ssml, string plaintext, decimal frequency)
        {
            var channel = _client.GetServerChannel(shortName);
            var service = new TtsService.TtsServiceClient(channel);
            await service.TransmitAsync(new TransmitRequest()
            {
                Coalition = RurouniJones.Dcs.Grpc.V0.Common.Coalition.Blue,
                Frequency = (ulong)frequency * 1_000_000,
                Ssml = ssml,
                Plaintext = plaintext,
                SrsClientName = "Custodian",
                Async = true
            });

            await service.TransmitAsync(new TransmitRequest()
            {
                Coalition = RurouniJones.Dcs.Grpc.V0.Common.Coalition.Red,
                Frequency = (ulong)frequency * 1_000_000,
                Ssml = ssml,
                Plaintext = plaintext,
                SrsClientName = "Custodian",
                Async = true
            });
        }

        public async Task TransmitToCoalitionOnServerAsync(string shortName, uint coalitionId, string ssml, string plaintext, decimal frequency)
        {
            var channel = _client.GetServerChannel(shortName);
            var service = new TtsService.TtsServiceClient(channel);
            await service.TransmitAsync(new TransmitRequest()
            {
                Coalition = (RurouniJones.Dcs.Grpc.V0.Common.Coalition)coalitionId,
                Frequency = (ulong)frequency * 1_000_000,
                Ssml = ssml,
                Plaintext = plaintext,
                SrsClientName = "Custodian",
                Async = true
            });
        }
    }
}
