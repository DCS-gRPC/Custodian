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
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RurouniJones.Custodian.Configuration;

namespace RurouniJones.Custodian.Core.Dcs
{
    public class Client
    {
        private static readonly ActivitySource Source = new(nameof(Client));

        private readonly ILogger<Client> _logger;

        public readonly Dictionary<GameServer, GrpcChannel> GameServers = new();

        public Client(ILogger<Client> logger, IOptions<Application> applicationConfiguration)
        {
            _logger = logger;

            var handler = new SocketsHttpHandler
            {
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                EnableMultipleHttp2Connections = true,
            };

            foreach (var gameServer in applicationConfiguration.Value.GameServers)
            {
                GameServers.Add(gameServer,
                    GrpcChannel.ForAddress($"http://{gameServer.Rpc.Host}:{gameServer.Rpc.Port}",
                        new GrpcChannelOptions
                        {
                            HttpHandler = handler
                        }
                    )
                );
            }
        }

        public GrpcChannel? GetServerChannel(string shortName)
        {
            return GameServers.First(x => x.Key.ShortName.ToLower() == shortName.ToLower()).Value;
        }
    }
}
