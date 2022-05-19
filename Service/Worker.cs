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

using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace RurouniJones.Custodian.Service
{
    public class Worker : BackgroundService
    {
        private static readonly ActivitySource Source = new(nameof(Worker));

        private readonly ILogger<Worker> _logger;
        private readonly Configuration.Application _configuration;
        private readonly Core.Discord.Client _discordClient;

        public Worker(ILogger<Worker> logger, IOptions<Configuration.Application> configuration, Core.Discord.Client discordClient)
        {
            _logger = logger;
            _configuration = configuration.Value;
            _discordClient = discordClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = _discordClient.StartAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var activity = Source.StartActivity(nameof(Worker));
                activity?.AddTag("TagKey", "TagValue");
                _logger.LogInformation("{name} Worker running at: {time}", _configuration.Name, DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
