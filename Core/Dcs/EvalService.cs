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
using RurouniJones.Dcs.Grpc.V0.Custom;

namespace RurouniJones.Custodian.Core.Dcs
{
    public class EvalService
    {
        private readonly ILogger<Client> _logger;
        private readonly Client _client;

        public EvalService(ILogger<Client> logger, Client client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<string> EvalCodeOnServerAsync(string shortName, string code)
        {
            var channel = _client.GetServerChannel(shortName);
            var service = new CustomService.CustomServiceClient(channel);
            var response = await service.EvalAsync(new EvalRequest()
            {
                Lua = code
            });

            return response.Json;
        }
    }
}
